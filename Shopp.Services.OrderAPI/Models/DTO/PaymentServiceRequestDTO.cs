namespace Shopp.Services.OrderAPI.Models.DTO
{
    public class PaymentServiceRequestDTO
    {
        public string? SessionId { get; set; }
        public string? SessionUrl { get; set; }
        public string ApprovedUrl { get; set; }
        public string CancelUrl { get; set; }

        public OrderHeaderDTO OrderHeader { get; set; }
    }
}
