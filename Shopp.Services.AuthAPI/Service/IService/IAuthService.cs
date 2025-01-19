using Shopp.Services.AuthAPI.Models.DTO;

namespace Shopp.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDTO requestDTO);
        Task<LoginResponseDTO> Login(LoginRequestDTO requestDTO);
        Task<bool> AssignRole(string email, string roleName);
    }
}
