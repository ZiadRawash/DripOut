using DripOut.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Account
{
	public class IdentityDto
	{
		public string? Message { get; set; }
		public bool IsSucceeded { set; get; } = false;
		public IEnumerable<string> Errors { set; get; }= Enumerable.Empty<string>();
		public AppUser? User { set; get; }
		public string? Token { set; get; }
		public string? RefreshToken { set; get; }
		public string? Email { set; get; }

	}
}
