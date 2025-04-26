using DripOut.Application.DTOs;
using DripOut.Application.Interfaces.Services;
using DripOut.Domain.Consts;
using DripOut.Domain.Models.Entities;
using DripOut.Persistence;
using DripOut.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Infrastructure.Implementaion
{
    public class ProductSrevice : IProductService
    {
        public BaseRepository<Product> _repo;
        public ProductSrevice(BaseRepository<Product> repo)
        {
            _repo = repo;
        }
        public async Task<EntityPage<Product>> GetAllAsync(string search = "", int crntPage = 1, int pageSize = 10)
        {
            
            var Products = await _repo.dbSet.ToListAsync();
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                Products = Products.Where(p => p.Title.ToLower().StartsWith(search)).ToList();
            }
            var EntityPage = new EntityPage<Product>
            {
                List = Products.Skip((crntPage - 1) * pageSize).Take(pageSize).AsQueryable().Include("Category"),
                CurrentPage = crntPage,
                PageSize = pageSize,
                TotalSize = Products.Count(),
                TotalPages = (int)Math.Ceiling(Products.Count() / (double)pageSize)
            };
            return EntityPage;
        }
    }
}
