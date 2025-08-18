using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Domain.Models
{
	public class ReviewVote
	{
		public int Id { get; set; }

		public int ReviewId { get; set; }
		public Review Review { get; set; } = null!;

		public string AppUserId { get; set; } = string.Empty;
		public AppUser User { get; set; } = null!;

		public bool Votedup {  get; set; }
		public bool Voteddown { get; set; }



	}
}
