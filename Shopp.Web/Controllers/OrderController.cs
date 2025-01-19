using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopp.Web.Models.DTO;
using Shopp.Web.Service.IService;
using Shopp.Web.Utility;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace Shopp.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public IActionResult OrderIndex()
        {
            return View();
        }

        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDTO orderHeaderDTO = new OrderHeaderDTO();
            string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            var response = await _orderService.GetOrder(orderId);
            if (response != null && response.IsSuccessful)
            {
                orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
            }
            else
            {
                orderHeaderDTO = new OrderHeaderDTO();
            }
            if (!User.IsInRole(StaticData.Roles.Admin.ToString()) && userId != orderHeaderDTO.UserId)
            {
                return NotFound();
            }
            return View(orderHeaderDTO);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<OrderHeaderDTO> list;
            string userId = "";
            if (!User.IsInRole(StaticData.Roles.Admin.ToString()))
            {
                userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            }
            ResponseDTO response = _orderService.GatAllOrder(userId).GetAwaiter().GetResult();
            if (response != null && response.IsSuccessful)
            {
                list = JsonConvert.DeserializeObject<List<OrderHeaderDTO>>(Convert.ToString(response.Result));
            }
            else
            {
                list = new List<OrderHeaderDTO>();
            }
            return Json(new { data = list });
        }
    }
}
