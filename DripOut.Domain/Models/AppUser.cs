using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Domain.Models
{
	public class AppUser : IdentityUser
	{
		public required string FirstName { get; set; }
		public string LastName { get; set; } = string.Empty;
		public List<RefreshToken> RefreshToken { get; set; } = new List<RefreshToken>();

		public ICollection<Review>? Reviews { get; set; }

		public virtual Image? Image { get; set; }

		public IList<Favourite> Favourites { get; set; } = new List<Favourite>();
    }

}

