using Newtonsoft.Json;
using Shopp.Services.ShoppingCartAPI.Models.DTO;
using Shopp.Services.ShoppingCartAPI.Services.IService;

namespace Shopp.Services.ShoppingCartAPI.Services
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<CouponDTO> GetCoupon(string couponCode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/coupon/GetByCode/{couponCode}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
            if (result.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(result.Result));
            }
            return new CouponDTO();
        }
    }
}
