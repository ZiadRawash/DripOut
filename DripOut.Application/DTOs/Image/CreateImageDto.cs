using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace DripOut.Application.DTOs.Image
{
	public class CreateImageDto
	{
		

		[Required(ErrorMessage = "File is required.")]
		
		public required IFormFile FormFile { get; set; }

	}
}
