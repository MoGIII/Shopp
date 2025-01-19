using Microsoft.EntityFrameworkCore;
using Shopp.Services.RewardAPI.Models;

namespace Shopp.Services.RewardAPI.Data
{
    public class RewardDbContext: DbContext
    {
        public RewardDbContext(DbContextOptions<RewardDbContext> options) : base(options)
        {

        }

        public DbSet<Reward> Rewards { get; set; }
        
    }
}
