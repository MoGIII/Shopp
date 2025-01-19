using Shopp.Services.ProductAPI.Models;
using Shopp.Services.ProductAPI.Models.DTO;

namespace Shopp.Services.ProductAPI.Utils
{
    public class Converter
    {
        internal static ProductDTO ConvertProductToDto(Product product)
        {
            ProductDTO ProductDTO = new ProductDTO()
            {
                CategoryName = product.CategoryName,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Name = product.Name,
                Price = product.Price,
                ProductId = product.ProductId,
            };
            return ProductDTO;
        }
        internal static Product ConvertDtoToProduct(ProductDTO productDTO)
        {
            Product Product = new Product()
            {
                ProductId = productDTO.ProductId,
                CategoryName = productDTO.CategoryName,
                Description = productDTO.Description,
                ImageUrl = productDTO.ImageUrl,
                Name = productDTO.Name,
                Price = productDTO.Price,
            };
            return Product;
        }
    }
}
