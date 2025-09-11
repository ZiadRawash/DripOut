using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Cart
{
	public class UpdateCartItemDTO
	{
		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
		public int cartItemId { get; set; }

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
		public int Quantity { get; set; }
	}
}
