using DripOut.Application.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Categories
{
    public class CatWithPrdDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public IEnumerable<ProductDTO>? Products { get; set; }
    }
}
