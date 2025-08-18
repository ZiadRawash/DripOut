using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Domain.Models
{
	public class Cart
	{
		public int Id { get; set; }
		public AppUser AppUser { get; set; }
		public string AppUserId { get; set; } = string.Empty;
		public DateTime CreatedOn { get; set; }= DateTime.UtcNow;
		public DateTime UpdatedOn { get; set; }
		public Collection<CartItem> CartItems { get; set; }= new Collection<CartItem>();

	}
}
