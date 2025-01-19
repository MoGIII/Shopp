using AutoMapper;
using Shopp.Services.ShoppingCartAPI.Models;
using Shopp.Services.ShoppingCartAPI.Models.DTO;

namespace Shopp.Services.ShoppingCartAPI.Utils
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config => 
            {
                config.CreateMap<CartHeader, CartHeaderDTO>().ReverseMap();
                config.CreateMap<CartDetails, CartDetailsDTO>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
