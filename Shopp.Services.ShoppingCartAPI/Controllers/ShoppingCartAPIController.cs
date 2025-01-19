using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopp.MessageBus;
using Shopp.Services.ShoppingCartAPI.Data;
using Shopp.Services.ShoppingCartAPI.Models;
using Shopp.Services.ShoppingCartAPI.Models.DTO;
using Shopp.Services.ShoppingCartAPI.Services.IService;
using System.Reflection.PortableExecutable;

namespace Shopp.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class ShoppingCartAPIController : ControllerBase
    {
        private IMapper _mapper;
        private ResponseDTO _response;
        private readonly ShoppingCartDbContext _db;
        private IProductService _productService;
        private ICouponService _couponService;
        private IMessageBus _messageBus;
        private IConfiguration _configuration;

        public ShoppingCartAPIController(IMapper mapper, ShoppingCartDbContext db, IProductService productService, ICouponService couponService, IMessageBus messageBus, IConfiguration configuration)
        {
            _mapper = mapper;
            _db = db;
            _response = new ResponseDTO();
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDTO> GetCart(string userId)
        {
            try
            {
                CartDTO cart = new CartDTO()
                {
                    CartHeader = _mapper.Map<CartHeaderDTO>(_db.CartHeader.First(c=>c.UserId==userId))
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDTO>>(_db.CartDetails
                    .Where(c => c.CartHeaderId == cart.CartHeader.CartHeaderId));

                IEnumerable<ProductDTO> products = await _productService.GetProducts(); 
                foreach (var item in cart.CartDetails) 
                {
                    item.Product = products.FirstOrDefault(p => p.ProductId==item.ProductId);
                    cart.CartHeader.CartTotal += (item.Count*item.Product.Price);
                }

                //apply coupon discount logic - if any
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDTO coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                    if (coupon != null && cart.CartHeader.CartTotal >= coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }

                _response.Result = cart;
            }
            catch (Exception ex) 
            { 
                _response.Message = ex.Message;
                _response.IsSuccessful = false;
            }

            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDTO cart)
        {
            try
            {
                var cartFromDb = await _db.CartHeader.FirstAsync(c => c.UserId == cart.CartHeader.UserId);
                cartFromDb.CouponCode = cart.CartHeader.CouponCode;
                _db.CartHeader.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccessful = false;
            }
            return _response;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] CartDTO cart)
        {
            try
            {
                var cartFromDb = await _db.CartHeader.FirstAsync(c => c.UserId == cart.CartHeader.UserId);
                cartFromDb.CouponCode = "";
                _db.CartHeader.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccessful = false;
            }
            return _response;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDTO> CartUpsert(CartDTO cart)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeader.AsNoTracking().FirstOrDefaultAsync(user=>user.UserId==cart.CartHeader.UserId);
                if (cartHeaderFromDb == null) 
                {
                    //create cart header
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cart.CartHeader);

                    _db.CartHeader.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    //Create cart details
                    cart.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cart.CartDetails.First()));
                    await _db.SaveChangesAsync();

                }
                else
                {
                    //check if cart details has same product
                    var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        user=>user.ProductId==cart.CartDetails.First().ProductId &&
                        user.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        //create new cartDetails
                        cart.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cart.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        //update count in cartDetails
                        cart.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cart.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cart.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cart.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                }
                _response.Result = cart;
            }
            catch (Exception ex) 
            { 
                _response.Message = ex.Message;
                _response.IsSuccessful = false;
            }

            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDTO> RemoveCart([FromBody]int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _db.CartDetails.First(d => d.CartDetailsId == cartDetailsId);
                int totalCountOfCartItems = _db.CartDetails.Where(d=>d.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);
                if (totalCountOfCartItems == 1) 
                {
                    var cartHeaderToRemove = await _db.CartHeader.FirstOrDefaultAsync(h => h.CartHeaderId == cartDetails.CartHeaderId);
                    _db.CartHeader.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccessful = false;
            }

            return _response;
        }

        [HttpPost("EmailCartRequest")]
        public async Task<object> EmailCartRequest([FromBody] CartDTO cart)
        {
            try
            {
                await _messageBus.PublishMessage(cart, _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
                _response.Result = true;
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
