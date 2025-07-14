using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Image
{
	public class ImageUploadResult
	{
		public bool IsSucceeded { get; set; }
		public string? ImageUrl { get; set; }
		public string? DeleteId { get; set; }
	}
}
