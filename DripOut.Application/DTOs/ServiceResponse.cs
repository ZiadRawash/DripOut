using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs
{
	public class ServiceResponse
	{
		public bool IsSucceeded { get; set; }
		public string? Message { get; set; }
		public List<string> Errors { get; set; } = new List<string>();
	}
}
