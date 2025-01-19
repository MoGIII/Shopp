using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopp.Web.Models.DTO;
using Shopp.Web.Service.IService;

namespace Shopp.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> ProductIndex()
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
        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDTO newProduct)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _productService.CreateProductAsync(newProduct);
                if (response != null && response.IsSuccessful)
                {
                    TempData["success"] = "Product created successfully!";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(newProduct);
        }
        public async Task<IActionResult> ProductDelete(int id)
        {
            ResponseDTO? response = await _productService.GetProductByIdAsync(id);
            if (response != null && response.IsSuccessful)
            {
                ProductDTO? product = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));

                return View(product);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDTO product)
        {
            ResponseDTO? response = await _productService.DeleteProductAsync(product.ProductId);
            if (response != null && response.IsSuccessful)
            {
                TempData["success"] = "Product deleted successfully!";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(product);
        }

        public async Task<IActionResult> ProductEdit(int id)
        {
            ResponseDTO? response = await _productService.GetProductByIdAsync(id);
            if (response != null && response.IsSuccessful)
            {
                ProductDTO? product = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));

                return View(product);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDTO product)
        {
            ResponseDTO? response = await _productService.UpdateProductAsync(product);
            if (response != null && response.IsSuccessful)
            {
                TempData["success"] = "Product updated successfully!";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(product);
        }
    }
}
