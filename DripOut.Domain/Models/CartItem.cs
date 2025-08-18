using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Domain.Models
{
	public class CartItem
	{
		public int Id { get; set; }

		public int Quantity { get; set; }

		public int CartId { get; set; }
		public Cart Cart { get; set; }

		public int ProductVariantId { get; set; }
		public ProductVariant ProductVariant { get; set; }
	}
}
