using Shopp.Web.Models.DTO;

namespace Shopp.Web.Service.IService
{
    public interface IProductService
    {
        Task<ResponseDTO?> GetAllProductsAsync();
        Task<ResponseDTO?> GetProductByIdAsync(int id);
        Task<ResponseDTO?> CreateProductAsync(ProductDTO productDTO);
        Task<ResponseDTO?> UpdateProductAsync(ProductDTO productDTO);
        Task<ResponseDTO?> DeleteProductAsync(int id);
    }
}
