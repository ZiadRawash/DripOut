using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Order
{
	public class InvalidCartItemDTO
	{
		public int VariantId { get; set; }
		public int RequestedQuantity { get; set; }
		public int AvailableQuantity { get; set; }
	}
}
