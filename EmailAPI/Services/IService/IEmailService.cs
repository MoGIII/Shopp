using Shopp.Services.EmailAPI.Message;
using Shopp.Services.EmailAPI.Models.DTO;

namespace Shopp.Services.EmailAPI.Services.IService
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDTO cart);
        Task RegisterUserEmailAndLog(string email);
        Task LogOrderPlaced(RewardMessage reward);
    }
}
