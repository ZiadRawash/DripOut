using DripOut.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs
{
    public class ProductsDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Amount { get; set; }

        public string Size { get; set; } = null!;

        public string Color { get; set; } = null!;

        public double Discount { get; set; } = 0;

        public float Rate { get; set; } = 0;

        public byte[]? Photo { get; set; } = null!;

        public int CategoryId { get; set; }
        public ICollection<ReviewDTO>? Reviews { get; set; }

    }
}
