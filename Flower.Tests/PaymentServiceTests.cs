using System;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services;
using Flower.Backend.Services.Interfaces;
using Flower.Data;
using Flower.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Flower.Tests
{
    public class PaymentServiceTests
    {
        private readonly Mock<IOrderCancellationService> _orderCancellationMock = new();
        private readonly Mock<IDeliverySlotService> _deliverySlotMock = new();
        private readonly Mock<IEmailService> _emailServiceMock = new();
        private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());

        private ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private IConfiguration CreateConfig(string secret = "test-webhook-secret")
        {
            var configData = new Dictionary<string, string?>
            {
                { "WebhookSettings:SecretKey", secret }
            };
            return new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();
        }

        private PaymentService CreatePaymentService(ApplicationDbContext context)
        {
            return new PaymentService(
                context,
                _orderCancellationMock.Object,
                new StockLockService(_memoryCache),
                _deliverySlotMock.Object,
                _emailServiceMock.Object,
                CreateConfig());
        }

        [Fact]
        public async Task ProcessWebhook_InvalidSignature_ShouldReturnFalse()
        {
            using var context = CreateInMemoryDbContext();
            var service = CreatePaymentService(context);

            var request = new PaymentWebhookRequest
            {
                OrderId = 1,
                Amount = 100,
                Status = "success",
                TransactionId = "txn-001",
                Signature = "invalid-signature"
            };

            var result = await service.ProcessWebhook(request);

            Assert.False(result);
        }

        [Fact]
        public async Task ProcessWebhook_MissingSignature_ShouldReturnFalse()
        {
            using var context = CreateInMemoryDbContext();
            var service = CreatePaymentService(context);

            var request = new PaymentWebhookRequest
            {
                OrderId = 1,
                Amount = 100,
                Status = "success",
                TransactionId = "txn-001",
                Signature = null
            };

            var result = await service.ProcessWebhook(request);

            Assert.False(result);
        }

        [Fact]
        public async Task RecordPayment_ShouldCreatePaymentAndUpdateOrder()
        {
            using var context = CreateInMemoryDbContext();

            var order = new Order
            {
                CustomerId = 0,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var service = CreatePaymentService(context);
            var result = await service.RecordPayment(order.Id, 150, PaymentMethod.OnlinePayment, "txn-002");

            Assert.NotNull(result);
            Assert.Equal(order.Id, result.OrderId);
            Assert.Equal(150, result.Amount);
            Assert.Equal(PaymentMethod.OnlinePayment, result.Method);
            Assert.Equal(PaymentStatus.Completed, result.Status);
            Assert.Equal("txn-002", result.TransactionId);

            var updatedOrder = await context.Orders.FindAsync(order.Id);
            Assert.NotNull(updatedOrder);
            Assert.Equal(PaymentStatus.Completed, updatedOrder.PaymentStatus);
            Assert.Equal(OrderStatus.Confirmed, updatedOrder.Status);
        }

        [Fact]
        public async Task GetByOrderId_ExistingPayment_ShouldReturnPayment()
        {
            using var context = CreateInMemoryDbContext();

            var order = new Order { CustomerId = 0, OrderDate = DateTime.UtcNow };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var payment = new Payment
            {
                OrderId = order.Id,
                Amount = 200,
                Method = PaymentMethod.COD,
                Status = PaymentStatus.Completed
            };
            context.Payments.Add(payment);
            await context.SaveChangesAsync();

            var service = CreatePaymentService(context);
            var result = await service.GetByOrderId(order.Id);

            Assert.NotNull(result);
            Assert.Equal(order.Id, result.OrderId);
        }
    }
}
