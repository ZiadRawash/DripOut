using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Account
{
	public class RefreshTokenDto
	{
		[Required]
		public string refreshToken { get; set; }=string.Empty;
	}
}
