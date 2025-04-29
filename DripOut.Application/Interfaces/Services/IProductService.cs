using DripOut.Application.DTOs;
using DripOut.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.Services
{
    public interface IProductService
    {
        public Task<EntityPage<ProductsDTO>> GetAllAsync(string search, int categoryID , int crntPage , int pageSize);

        public Task<Product> FindAsync(int id);
        public Task<Product> FindAsync(Expression<Func<Product, bool>> expression,
          params Expression<Func<Product, object>>[] includes);

        public Task<Product> AddAsync(Product product);


    }
}
