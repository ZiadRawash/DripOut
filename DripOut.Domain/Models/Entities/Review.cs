using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Domain.Models.Entities
{
    public class Review
    {
        public int Id { get; set; }

        [Required, MaxLength(1000)]
        public string ReviewText { get; set; } = default!;

        public double Stars { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public string AppUserId { get; set; }
        public AppUser? User { get; set; }
    }
}
