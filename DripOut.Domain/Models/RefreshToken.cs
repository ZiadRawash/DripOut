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
		public string Token { get; set; } = string.Empty;
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
		public DateTime ExpiresOn { get; set; } = DateTime.UtcNow.AddDays(7);
		public DateTime? RevokedOn { get; set; }

		public bool IsActive => RevokedOn == null && DateTime.UtcNow <= ExpiresOn;
		public string AppUserId { get; set; } = string.Empty;
		public AppUser? AppUser { get; set; }
	}
}
