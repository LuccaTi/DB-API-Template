using DBAPITemplate.Application.DTOs.Product;
using DBAPITemplate.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAPITemplate.Application.Mappers
{
    public class ProductMapperRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ProductRequestDto, Product>();
            config.NewConfig<Product, ProductResponseDto>();
        }
    }
}
