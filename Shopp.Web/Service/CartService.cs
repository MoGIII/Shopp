using Shopp.Web.Models.DTO;
using Shopp.Web.Service.IService;
using Shopp.Web.Utility;
using static Shopp.Web.Utility.StaticData;

namespace Shopp.Web.Service
{
    public class CartService : ICartService
    {
        private readonly IBaseService _baseService;
        public CartService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDTO?> ApplyCouponAsync(CartDTO cart)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cart,
                Url = $"{StaticData.CartAPIBase}/api/cart/ApplyCoupon"

            });
        }

        public async Task<ResponseDTO?> GetCartByUserIdAsync(string userID)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = $"{StaticData.CartAPIBase}/api/cart/GetCart/{userID}"

            });
        }

        public async Task<ResponseDTO?> RemoveFromCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDetailsId,
                Url = $"{StaticData.CartAPIBase}/api/cart/RemoveCart"

            });
        }

        public async Task<ResponseDTO?> UpsertCartAsync(CartDTO cart)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cart,
                Url = $"{StaticData.CartAPIBase}/api/cart/CartUpsert"

            });
        }
        public async Task<ResponseDTO?> EmailCartAsync(CartDTO cart)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cart,
                Url = $"{StaticData.CartAPIBase}/api/cart/EmailCartRequest"

            });
        }
    }
}
