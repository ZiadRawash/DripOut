using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Image
{
	public class SavedImageDto: ServiceResponse
	{
		public string? ImageUrl { get; set; } = null!;
		public string? ProductId { get; set; } = null!;

	}
}
