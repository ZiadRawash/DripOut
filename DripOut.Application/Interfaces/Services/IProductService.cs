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
		public Task<Product?> GetByIdAsync(int id);
		public Task<Product> UpdateRateAsync(int productId);
		public Task<Product> CreateProductAsync(ProductInputDTO prdInputDTO);
		public Task<bool> AddVarientAsync(VariantDTO variantDTO);
		public Task<bool> UpdateProductAsync(int id, ProductInputDTO inputProduct);
		public Task<bool> DeleteProductAsync(int id);
	}
}