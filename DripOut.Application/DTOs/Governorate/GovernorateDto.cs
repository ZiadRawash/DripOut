using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Governorate
{
	public class GovernorateDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public decimal ShippingCost { get; set; }
		public decimal MinimumOrderForFreeShipping { get; set; }
		public decimal DiscountPercentage { get; set; }
		public bool IsActive { get; set; }
	}
}
