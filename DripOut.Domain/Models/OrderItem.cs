using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Domain.Models
{
	public class OrderItem
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public virtual Order Order { get; set; } = null!;

		public int ProductVariantId { get; set; }
		public virtual ProductVariant ProductVariant { get; set; } = null!;

		public string ProductName { get; set; } = string.Empty;
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }
		public decimal TotalPrice { get; set; }
		public decimal DiscountApplied { get; set; }


		public virtual StockReservation? StockReservation { get; set; }
	}
}
