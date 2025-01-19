using Newtonsoft.Json;
using Shopp.Web.Models.DTO;
using Shopp.Web.Service.IService;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Text;
using static Shopp.Web.Utility.StaticData;
using static System.Net.Mime.MediaTypeNames;

namespace Shopp.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDTO?> SendAsync(RequestDTO requestDto, bool withBearer = true)
        {
            try
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("ShoppAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", Application.Json);
                //if (requestDto.ContentType == ContentType.MultipartFormData)
                //{
                //    message.Headers.Add("Accept", "*/*");
                //}
                //else
                //{
                //    message.Headers.Add("Accept", Application.Json);
                //}
                // Add token
                if (withBearer) 
                { 
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization",$"Bearer {token}");
                }
                message.RequestUri = new Uri(requestDto.Url);
                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, Application.Json);

                }
                //if (requestDto.ContentType == ContentType.MultipartFormData)
                //{
                //    var content = new MultipartFormDataContent();
                //    foreach (var prop in requestDto.Data.GetType().GetProperties())
                //    {
                //        var value = prop.GetValue(requestDto.Data);
                //        if (value is FormFile)
                //        {
                //            var file = (FormFile)value;
                //            if (file != null)
                //            {
                //                content.Add(new StreamContent(file.OpenReadStream()), prop.Name,file.FileName);
                //            }
                //        }
                //    }
                    
                //}
                //else
                //{
                //    if (requestDto.Data != null)
                //    {
                //        message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, Application.Json);

                //    }
                //}
                

                switch (requestDto.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                HttpResponseMessage? apiResponse = await httpClient.SendAsync(message);

                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new ResponseDTO() { IsSuccessful = false, Message = "Not Found!" };
                    case HttpStatusCode.Forbidden:
                        return new ResponseDTO() { IsSuccessful = false, Message = "Forbidden!" };
                    case HttpStatusCode.Unauthorized:
                        return new ResponseDTO() { IsSuccessful = false, Message = "Unauthorized to access!" };
                    case HttpStatusCode.InternalServerError:
                        return new ResponseDTO() { IsSuccessful = false, Message = "Internal Server Error!" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                var dto = new ResponseDTO()
                {
                    IsSuccessful = false,
                    Message = ex.Message
                };
                return dto;
            }

        }
    }
}
