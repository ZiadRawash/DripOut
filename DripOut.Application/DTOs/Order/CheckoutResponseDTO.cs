using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Order
{
	public class CheckoutResponseDTO
	{
		public bool IsSucceeded { get; set; }
		public string Message { get; set; }=string.Empty;

		// Order details
		public int? OrderId { get; set; }
		public decimal Subtotal { get; set; }            // مجموع أسعار المنتجات
		public decimal ShippingCostBeforeDiscount { get; set; }
		public decimal ShippingDiscount { get; set; }
		public decimal ShippingCostAfterDiscount { get; set; }
		public decimal TotalCost { get; set; }           // Subtotal + ShippingAfterDiscount

		// Errors if any
		public List<InvalidCartItemDTO> InvalidItems { get; set; } = new();
	}
}
