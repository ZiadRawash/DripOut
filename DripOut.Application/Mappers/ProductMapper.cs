using DripOut.Application.DTOs;
using DripOut.Application.DTOs.Products;
using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Mappers
{
    public static class ProductMapper
    {
        public static ProductDTO MapToProductDTO(this Product product)
        {
            return new ProductDTO
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                Discount = product.Discount,
                Rate = product.Rate,
                Images = product.Images
            };
        }
        public static EntityPage<ProductDTO> MapToProductDTO(this EntityPage<Product> productsPage)
        {
            return new EntityPage<ProductDTO>
            {
                Items =  productsPage.Items!.Select(p => p.MapToProductDTO()),
                CurrentPage = productsPage.CurrentPage,
                PageSize = productsPage.PageSize,
                Count = productsPage.Count,
                TotalPages = productsPage.TotalPages
            };
        }
        public static Product MapToProduct(this ProductInputDTO productInput)
        {
            return new Product
            {
                Title = productInput.Title,
                Description = productInput.Description,
                Price = productInput.Price,
                CategoryId = productInput.CategoryId,
            };
        }
        public static ProductVariant MapToProductVariant(this VariantInputDTO variantInput)
        {
            return new ProductVariant
            {
                Size = variantInput.Size,
                StockQuantity = variantInput.StockQuantity,
                ProductId = variantInput.ProductId
            };
        }
    }
}
