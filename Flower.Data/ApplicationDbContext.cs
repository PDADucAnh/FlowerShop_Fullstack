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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

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

            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Product)
                .WithMany()
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
