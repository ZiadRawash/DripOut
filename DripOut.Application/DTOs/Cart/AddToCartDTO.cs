using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Cart
{
	public class AddToCartDTO
	{
		[Required]
		public int Id { get; set; }
		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
		public int Quantity { get; set; }
	}
}
