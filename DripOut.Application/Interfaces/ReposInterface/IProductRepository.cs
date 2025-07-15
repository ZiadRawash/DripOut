using DripOut.Application.DTOs;
using DripOut.Application.DTOs.Products;
using DripOut.Application.Helpers;
using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.ReposInterface
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        public Task<EntityPage<Product>> GetAllAsync(QueryModel queryModel);
        public Task<Product> UpdateRateAsync(int productId);
    }
}
