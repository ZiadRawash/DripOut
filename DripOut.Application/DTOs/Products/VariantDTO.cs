using DripOut.Domain.Consts;
using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DripOut.Application.Validators.ProductVariant;

namespace DripOut.Application.DTOs.Products
{
    public class VariantDTO
    {
        [Required,ValidSizeAtribute]
        public string Size { get; set; } = ProductSize.AllSizes[2];

        [Required,Range(0,20)]
        public int StockQuantity { get; set; }
        [Required]
        public int ProductId { get; set; }
    }
}
