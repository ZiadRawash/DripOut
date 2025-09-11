using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Account
{
	public class TokenWithRefreshtokenDTO
	{
		public string Token {  get; set; }=string.Empty;
		public string RefreshToken { get; set; }=string.Empty;

	}
}
