using DripOut.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Domain.Models
{
	public class Order
	{
		public int Id { get; set; }
		public DateTime OrderDate { get; set; } = DateTime.UtcNow;

		public decimal TotalCost { get; set; }

		public OrderStatus Status { get; set; }

		public string ShippingName { get; set; } = string.Empty;
		public string ShippingAddress { get; set; } = string.Empty;
		public string ShippingPhone { get; set; } = string.Empty;
		public string? PaymentIntentId { get; set; }

		public decimal ShippingCost { get; set; }// snapshot
		public string ShippingGovernorateNameSnapshot { get; set; } = string.Empty; // snapshot
		public decimal ShippingCostSnapShot { get; set; }                           // snapshot                       
		public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

		public int GovernorateId { get; set; }
		public Governorate Governorate { get; set; } = null!;
		public string AppUserId { get; set; } = string.Empty;
		public AppUser User { get; set; } = null!;
	}
}
