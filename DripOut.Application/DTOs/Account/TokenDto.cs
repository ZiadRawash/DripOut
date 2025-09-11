using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Account
{
	public class TokenDto
	{
		[Required]
		public string Token { get; set; } = string.Empty;
	}
}
