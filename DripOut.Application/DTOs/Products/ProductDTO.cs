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
    public class ProductDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public decimal Price { get; set; }

        public double Discount { get; set; } = 0;

        public float Rate { get; set; } = 0;

		public virtual ICollection<DripOut.Domain.Models.Image>? Images { get; set; }


    }
}
