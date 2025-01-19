using Microsoft.EntityFrameworkCore;
using Shopp.Services.ShoppingCartAPI.Models;

namespace Shopp.Services.ShoppingCartAPI.Data
{
    public class ShoppingCartDbContext: DbContext
    {
        public ShoppingCartDbContext(DbContextOptions<ShoppingCartDbContext> options): base(options)
        {
            
        }

        public DbSet<CartHeader> CartHeader {  get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }

    }
}
