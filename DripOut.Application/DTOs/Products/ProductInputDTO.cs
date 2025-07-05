using DripOut.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Products
{
    public class ProductInputDTO
    {
        [Required, MaxLength(100, ErrorMessage = "Title is too big")]
        public string Title { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required, Precision(18, 2)]
        [Range(0, 10000)]
        public decimal Price { get; set; }

        public int CategoryId { get; set; }

    }
}
