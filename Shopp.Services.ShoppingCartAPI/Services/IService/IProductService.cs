using Shopp.Services.ShoppingCartAPI.Models.DTO;

namespace Shopp.Services.ShoppingCartAPI.Services.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProducts();
    }
}
