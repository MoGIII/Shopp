using Shopp.Web.Models.DTO;

namespace Shopp.Web.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDTO?> GetCartByUserIdAsync(string userID);
        Task<ResponseDTO?> UpsertCartAsync(CartDTO cart);
        Task<ResponseDTO?> RemoveFromCartAsync(int cartDetailsId);
        Task<ResponseDTO?> ApplyCouponAsync(CartDTO cart);
        Task<ResponseDTO?> EmailCartAsync(CartDTO cart);
    }
}
