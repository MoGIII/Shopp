using Shopp.Services.CouponAPI.Models;
using Shopp.Services.CouponAPI.Models.DTO;

namespace Shopp.Services.CouponAPI.Utils
{
    public class Converter
    {
        internal static CouponDTO ConvertCouponToDto(Coupon coupon)
        {
            CouponDTO CouponDTO = new CouponDTO()
            {
                CouponId = coupon.CouponId,
                CouponCode = coupon.CouponCode,
                DiscountAmount = coupon.DiscountAmount,
                MinAmount = coupon.MinAmount
            };
            return CouponDTO;
        }
        internal static Coupon ConvertDtoToCoupon(CouponDTO couponDTO)
        {
            Coupon Coupon = new Coupon()
            {
                CouponId = couponDTO.CouponId,
                CouponCode = couponDTO.CouponCode,
                MinAmount = couponDTO.MinAmount,
                DiscountAmount = couponDTO.DiscountAmount
            };
            return Coupon;
        }
    }
}
