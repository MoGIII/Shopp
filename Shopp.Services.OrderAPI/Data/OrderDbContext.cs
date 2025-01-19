using Microsoft.EntityFrameworkCore;
using Shopp.Services.OrderAPI.Models;

namespace Shopp.Services.OrderAPI.Data
{
    public class OrderDbContext: DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {

        }

        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
    }
}
