using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopp.Services.ProductAPI.Data;
using Shopp.Services.ProductAPI.Models;
using Shopp.Services.ProductAPI.Models.DTO;
using Shopp.Services.ProductAPI.Utils;

namespace Shopp.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly ProductDbContext _db;
        private ResponseDTO _response;
        public ProductAPIController(ProductDbContext db, IMapper mapper)
        {
            _db = db;
            _response = new ResponseDTO();
        }
        [HttpGet]
        public ResponseDTO Get() 
        {
            IEnumerable<Product> products = new List<Product>();
            try
            {
                products = _db.Products.ToList();
                _response.Result = products;
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
            Product product = new Product();
            try
            {
                product = _db.Products.First(p=> p.ProductId==id);
                ProductDTO productDto = Converter.ConvertProductToDto(product);
                _response.Result = productDto;
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
        public ResponseDTO Post([FromBody] ProductDTO productDto)
        {
            Product product;
            try
            {
                product = Converter.ConvertDtoToProduct(productDto);
                _db.Products.Add(product);
                _db.SaveChanges();

                _response.Result = product;
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
        public ResponseDTO Put([FromBody] ProductDTO ProductDto)
        {
            Product product;
            try
            {
                product = Converter.ConvertDtoToProduct(ProductDto);
                _db.Products.Update(product);
                _db.SaveChanges();

                _response.Result = product;
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
            Product product;
            try
            {
                product = _db.Products.First(p => p.ProductId == id);
                _db.Products.Remove(product);
                _db.SaveChanges();
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
