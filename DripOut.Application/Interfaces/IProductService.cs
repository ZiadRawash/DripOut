using DripOut.Application.DTOs;
using DripOut.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces
{
    public interface IProductService : IBaseRepository<Product>
    {
        public Task<EntityPage<ProductsDTO>> GetAllAsync(string search, int categoryID , int crntPage , int pageSize);

    }
}
