using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Domain.Models
{
	public class StockReservation
	{
		
        public int Id { get; set; }


		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
		public int Quantity { get; set; }

		[Required]
		public DateTime ExpiresAt { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public string? UserId { get; set; }

		[NotMapped]
		public bool IsExpired => DateTime.UtcNow > ExpiresAt;

		[NotMapped]
		public TimeSpan TimeRemaining => ExpiresAt > DateTime.UtcNow
			? ExpiresAt - DateTime.UtcNow
			: TimeSpan.Zero;

		public int OrderItemId { get; set; }
		public int ProductVariantId { get; set; }
		public virtual OrderItem OrderItem { get; set; } = null!;
		public virtual ProductVariant ProductVariant { get; set; } = null!;
	}
}
