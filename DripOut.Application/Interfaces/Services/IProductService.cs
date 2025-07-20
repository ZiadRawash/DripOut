using DripOut.Application.DTOs;
using DripOut.Application.DTOs.Products;
using DripOut.Application.Helpers;
using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.Services
{
    public interface IProductService
    {
        public Task<EntityPage<Product>> GetAllAsync(QueryModel queryModel);
        public Task<Product> UpdateRateAsync(int productId);
        public Task<Product> CreateProductAsync(ProductInputDTO prdInputDTO);
    }
}
