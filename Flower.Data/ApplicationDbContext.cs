using Flower.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flower.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        private bool IsPostgres => Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL";

        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CategoryProduct> CategoriesProducts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<DeliverySlot> DeliverySlots { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PhoneBlacklist> PhoneBlacklists { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<PaymentMethodDefinition> PaymentMethods { get; set; }
        public DbSet<CustomerPaymentPreference> CustomerPaymentPreferences { get; set; }
        public DbSet<PaymentAttempt> PaymentAttempts { get; set; }
        public DbSet<CancellationPolicy> CancellationPolicies { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EmailHistory> EmailHistories { get; set; }
        public DbSet<PromotionCampaign> PromotionCampaigns { get; set; }
        public DbSet<PromotionProduct> PromotionProducts { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponUsage> CouponUsages { get; set; }
        public DbSet<FlashSale> FlashSales { get; set; }
        public DbSet<FlashSaleProduct> FlashSaleProducts { get; set; }
        public DbSet<AdminNotification> AdminNotifications { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Phone)
                .HasDatabaseName("IX_Customers_Phone");

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.ResetToken)
                .HasDatabaseName("IX_Customers_ResetToken")
                .HasFilter(IsPostgres ? "\"ResetToken\" IS NOT NULL" : "[ResetToken] IS NOT NULL");

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Sku)
                .IsUnique()
                .HasFilter(IsPostgres ? "\"Sku\" IS NOT NULL" : "[Sku] IS NOT NULL");

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.CategoryProduct)
                .WithMany(cp => cp.Products)
                .HasForeignKey(p => p.CategoryProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany()
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Status)
                .HasDatabaseName("IX_Orders_Status")
                .IncludeProperties(o => new { o.OrderDate, o.PaymentMethod });

            modelBuilder.Entity<Order>()
                .HasIndex(o => new { o.Status, o.OrderDate })
                .HasDatabaseName("IX_Orders_Status_OrderDate");

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderDate)
                .HasDatabaseName("IX_Orders_OrderDate");

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.TokenHash)
                .HasDatabaseName("IX_RefreshTokens_TokenHash")
                .IsUnique();

            modelBuilder.Entity<DeliverySlot>()
                .HasIndex(ds => new { ds.ProductId, ds.DeliveryDate, ds.TimeSlot, ds.IsActive })
                .HasDatabaseName("IX_DeliverySlots_ProductId_DeliveryDate_TimeSlot_IsActive");

            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Product)
                .WithMany()
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PhoneBlacklist>()
                .HasIndex(pb => new { pb.PhoneNumber, pb.IsActive })
                .HasDatabaseName("IX_PhoneBlacklist_PhoneNumber_IsActive");

            modelBuilder.Entity<CustomerAddress>()
                .HasOne(ca => ca.Customer)
                .WithMany()
                .HasForeignKey(ca => ca.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerAddress>()
                .HasIndex(ca => new { ca.CustomerId, ca.IsDefault })
                .HasDatabaseName("IX_CustomerAddresses_CustomerId_IsDefault")
                .HasFilter(IsPostgres ? "\"IsDefault\" = true" : "[IsDefault] = 1");

            modelBuilder.Entity<PaymentMethodDefinition>()
                .HasIndex(pm => pm.Code)
                .IsUnique();

            modelBuilder.Entity<CustomerPaymentPreference>()
                .HasOne(cpp => cpp.Customer)
                .WithMany()
                .HasForeignKey(cpp => cpp.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerPaymentPreference>()
                .HasOne(cpp => cpp.PaymentMethod)
                .WithMany()
                .HasForeignKey(cpp => cpp.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerPaymentPreference>()
                .HasIndex(cpp => new { cpp.CustomerId, cpp.PaymentMethodId })
                .IsUnique();

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.PaymentMethodRef)
                .WithMany()
                .HasForeignKey(p => p.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<PaymentAttempt>()
                .HasOne(pa => pa.Payment)
                .WithMany()
                .HasForeignKey(pa => pa.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentAttempt>()
                .HasIndex(pa => new { pa.PaymentId, pa.AttemptNumber })
                .IsUnique();

            modelBuilder.Entity<DeliverySlot>()
                .HasIndex(ds => new { ds.DeliveryDate, ds.TimeSlot, ds.IsActive })
                .HasDatabaseName("IX_DeliverySlots_Date_TimeSlot_IsActive");

            modelBuilder.Entity<Refund>()
                .HasOne(r => r.Order)
                .WithMany()
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Refund>()
                .HasOne(r => r.Payment)
                .WithMany()
                .HasForeignKey(r => r.PaymentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<Refund>()
                .HasIndex(r => r.OrderId)
                .HasDatabaseName("IX_Refunds_OrderId");

            modelBuilder.Entity<CancellationPolicy>()
                .HasIndex(cp => cp.OrderStatus)
                .IsUnique()
                .HasDatabaseName("IX_CancellationPolicies_OrderStatus");

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Customer)
                .WithMany()
                .HasForeignKey(n => n.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasIndex(n => n.CustomerId)
                .HasDatabaseName("IX_Notifications_CustomerId");

            modelBuilder.Entity<Notification>()
                .HasIndex(n => new { n.CustomerId, n.IsRead })
                .HasDatabaseName("IX_Notifications_CustomerId_IsRead");

            modelBuilder.Entity<EmailHistory>()
                .HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<EmailHistory>()
                .HasIndex(e => e.OrderId)
                .HasDatabaseName("IX_EmailHistories_OrderId");

            modelBuilder.Entity<EmailHistory>()
                .HasIndex(e => e.EmailType)
                .HasDatabaseName("IX_EmailHistories_EmailType");

            modelBuilder.Entity<PromotionProduct>()
                .HasOne(pp => pp.Promotion)
                .WithMany(p => p.PromotionProducts)
                .HasForeignKey(pp => pp.PromotionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PromotionProduct>()
                .HasOne(pp => pp.Product)
                .WithMany()
                .HasForeignKey(pp => pp.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PromotionProduct>()
                .HasIndex(pp => new { pp.PromotionId, pp.ProductId })
                .IsUnique()
                .HasDatabaseName("IX_PromotionProducts_PromotionId_ProductId");

            modelBuilder.Entity<PromotionCampaign>()
                .HasIndex(pc => new { pc.IsActive, pc.StartDate, pc.EndDate })
                .HasDatabaseName("IX_PromotionCampaigns_Active_StartDate_EndDate");

            modelBuilder.Entity<Coupon>()
                .HasIndex(c => c.Code)
                .IsUnique()
                .HasDatabaseName("IX_Coupons_Code");

            modelBuilder.Entity<CouponUsage>()
                .HasOne(cu => cu.Coupon)
                .WithMany(c => c.Usages)
                .HasForeignKey(cu => cu.CouponId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CouponUsage>()
                .HasOne(cu => cu.Customer)
                .WithMany()
                .HasForeignKey(cu => cu.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CouponUsage>()
                .HasOne(cu => cu.Order)
                .WithMany()
                .HasForeignKey(cu => cu.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CouponUsage>()
                .HasIndex(cu => cu.OrderId)
                .IsUnique()
                .HasDatabaseName("IX_CouponUsages_OrderId");

            modelBuilder.Entity<CouponUsage>()
                .HasIndex(cu => new { cu.CouponId, cu.CustomerId })
                .HasDatabaseName("IX_CouponUsages_CouponId_CustomerId");

            modelBuilder.Entity<FlashSale>()
                .HasIndex(fs => new { fs.IsActive, fs.StartDate, fs.EndDate })
                .HasDatabaseName("IX_FlashSales_Active_StartDate_EndDate");

            modelBuilder.Entity<FlashSaleProduct>()
                .HasOne(fsp => fsp.FlashSale)
                .WithMany(fs => fs.FlashSaleProducts)
                .HasForeignKey(fsp => fsp.FlashSaleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FlashSaleProduct>()
                .HasOne(fsp => fsp.Product)
                .WithMany()
                .HasForeignKey(fsp => fsp.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Promotion)
                .WithMany()
                .HasForeignKey(o => o.PromotionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Coupon)
                .WithMany()
                .HasForeignKey(o => o.CouponId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
