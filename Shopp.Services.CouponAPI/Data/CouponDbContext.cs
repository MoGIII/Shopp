using Microsoft.EntityFrameworkCore;
using Shopp.Services.CouponAPI.Models;

namespace Shopp.Services.CouponAPI.Data
{
    public class CouponDbContext: DbContext
    {
        public CouponDbContext(DbContextOptions<CouponDbContext> options): base(options)
        {
            
        }

        public DbSet<Coupon> Coupons {  get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = 1,
                CouponCode = "10OFF",
                DiscountAmount = 10.0,
                MinAmount = 20,
                ExpirationDate = new DateTime(2099,12,31)
            });
            modelBuilder.Entity<Coupon>().HasData(new Coupon
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
