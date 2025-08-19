using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Cart
{
	public class CartItemDto
	{
		public int CartId { get; set; }
		public int VariantId { get; set; }
		public string VarientName { get; set; }
		public string ImageUrl { get; set; }
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }
		public decimal Subtotal { get; set; }
		public decimal DiscountApplied { get; set; }
		public decimal Total { get; set; }
	}
}
