using Shopp.Services.AuthAPI.Models;

namespace Shopp.Services.AuthAPI.Service.IService
{
    public interface IJWTTokenGenerator
    {
        string GenerateToken(ApplicationUser user, IEnumerable<string> roles);
    }
}
