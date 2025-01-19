using Newtonsoft.Json;
using Shopp.Services.OrderAPI.Models.DTO;
using Shopp.Services.OrderAPI.Services.IService;

namespace Shopp.Services.OrderAPI.Services
{
    public class ProductService: IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/product");
            var apiContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
            if (result.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(Convert.ToString(result.Result));
            }
            return new List<ProductDTO>();
        }
    }
}
