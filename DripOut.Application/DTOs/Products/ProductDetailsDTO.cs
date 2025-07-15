using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Products
{
    public class ProductDetailsDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Amount { get; set; }

        public double Discount { get; set; } = 0.0;

        public float Rate { get; set; } = 0;
        public int Reviews { get; set; } = 0;

        public virtual List<string>? Images { get; set; }

        public virtual IEnumerable<VariantDTO>? Variants { get; set; }

        public int CategoryId { get; set; }
    }
}
