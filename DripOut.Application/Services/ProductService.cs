using AutoMapper;
using DripOut.Application.DTOs;
using DripOut.Application.Interfaces.Services;
using DripOut.Domain.Consts;
using DripOut.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Services
{
    public class ProductService : IProductService
    {
        public IBaseRepository<Product> _repo;
        private readonly IMapper _mapper;
        public ProductService(IBaseRepository<Product> repo,IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Product> AddAsync(Product product) => await _repo.AddAsync(product);

        public async Task<Product> FindAsync(int id) => await _repo.FindAsync(id)!;

        public async Task<EntityPage<ProductsDTO>> GetAllAsync(string search = "",int categoryId = 0, int crntPage = 1, int pageSize = 10)
        {

            var Products =  _repo.GetAll(p => categoryId == 0 ? p.Id > 0 : p.CategoryId == categoryId)
                .Include(p => p.Category).Include(p => p.Reviews)!.ThenInclude(u => u.User).ToList();
            //Products.AsQueryable().Include(r => r.Reviews)!.ThenInclude(u=>u.User);
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                Products = Products.Where(p => p.Title.ToLower().StartsWith(search)).ToList();
            }
            var list = _mapper.Map<List<ProductsDTO>>(Products);
            var EntityPage = new EntityPage<ProductsDTO>
            {
                List = list.Skip((crntPage - 1) * pageSize).Take(pageSize),
                CurrentPage = crntPage,
                PageSize = pageSize,
                TotalSize = Products.Count(),
                TotalPages = (int)Math.Ceiling(Products.Count() / (double)pageSize)
            };
            return EntityPage;
        }

        public async Task<Product> FindAsync(Expression<Func<Product, bool>> expression
            , params Expression<Func<Product, object>>[] includes)
        => await _repo.FindAsync(expression, includes)!;
    }
}
