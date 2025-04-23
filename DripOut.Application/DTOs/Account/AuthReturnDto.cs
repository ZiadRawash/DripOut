﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Account
{
	public class AuthReturnDto
	{
		public string? Email { get; set; }
		public string? Password { get; set; }
		public string? Role { get; set; }
		public string? Token { get; set; }
		public string? RefreshToken { get; set; }
	}
}
