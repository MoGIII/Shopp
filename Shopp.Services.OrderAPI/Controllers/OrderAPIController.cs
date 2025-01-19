using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopp.MessageBus;
using Shopp.Services.OrderAPI.Data;
using Shopp.Services.OrderAPI.Models;
using Shopp.Services.OrderAPI.Models.DTO;
using Shopp.Services.OrderAPI.Services.IService;
using Shopp.Services.OrderAPI.Utils;
using Stripe;
using Stripe.Checkout;

namespace Shopp.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDTO _response;
        private IMapper _mapper;
        private readonly OrderDbContext _db;
        private IProductService _productService;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;

        public OrderAPIController(IMapper mapper, OrderDbContext db, IProductService productService, IConfiguration configuration, IMessageBus messageBus )
        {
            _db = db;
            _mapper = mapper;
            _productService = productService;
            _response = new ResponseDTO();
            _configuration = configuration;
            _messageBus = messageBus;
        }


        [Authorize]
        [HttpGet("GetOrders")]
        public ResponseDTO? Get(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> headerList;
                if (User.IsInRole(StaticData.RoleAdmin))
                {
                    headerList = _db.OrderHeaders.Include(u => u.OrderDetails).OrderByDescending(u => u.OrderHeaderId).ToList();

                }
                else
                {
                    headerList = _db.OrderHeaders.Include(u => u.OrderDetails).Where(u => u.UserId == userId).OrderByDescending(u => u.OrderHeaderId).ToList();
                }
                _response.Result = _mapper.Map<IEnumerable<OrderHeaderDTO>>(headerList);
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccessful = false;
            }
            return _response;
        }

        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public ResponseDTO? Get(int id)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.Include(u => u.OrderDetails).First(u => u.OrderHeaderId == id);
                _response.Result =_mapper.Map<OrderHeaderDTO>(orderHeader);
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccessful = false;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDTO> CreateOrder([FromBody] CartDTO cart)
        {
            try
            {
                OrderHeaderDTO orderHeaderDTO = _mapper.Map<OrderHeaderDTO>(cart.CartHeader);
                orderHeaderDTO.OrderTime = DateTime.Now;
                orderHeaderDTO.Status = StaticData.Status_Pending;
                orderHeaderDTO.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDTO>>(cart.CartDetails);

                OrderHeader createdOrder = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDTO)).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDTO.OrderHeaderId = createdOrder.OrderHeaderId;
                _response.Result = orderHeaderDTO;
            }
            catch (Exception ex) 
            {
                _response.Message = ex.Message;
                _response.IsSuccessful = false;
            }

            return _response;
        }

        [Authorize]
        [HttpPost("CreatePaymentSession")]
        public async Task<ResponseDTO> CreatePaymentSession([FromBody] PaymentServiceRequestDTO paymentRequest)
        {
            try
            {

                var options = new SessionCreateOptions
                {
                    SuccessUrl = paymentRequest.ApprovedUrl,
                    CancelUrl = paymentRequest.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                var discounts = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon = paymentRequest.OrderHeader.CouponCode
                    }
                };

                foreach(var item in paymentRequest.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price*100),
                            Currency = "USD",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                if (paymentRequest.OrderHeader.Discount > 0)
                {
                    options.Discounts = discounts;
                }
                var service = new SessionService();
                Session session = service.Create(options);

                paymentRequest.SessionUrl = session.Url;
                OrderHeader orderHeader = _db.OrderHeaders.First(h => h.OrderHeaderId == paymentRequest.OrderHeader.OrderHeaderId);
                orderHeader.SessionId = session.Id;
                _db.SaveChanges();

                _response.Result = paymentRequest;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccessful = false;
            }

            return _response;
        }

        [Authorize]
        [HttpPost("ValidatePaymentSession")]
        public async Task<ResponseDTO> ValidatePaymentSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.First(h => h.OrderHeaderId == orderHeaderId);

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if(paymentIntent.Status == "succeeded")
                {
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = StaticData.Status_Approved;
                    _db.SaveChanges();

                    RewardDTO reward = new RewardDTO()
                    {
                        OrderId = orderHeader.OrderHeaderId,
                        RewardActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        UserId = orderHeader.UserId
                    };

                    string topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    //await _messageBus.PublishMessage(reward, topicName);   


                    _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
                }
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccessful = false;
            }

            return _response;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDTO> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == orderId);
                if (orderHeader != null)
                {
                    if (newStatus == StaticData.Status_Cancelled)
                    {
                        //give refund
                        var options = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderHeader.PaymentIntentId

                        };

                        var service = new RefundService();
                        Refund refund = service.Create(options);  
                    }
                    orderHeader.Status = newStatus;
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccessful = false;
            }
            return _response;
        }
    }
}
