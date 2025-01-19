

using Shopp.Services.RewardAPI.Message;

namespace Shopp.Services.RewardAPI.Services.IServices
{
    public interface IRewardService
    {
        Task UpdateRewards(RewardMessage message);
    }
}
