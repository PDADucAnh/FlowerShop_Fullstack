using Flower.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace Flower.Data
{
    public interface IApplicationDbContext
    {
        DatabaseFacade Database { get; }
        DbSet<Category> Categories { get; set; }
        DbSet<Post> Posts { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<CategoryProduct> CategoriesProducts { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<Customer> Customers { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<OrderDetail> OrderDetails { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
        DbSet<Advertisement> Advertisements { get; set; }
        DbSet<DeliverySlot> DeliverySlots { get; set; }
        DbSet<Payment> Payments { get; set; }
        DbSet<PhoneBlacklist> PhoneBlacklists { get; set; }
        DbSet<ProductVariant> ProductVariants { get; set; }
        DbSet<CustomerAddress> CustomerAddresses { get; set; }
        DbSet<PaymentMethodDefinition> PaymentMethods { get; set; }
        DbSet<CustomerPaymentPreference> CustomerPaymentPreferences { get; set; }
        DbSet<PaymentAttempt> PaymentAttempts { get; set; }
        DbSet<CancellationPolicy> CancellationPolicies { get; set; }
        DbSet<Refund> Refunds { get; set; }
        DbSet<Notification> Notifications { get; set; }
        DbSet<EmailHistory> EmailHistories { get; set; }
        DbSet<PromotionCampaign> PromotionCampaigns { get; set; }
        DbSet<PromotionProduct> PromotionProducts { get; set; }
        DbSet<Coupon> Coupons { get; set; }
        DbSet<CouponUsage> CouponUsages { get; set; }
        DbSet<FlashSale> FlashSales { get; set; }
        DbSet<FlashSaleProduct> FlashSaleProducts { get; set; }

        EntityEntry Entry(object entity);
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
