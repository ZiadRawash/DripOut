using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace DripOut.Domain.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(100,ErrorMessage ="Title is too big")]
        public string Title { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required , Precision(18,2)]
        [Range(0,10000)]
        public decimal Price { get; set; }

        [Range(0,100)]
        public int Amount { get; set; }

        [MinLength(0)]
        public double Discount { get; set; } = 0.0;

        [Range(0,5)]
        public float Rate { get; set; } = 0;

        public virtual ICollection<ProductVariant>? Variants { get; set; }
        public virtual ICollection<Image>? Images { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<Review>? Reviews { get; set; }
    }
}