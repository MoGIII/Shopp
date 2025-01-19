using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopp.Web.Models;
using Shopp.Web.Models.DTO;
using Shopp.Web.Service.IService;
using System.Diagnostics;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Shopp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDTO>? products = new List<ProductDTO>();
            ResponseDTO? response = await _productService.GetAllProductsAsync();
            if (response != null && response.IsSuccessful)
            {
                //TempData["success"] = "All Couopons retrived!";
                products = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(products);
        }
        [Authorize]
        public async Task<IActionResult> ProductDetails(int id)
        {
            ProductDTO? product = new ProductDTO();
            ResponseDTO? response = await _productService.GetProductByIdAsync(id);
            if (response != null && response.IsSuccessful)
            {
                //TempData["success"] = "All Couopons retrived!";
                product = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(product);
        }

        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDTO product)
        {
            CartDTO cartDTO = new CartDTO()
            {
                CartHeader = new CartHeaderDTO()
                {
                    UserId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value
                }
            };
            CartDetailsDTO cartDetailsDTO = new CartDetailsDTO()
            {
                Count = product.Count,
                ProductId = product.ProductId
            };

            List<CartDetailsDTO> cartDetailsList = new List<CartDetailsDTO>() {cartDetailsDTO};

            cartDTO.CartDetails = cartDetailsList;

            ResponseDTO? response = await _cartService.UpsertCartAsync(cartDTO);
            
            if (response != null && response.IsSuccessful)
            {
                TempData["success"] = "Item added to the shopping cart!";
                return RedirectToAction(nameof(Index));
                
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
