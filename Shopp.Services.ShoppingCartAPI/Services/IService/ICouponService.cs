using Shopp.Services.ShoppingCartAPI.Models.DTO;

namespace Shopp.Services.ShoppingCartAPI.Services.IService
{
    public interface ICouponService
    {
        Task<CouponDTO> GetCoupon(string couponCode);
    }
}
