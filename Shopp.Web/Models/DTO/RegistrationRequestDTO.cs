using System.ComponentModel.DataAnnotations;

namespace Shopp.Web.Models.DTO
{
    public class RegistrationRequestDTO
    {
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string? Role { get; set; }
    }
}
