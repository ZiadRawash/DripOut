using DripOut.Application.DTOs.Categories;
using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Mappers
{
    public static class CategoryMapper
    {
        public static CategoryDTO MapToCategoryDTO(this Category category)
        {
            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name
            };
        }
        public static CatWithPrdDTO MapToCatWithPrdDTO(this Category category)
        {
            return new CatWithPrdDTO
            {
                Id = category.Id,
                Name = category.Name,
                Products = category.Products?.Select(p => p.MapToProductDTO())
            };
        }
    }
}
