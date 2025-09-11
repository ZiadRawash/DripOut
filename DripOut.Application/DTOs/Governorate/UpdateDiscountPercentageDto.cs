using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Governorate
{
	public class UpdateDiscountPercentageDto
	{
		[Required(ErrorMessage = "Discount percentage is required")]
		[Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100")]
		public decimal DiscountPercentage { get; set; }
	}
}
