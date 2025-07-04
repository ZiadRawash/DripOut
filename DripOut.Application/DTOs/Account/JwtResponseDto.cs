﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Account
{
	public class JwtResponseDto: ServiceResponse
	{
		public string? Token { get; set; }
		public string? RefreshToken { get; set; }
		public string? Email { get; set; }
	}
}
