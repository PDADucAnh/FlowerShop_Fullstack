using Flower.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flower.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

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
                .HasFilter("[ResetToken] IS NOT NULL");

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Sku)
                .IsUnique()
                .HasFilter("[Sku] IS NOT NULL");

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
                .HasFilter("[IsDefault] = 1");

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
        }
    }
}
