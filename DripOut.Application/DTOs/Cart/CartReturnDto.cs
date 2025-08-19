using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Cart
{
	public class CartReturnDto
	{
		public List<CartItemDto> Items { get; set; } = new();
		public int TotalItems { get; set; }
		public decimal CartSubtotal { get; set; }
		public decimal TotalDiscount { get; set; }
		public decimal CartTotal { get; set; }
	}
}
