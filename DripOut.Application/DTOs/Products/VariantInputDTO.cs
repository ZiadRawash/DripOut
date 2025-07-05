using DripOut.Domain.Consts;
using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Products
{
    public class VariantInputDTO
    {
        [Required, MaxLength(5, ErrorMessage = "No such size")]
        public string Size { get; set; } = ProductSize.M;

        [Required,Range(0,20)]
        public int StockQuantity { get; set; }
        [Required]
        public int ProductId { get; set; }
    }
}
