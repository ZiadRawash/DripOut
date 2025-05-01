using AutoMapper;
using DripOut.Application.DTOs;
using DripOut.Application.Interfaces;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Domain.Consts;
using DripOut.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Persistence.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        
        private readonly ApplicationDbContext _context;
        public ProductRepository( ApplicationDbContext context) :base(context)
        {

			_context = context;

		}

		public async Task<EntityPage<Product>> GetAllAsync(string search = "", int categoryId = 0, int crntPage = 1, int pageSize = 10)
		{
			
			var query = _context.Products.AsQueryable();

		
			if (categoryId > 0)
			{
				query = query.Where(p => p.CategoryId == categoryId);
			}

			
			if (!string.IsNullOrEmpty(search))
			{
				var searchLower = search.ToLower();
				query = query.Where(p => p.Title.ToLower().StartsWith(searchLower));
			}

			
			query = query
				.Include(p => p.Category)
				.Include(p => p.Reviews)!
				.ThenInclude(u => u.User);

			
			var totalCount = await query.CountAsync();

			
			var products = await query
				.OrderBy(p => p.Id) 
				.Skip((crntPage - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		

			// Create the page result
			var entityPage = new EntityPage<Product>
			{
				List = products,
				CurrentPage = crntPage,
				PageSize = pageSize,
				TotalSize = totalCount,
				TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
			};

			return entityPage;
		}

		
	}
}
