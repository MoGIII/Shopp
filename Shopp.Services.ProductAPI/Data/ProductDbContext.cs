using Microsoft.EntityFrameworkCore;
using Shopp.Services.ProductAPI.Models;

namespace Shopp.Services.ProductAPI.Data
{
    public class ProductDbContext: DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options): base(options)
        {
            
        }

        public DbSet<Product> Products {  get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 1,
                Name = "Ergonomic Mouse",
                Price = 50.99,
                Description = "An ergonomic mouse for less joint pain",
                ImageUrl = "https://placehold.co/603x403",
                CategoryName = "Mouse"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 2,
                Name = "Gaming Keyboard",
                Price = 133.99,
                Description = "Top tier gaming keyboard",
                ImageUrl = "https://placehold.co/602x402",
                CategoryName = "Keyboard"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 3,
                Name = "Monitor",
                Price = 299.99,
                Description = "Gaming curved monitor X\" and refresh rate of YYY Mhz",
                ImageUrl = "https://placehold.co/601x401",
                CategoryName = "Monitor"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 4,
                Name = "Office Headphones",
                Price = 15,
                Description = "Basic office headphones",
                ImageUrl = "https://placehold.co/600x400",
                CategoryName = "Headphones"
            });
        }
    }
}
