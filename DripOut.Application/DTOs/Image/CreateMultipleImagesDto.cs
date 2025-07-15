using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Image
{
	public class CreateMultipleImagesDto
	{
		[Required(ErrorMessage = "At least one file is required.")]
		[MinLength(1, ErrorMessage = "At least one file must be provided.")]
		[MaxLength(3, ErrorMessage = "At most three file must be provided.")]
		public required IList<IFormFile> FormFiles { get; set; }
		[Required]
		public int ProductId { get; set; }
    }
}
