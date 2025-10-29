using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Order
{
	public class CartValidationResultDto
	{
		public bool IsValid { get; set; }
		public List<InvalidCartItemDTO> InvalidItems { get; set; } = new();
	}
}
