using DripOut.Application.DTOs;
using DripOut.Application.DTOs.Products;
using DripOut.Application.Helpers;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Application.Mappers;
using DripOut.Domain.Models;
using DripOut.Domain.Consts;
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

			var query = _context.Products
				.Include(p => p.Variants)
				.Include(p => p.Reviews)
				.Include(p => p.Images).AsQueryable();

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


			var products = await query
				.Skip((queryModel.Page - 1) * queryModel.PageSize)
				.Take(queryModel.PageSize)
				.ToListAsync();

            var allProducts = await query.Include(p => p.Variants).ToListAsync();

			// Get all sizes if the category is specified
            var variants =  allProducts.SelectMany(p => p.Variants!).ToList();
            var sizes = variants.Select(v => v.Size).Distinct().ToList();

                // Create the page result
            var entityPage = new EntityPage<Product>
			{
				Items = products,
				CurrentPage = queryModel.Page,
				PageSize = queryModel.PageSize,
				Count = totalCount,
				TotalPages = (int)Math.Ceiling(totalCount / (double)queryModel.PageSize),
				MinPrice = allProducts.Any() ? allProducts.Min(p => p.Price) : 0,
				MaxPrice = allProducts.Any() ? allProducts.Max(p => p.Price) : 0,
				Sizes = sizes
            };

			return entityPage;
		}

        public async Task<Product> UpdateRateAsync(int productId)
        {
            var product = await FindAsync(p => p.Id == productId,
                p => p.Reviews!);
            if (product!.Reviews is null || !product.Reviews.Any())
                return null!;

            product.Rate = (float)product.Reviews.Average(r => r.Stars);
            await UpdateAsync(product);
            return product;
        }
    }
}
