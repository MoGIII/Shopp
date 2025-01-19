using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopp.Web.Models.DTO;
using Shopp.Web.Service.IService;
using Shopp.Web.Utility;
using System.IdentityModel.Tokens.Jwt;

namespace Shopp.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {

            return View(await LoadCartBasedOnLoggedUser());
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {

            return View(await LoadCartBasedOnLoggedUser());
        }
        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null && response.IsSuccessful)
            {
                TempData["success"] = "Item successfully removed!";
               return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDTO cart)
        {
            

            ResponseDTO? response = await _cartService.ApplyCouponAsync(cart);
            if (response != null && response.IsSuccessful)
            {
                TempData["success"] = "Coupon successfully added!";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDTO cart)
        {

            cart.CartHeader.CouponCode = "";
            ResponseDTO? response = await _cartService.ApplyCouponAsync(cart);
            if (response != null && response.IsSuccessful)
            {
                TempData["success"] = "Coupon successfully removed!";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        private async Task<CartDTO> LoadCartBasedOnLoggedUser()
        {
            var userId = User.Claims.Where(u => u.Type==JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _cartService.GetCartByUserIdAsync(userId);
            if (response != null && response.IsSuccessful)
            {
                CartDTO cart = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));
                return cart;
            }
            return new CartDTO();
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDTO cartDto)
        {

            CartDTO cart = await LoadCartBasedOnLoggedUser();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _cartService.EmailCartAsync(cart);
            if (response != null && response.IsSuccessful)
            {
                TempData["success"] = "Email will be processed and sent shortly";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDTO cartDto)
        {
            CartDTO cart = await LoadCartBasedOnLoggedUser();
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.Email = cartDto.CartHeader.Email;
            cart.CartHeader.Name = cartDto.CartHeader.Name;

            var response = await _orderService.CreateOrder(cart);
            OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
            if (response != null && response.IsSuccessful)
            {
                //get payment session and redirect to place order
                var domain = $"{Request.Scheme}://{Request.Host.Value}/";
                PaymentServiceRequestDTO paymentRequest = new PaymentServiceRequestDTO()
                {
                    ApprovedUrl = $"{domain}cart/Confirmation?orderId={orderHeaderDTO.OrderHeaderId}",
                    CancelUrl = $"{domain}cart/Checkout",
                    OrderHeader = orderHeaderDTO

                };

                var paymentResponse = await _orderService.CreatePaymentSession(paymentRequest);

                PaymentServiceRequestDTO result = JsonConvert.DeserializeObject<PaymentServiceRequestDTO>(Convert.ToString(paymentResponse.Result));

                Response.Headers.Add("Location", result.SessionUrl);

                return new StatusCodeResult(303);

            }
            return View();
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseDTO? response = await _orderService.ValidatePaymentSession(orderId);
            if (response != null && response.IsSuccessful)
            {
                OrderHeaderDTO orderHeader = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
                if (orderHeader.Status==StaticData.Status_Approved)
                {
                    return View(orderId);
                }
            }
            //redirect to error page based on status
            return View(orderId);
        }
    }
}
