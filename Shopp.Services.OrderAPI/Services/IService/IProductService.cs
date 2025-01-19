using Shopp.Services.OrderAPI.Models.DTO;

namespace Shopp.Services.OrderAPI.Services.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProducts();
    }
}
