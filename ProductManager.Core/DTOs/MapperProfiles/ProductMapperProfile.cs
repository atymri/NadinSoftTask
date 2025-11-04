using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ProductManager.Core.Domain.Entities;
using ProductManager.Core.DTOs.ProductDTOs;

namespace ProductManager.Core.DTOs.MapperProfiles
{
    public class ProductMapperProfile : Profile
    {
        public ProductMapperProfile()
        {
            CreateMap<Product, ProductResponse>();
            CreateMap<ProductAddRequest, Product>();
            CreateMap<ProductUpdateRequest, Product>();
        }
    }
}
