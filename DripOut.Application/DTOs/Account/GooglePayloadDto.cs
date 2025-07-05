using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Account
{
	public class GooglePayloadDto: ServiceResponse
	{
			public string Email { get; set; }=string.Empty;
			public string FullName { get; set; } = string.Empty;
			public string GivenName { get; set; } = string.Empty;
			public string FamilyName { get; set; } = string.Empty;
			public string Picture { get; set; } = string.Empty;
			public string GoogleId { get; set; }= string.Empty;
	}

}

