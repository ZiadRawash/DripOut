using DripOut.Application.DTOs;
using DripOut.Application.DTOs.Image;
using DripOut.Application.DTOs.Products;
using DripOut.Application.Helpers;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Application.Interfaces.Services;
using DripOut.Application.Mappers;
using DripOut.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.BusinessLogic
{
    public class ProductService : IProductService
    {
        private readonly IBaseRepository<Product> _prdRepo;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductService(IBaseRepository<Product> productRepository, ICloudinaryService cloudinaryService)
        {
            _prdRepo = productRepository;
            _cloudinaryService = cloudinaryService;
        }
        public async Task<EntityPage<Product>> GetAllAsync(QueryModel queryModel)
        {

            var query = _prdRepo.GetAll(
                p=>p.Variants!,
                p=>p.Reviews!,
                p => p.Images!);

            //Filtering
            query = query.WhereIf(queryModel.CategoryID != 0, p => p.CategoryId == queryModel.CategoryID)
                .WhereIf(!string.IsNullOrEmpty(queryModel.search), p => p.Title.ToLower().Contains(queryModel.search))
                .WhereIf(!string.IsNullOrEmpty(queryModel.Size), p => p.Variants!.Any(v => v.Size == queryModel.Size))
                .Where(p => p.Price >= queryModel.MinPrice && p.Price <= queryModel.MaxPrice);


            //Sorting
            if (string.IsNullOrEmpty(queryModel.OrderBy))
                query = query.OrderByDescending(p => p.Id);
            else if (queryModel.OrderBy == "ASC")
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
            var variants = allProducts.SelectMany(p => p.Variants!).ToList();
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
            var product = await _prdRepo.FindAsync(p => p.Id == productId,
                p => p.Reviews!);
            if (product!.Reviews is null || !product.Reviews.Any())
                return null!;

            product.Rate = (float)product.Reviews.Average(r => r.Stars);
            await _prdRepo.UpdateAsync(product);
            return product;
        }
        public async Task<Product> CreateProductAsync(ProductInputDTO inputProduct)
        {
            var product = inputProduct.MapToProduct();
            if (inputProduct.Images != null && inputProduct.Images.Any())
            {
                product.Images = new List<Image>();
                foreach (var formFile in inputProduct.Images)
                {
                    var uploadResult = await _cloudinaryService.UploadImage(new CreateImageDto { FormFile = formFile });
                    if (uploadResult.IsSucceeded)
                    {
                        product.Images.Add(new Image
                        {
                            ImageUrl = uploadResult.ImageUrl!,
                            PublicID = uploadResult.deleteID,
                        });
                    }
                }
            }
            await _prdRepo.AddAsync(product);
            return product;
        }
    }

}
