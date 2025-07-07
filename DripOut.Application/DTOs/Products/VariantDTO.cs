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
    public class VariantDTO
    {
        public int Id { get; set; }
        public string Size { get; set; } = ProductSize.M;
        public int StockQuantity { get; set; }
    }
}
