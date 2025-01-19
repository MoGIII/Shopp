using Microsoft.EntityFrameworkCore;
using Shopp.Services.ProductAPI.Models;

namespace Shopp.Services.ProductAPI.Data
{
    public class ProductDbContext: DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options): base(options)
        {
            
        }

        public DbSet<Product> Coupons {  get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasData(new Product
            {
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                CouponId = 2,
                CouponCode = "20OFF",
                DiscountAmount = 20.0,
                MinAmount = 50,
                ExpirationDate = new DateTime(2088, 12, 31)
            });
        }
    }
}
