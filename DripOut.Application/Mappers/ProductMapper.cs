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
                Description = product.Description,
                Price = product.Price,
                Discount = product.Discount,
                Rate = product.Rate,
                Reviews = product.Reviews?.Count ?? 0,
                Image =  product.Images?.Select(i => i.ImageUrl).FirstOrDefault(),
                CategoryId = product.CategoryId
            };
        }
        public static ProductDetailsDTO MapToDetailedProdDTO(this Product product)
        {
            return new ProductDetailsDTO
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Amount = product.Amount,
                Price = product.Price,
                Discount = product.Discount,
                Rate = product.Rate,
                Reviews = product.Reviews?.Count ?? 0,
                Variants = product.Variants is null ? [] : product.Variants.Select(v=>v.MapToVariantDTO()),
                Images = product.Images?.Select(i => i.ImageUrl).ToList(),
                CategoryId = product.CategoryId
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
                TotalPages = productsPage.TotalPages,
                MinPrice = productsPage.MinPrice,
                MaxPrice = productsPage.MaxPrice,
                Sizes = productsPage.Sizes
            };
        }
        public static Product MapToProduct(this ProductInputDTO productInput)
        {
            return new Product
            {
                Title = productInput.Title,
                Description = productInput.Description,
                Price = productInput.Price,
                Discount = productInput.Discount,
                CategoryId = productInput.CategoryId,
            };
        }

        //This function takes a new product to modify the old one , avoiding creating new product object
        public static void UpdateProduct(this Product oldProduct,ProductInputDTO newProduct)
        {
            oldProduct.Title = newProduct.Title;
            oldProduct.Description = newProduct.Description;
            oldProduct.Price = newProduct.Price;
            oldProduct.Discount = newProduct.Discount;
            oldProduct.CategoryId = newProduct.CategoryId;
        }
        public static ProductVariant MapToProductVariant(this VariantDTO variantInput)
        {
            return new ProductVariant
            {
                Size = variantInput.Size,
                StockQuantity = variantInput.StockQuantity,
                ProductId = variantInput.ProductId
            };
        }
        public static VariantDTO MapToVariantDTO(this ProductVariant productVariant)
        {
            return new VariantDTO
            {
                ProductId = productVariant.ProductId,
                StockQuantity = productVariant.StockQuantity,
                Size = productVariant.Size
            };
        }
    }
}
