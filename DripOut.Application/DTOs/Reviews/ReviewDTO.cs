using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DripOut.Application.DTOs.Account;

namespace DripOut.Application.DTOs.Reviews
{
    public class ReviewDTO
    {

        [Required, MaxLength(1000)]
        public string ReviewText { get; set; } = default!;
        [Range(0,5)]
        public double Stars { get; set; } = 1;
        public DateTime CreatedOn { get; set; }
        public UserDTO User { get; set; } = default!;

    }
}
