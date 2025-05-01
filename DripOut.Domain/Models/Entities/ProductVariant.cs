using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Domain.Models.Entities
{
    public class ProductVariant
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Size { get; set; } = null!;

        public int StockQuantity { get; set; }

        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}
