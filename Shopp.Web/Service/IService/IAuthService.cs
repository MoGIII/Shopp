using Shopp.Web.Models.DTO;

namespace Shopp.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDTO?> LoginAsync(LoginRequestDTO request);
        Task<ResponseDTO?> RegisterAsync(RegistrationRequestDTO request);
        Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO request);
    }
}
