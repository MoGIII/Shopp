using Shopp.Web.Models.DTO;

namespace Shopp.Web.Service.IService
{
    public interface IBaseService
    {
        Task<ResponseDTO?> SendAsync(RequestDTO requestDto, bool withBearer = true);

    }
}
