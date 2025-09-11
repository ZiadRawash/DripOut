using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Governorate
{
	public class UpdateShippingCostDto
	{
		[Required(ErrorMessage = "Shipping cost is required")]
		[Range(0, double.MaxValue, ErrorMessage = "Shipping cost must be non-negative")]
		public decimal ShippingCost { get; set; }
	}
}
