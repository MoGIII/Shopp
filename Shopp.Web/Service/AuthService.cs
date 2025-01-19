using Shopp.Web.Models.DTO;
using Shopp.Web.Service.IService;
using Shopp.Web.Utility;
using static Shopp.Web.Utility.StaticData;

namespace Shopp.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO request)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = request,
                Url = $"{StaticData.AuthAPIBase}/api/auth/assignRole"

            }, withBearer: false);
        }

        public async Task<ResponseDTO?> LoginAsync(LoginRequestDTO request)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = request,
                Url = $"{StaticData.AuthAPIBase}/api/auth/login"

            });
        }

        public async Task<ResponseDTO?> RegisterAsync(RegistrationRequestDTO request)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = request,
                Url = $"{StaticData.AuthAPIBase}/api/auth/register"

            });
        }
    }
}
