using Shopp.Web.Models.DTO;
using Shopp.Web.Service.IService;

namespace Shopp.Web.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDTO?> CreateOrder(CartDTO cart)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.StaticData.ApiType.POST,
                Data = cart,
                 Url = $"{Utility.StaticData.OrderAPIBase}/api/order/CreateOrder"
            });
        }

        public async Task<ResponseDTO?> CreatePaymentSession(PaymentServiceRequestDTO paymentRequest)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.StaticData.ApiType.POST,
                Data = paymentRequest,
                Url = $"{Utility.StaticData.OrderAPIBase}/api/order/CreatePaymentSession"
            });
        }

        public async Task<ResponseDTO?> GatAllOrder(string? userId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.StaticData.ApiType.GET,
                Url = $"{Utility.StaticData.OrderAPIBase}/api/order/GetOrders/{userId}"
            });
        }

        public async Task<ResponseDTO?> GetOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.StaticData.ApiType.GET,
                Url = $"{Utility.StaticData.OrderAPIBase}/api/order/GetOrder/{orderId}"
            });
        }

        public async Task<ResponseDTO?> UpdateOrderStatus(int orderId, string newStatus)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.StaticData.ApiType.POST,
                Data = newStatus,
                Url = $"{Utility.StaticData.OrderAPIBase}/api/order/UpdateOrderStatus/{orderId}"
            });
        }

        public async Task<ResponseDTO?> ValidatePaymentSession(int orderHeaderId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.StaticData.ApiType.POST,
                Data = orderHeaderId,
                Url = $"{Utility.StaticData.OrderAPIBase}/api/order/ValidatePaymentSession"
            });
        }
    }
}
