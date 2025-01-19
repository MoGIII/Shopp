
using System.ComponentModel.DataAnnotations;

namespace Shopp.Web.Models.DTO
{
    public class ProductDTO
    {    
        public int ProductId { get; set; }        
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
        [Range(0, 100)]
        public int Count { get; set; } = 1;
        public string? ImageLocalPath { get; set; }
        public IFormFile? Image { get; set; }
    }
}
