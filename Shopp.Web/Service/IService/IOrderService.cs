using Shopp.Web.Models.DTO;

namespace Shopp.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDTO?> CreateOrder(CartDTO cart);
        Task<ResponseDTO?> CreatePaymentSession(PaymentServiceRequestDTO paymentRequest);
        Task<ResponseDTO?> ValidatePaymentSession(int orderHeaderId);
        Task<ResponseDTO?> GatAllOrder(string? userId);
        Task<ResponseDTO?> GetOrder(int orderId);
        Task<ResponseDTO?> UpdateOrderStatus(int orderId, string newStatus);
    }
}
