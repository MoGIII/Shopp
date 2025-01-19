using Shopp.Web.Models.DTO;
using Shopp.Web.Service.IService;
using Shopp.Web.Utility;
using static Shopp.Web.Utility.StaticData;

namespace Shopp.Web.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDTO?> CreateProductAsync(ProductDTO productDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = productDTO,
                Url = $"{StaticData.ProductAPIBase}/api/product",
                ContentType = StaticData.ContentType.MultipartFormData

            });
        }

        public async Task<ResponseDTO?> DeleteProductAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.DELETE,
                Url = $"{StaticData.ProductAPIBase}/api/product/{id}"

            });
        }

        public async Task<ResponseDTO?> GetAllProductsAsync()
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = $"{StaticData.ProductAPIBase}/api/product"

            });
        }

        public async Task<ResponseDTO?> GetProductByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = $"{StaticData.ProductAPIBase}/api/product/{id}"

            });
        }

        public async Task<ResponseDTO?> UpdateProductAsync(ProductDTO productDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.PUT,
                Data = productDTO,
                Url = $"{StaticData.ProductAPIBase}/api/product",
                ContentType = StaticData.ContentType.MultipartFormData

            });
        }
    }
}
