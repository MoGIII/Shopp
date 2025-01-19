using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopp.Services.CouponAPI.Data;
using Shopp.Services.CouponAPI.Models;
using Shopp.Services.CouponAPI.Models.DTO;
using Shopp.Services.CouponAPI.Utils;

namespace Shopp.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly CouponDbContext _db;
        private ResponseDTO _response;
        private readonly IConfiguration _configuration;
        public CouponAPIController(CouponDbContext db, IConfiguration configuration)
        {
            _db = db;
            _response = new ResponseDTO();
            _configuration = configuration;
        }
        [HttpGet]
        public ResponseDTO Get() 
        {
            IEnumerable<Coupon> coupons = new List<Coupon>();
            try
            {
                coupons = _db.Coupons.ToList();
                _response.Result = coupons;
            }
            catch (Exception ex)
            {
                _response.IsSuccessful = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDTO Get(int id)
        {
            Coupon coupon = new Coupon();
            try
            {
                coupon = _db.Coupons.First(c=> c.CouponId==id);
                CouponDTO couponDto = Converter.ConvertCouponToDto(coupon);
                _response.Result = couponDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccessful = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDTO Get(string code)
        {
            Coupon coupon = new Coupon();
            try
            {
                coupon = _db.Coupons.First(c => c.CouponCode.ToLower() == code.ToLower());
                CouponDTO couponDto = Converter.ConvertCouponToDto(coupon);
                _response.Result = couponDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccessful = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles ="ADMIN")]
        public ResponseDTO Post([FromBody] CouponDTO couponDto)
        {
            Coupon coupon;
            try
            {
                coupon = Converter.ConvertDtoToCoupon(couponDto);
                _db.Coupons.Add(coupon);
                _db.SaveChanges();

                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)(couponDto.DiscountAmount * 100),
                    Name = couponDto.CouponCode,
                    Currency = "USD",
                    Id = couponDto.CouponCode
                };
                var service = new Stripe.CouponService();
                service.Create(options);

                _response.Result = coupon;
            }
            catch (Exception ex)
            {
                _response.IsSuccessful = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDTO Put([FromBody] CouponDTO couponDto)
        {
            Coupon coupon;
            try
            {
                coupon = Converter.ConvertDtoToCoupon(couponDto);
                _db.Coupons.Update(coupon);
                _db.SaveChanges();

                _response.Result = coupon;
            }
            catch (Exception ex)
            {
                _response.IsSuccessful = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDTO Delete(int id)
        {
            Coupon coupon;
            try
            {
                coupon = _db.Coupons.First(c => c.CouponId == id);
                _db.Coupons.Remove(coupon);
                _db.SaveChanges();

                var service = new Stripe.CouponService();
                service.Delete(coupon.CouponCode);
            }
            catch (Exception ex)
            {
                _response.IsSuccessful = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
