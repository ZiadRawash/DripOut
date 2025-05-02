using DripOut.Application.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs
{
    public class CatWithPrdDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public EntityPage<ProductDTO> ProductsPage { get; set; } = null!;
    }
}
