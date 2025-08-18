using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Domain.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required, MaxLength(1000)]
        public string ReviewText { get; set; } = null!;

        [Range(1,5)]
        //how the default is 1????????????
        public double Stars { get; set; } = 1;
        public int Ups { get; set; } = 0;
        public int Downs { get; set; } = 0;
        
        public int totalReacts => Ups-Downs;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public string AppUserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }
		public ICollection<ReviewVote> ReviewVotes { get; set; } = new List<ReviewVote>();
	}
}
