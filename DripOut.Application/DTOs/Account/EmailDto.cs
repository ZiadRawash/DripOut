using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Account
{
	public class EmailDto
	{
		[Required]
		[EmailAddress]
		public string email { get; set; } = string.Empty;
	}
}
