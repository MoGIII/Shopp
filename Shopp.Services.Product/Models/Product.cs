using System.ComponentModel.DataAnnotations;

namespace Shopp.Services.ProductAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string ImageURL { get; set; }
        [Range(1, 10000)]
        public double Price { get; set; }
    }
}
