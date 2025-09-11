using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Governorate
{
	public class UpdateGovernorateDto
	{
		[Required(ErrorMessage = "Governorate name is required")]
		[StringLength(100, ErrorMessage = "Governorate name cannot exceed 100 characters")]
		[MinLength(2, ErrorMessage = "Governorate name must be at least 2 characters")]
		public string Name { get; set; } = string.Empty;

		[Required(ErrorMessage = "Shipping cost is required")]
		[Range(0, double.MaxValue, ErrorMessage = "Shipping cost must be non-negative")]
		public decimal ShippingCost { get; set; }

		[Range(0, double.MaxValue, ErrorMessage = "Minimum order amount must be non-negative")]
		public decimal MinimumOrderForFreeShipping { get; set; }

		[Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100")]
		public decimal DiscountPercentage { get; set; }
	}
}
