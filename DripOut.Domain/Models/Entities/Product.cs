using DripOut.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DripOut.Domain.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required , Precision(18,2)]
        public decimal Price { get; set; }

        public int Amount { get; set; }

        [Required, MaxLength(20)]
        public string Size { get; set; } = null!;

        [Required, MaxLength(30)]
        public string Color { get; set; } = null!;

        public double Discount { get; set; } = 0;

        public float Rate { get; set; } = 0;

        public byte[]? Photo { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<Review>? Reviews { get; set; }
    }
}