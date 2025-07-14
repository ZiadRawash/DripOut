using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Reviews
{
    public class ReviewInputDTO
    {

        [Required, MaxLength(1000)]
        public string ReviewText { get; set; } = null!;

        [Range(1, 5)]
        public double Stars { get; set; } = 1;
        [Required]
        public int ProductId { get; set; }
    }
}
