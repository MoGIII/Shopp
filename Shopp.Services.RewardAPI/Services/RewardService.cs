using Microsoft.EntityFrameworkCore;
using Shopp.Services.RewardAPI.Data;
using Shopp.Services.RewardAPI.Message;
using Shopp.Services.RewardAPI.Models;
using Shopp.Services.RewardAPI.Services.IServices;
using System.Text;

namespace Shopp.Services.RewardAPI.Services
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<RewardDbContext> _dbOptions;

        public RewardService(DbContextOptions<RewardDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }
        public async Task UpdateRewards(RewardMessage message)
        {
            try
            {
                Reward reward = new Reward()
                {
                    OrderId = message.OrderId,
                    RewardActivity = message.RewardActivity,
                    UserId = message.UserId,
                    RewardDate = DateTime.Now
                };

                await using var db = new RewardDbContext(_dbOptions);
                await db.Rewards.AddAsync(reward);
                await db.SaveChangesAsync();
            }catch (Exception ex)
            {
                throw;
            }
        }
    }
}
