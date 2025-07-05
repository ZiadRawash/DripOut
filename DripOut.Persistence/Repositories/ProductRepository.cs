using AutoMapper;
using DripOut.Application.DTOs;
using DripOut.Application.DTOs.Products;
using DripOut.Application.Helpers;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Application.Mappers;
using DripOut.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Persistence.Repositories
{
    public class ProductRepository : BaseRepository<Product> , IProductRepository
	{
        
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

		public async Task<EntityPage<Product>> GetAllAsync(QueryModel queryModel)
		{

			var query = _context.Products.Include(p => p.Variants).Include(p => p.Images).AsQueryable();

            //Filtering
            query = query.WhereIf(queryModel.CategoryID != 0, p => p.CategoryId == queryModel.CategoryID)
				.WhereIf(!queryModel.search.IsNullOrEmpty(), p => p.Title.ToLower().Contains(queryModel.search))
				.WhereIf(!queryModel.Size.IsNullOrEmpty(), p => p.Variants!.Any(v => v.Size == queryModel.Size))
                .Where(p => p.Price >= queryModel.MinPrice && p.Price <= queryModel.MaxPrice);
			//Sorting
			if (queryModel.OrderBy.IsNullOrEmpty())
				query = query.OrderByDescending(p => p.Id);
			else if(queryModel.OrderBy == "ASC")
                query = query.OrderBy(p => p.Price);
			else
                query = query.OrderByDescending(p => p.Price);

            var totalCount = await query.CountAsync();


			var products = query
				.Skip((queryModel.Page - 1) * queryModel.PageSize)
				.Take(queryModel.PageSize)
				.ToListAsync();


			// Create the page result
			var entityPage = new EntityPage<Product>
			{
				Items = await products,
				CurrentPage = queryModel.Page,
				PageSize = queryModel.PageSize,
				Count = totalCount,
				TotalPages = (int)Math.Ceiling(totalCount / (double)queryModel.PageSize)
			};

			return entityPage;
		}


	}
}
