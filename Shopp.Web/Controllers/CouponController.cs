using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopp.Web.Models.DTO;
using Shopp.Web.Service.IService;

namespace Shopp.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDTO>? coupons = new List<CouponDTO>();
            ResponseDTO? response = await _couponService.GetAllCouponsAsync();
            if (response != null && response.IsSuccessful) 
            {
                //TempData["success"] = "All Couopons retrived!";
                coupons = JsonConvert.DeserializeObject<List<CouponDTO>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(coupons);
        }
        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDTO newCoupon)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _couponService.CreateCouponAsync(newCoupon);
                if (response != null && response.IsSuccessful)
                {
                    TempData["success"] = "Coupon created successfully!";
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(newCoupon);
        }
        public async Task<IActionResult> CouponDelete(int id)
        {
            ResponseDTO? response = await _couponService.GetCouponByIdAsync(id);
            if (response != null && response.IsSuccessful)
            {
                CouponDTO? coupon = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));
                
                return View(coupon);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDTO coupon)
        {
            ResponseDTO? response = await _couponService.DeleteCouponAsync(coupon.CouponId);
            if (response != null && response.IsSuccessful)
            {
                TempData["success"] = "Coupon deleted successfully!";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(coupon);
        }
    }
}
