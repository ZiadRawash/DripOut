using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Domain.Models
{
	public class RefreshToken
	{
		public int Id { get; set; } 
		public string Token { get; set; }
		public DateTime CreatedOn { get; private set; } = DateTime.UtcNow;
		public DateTime ExpiresOn { get; private set; } = DateTime.UtcNow.AddDays(7);
		public DateTime? RevokedOn { get; set; }

		public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
		public bool IsActive => RevokedOn == null && !IsExpired;

		public string AppUserId { get; set; }
		public AppUser AppUser { get; set; }
	}
}
