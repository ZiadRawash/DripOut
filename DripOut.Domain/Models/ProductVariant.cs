using DripOut.Domain.Consts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Domain.Models

{
    public class ProductVariant
    {
        public int Id { get; set; }

        [Required, MaxLength(5,ErrorMessage ="No such size")]
        public string Size { get; set; } = ProductSize.M;

        [Required , Range(0,20)]
        public int StockQuantity { get; set; }
        [Required]
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}
    