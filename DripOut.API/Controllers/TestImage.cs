using DripOut.Application.DTOs.Image;
using DripOut.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DripOut.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestImage : ControllerBase
	{
		private readonly ICloudinaryService _cloudinaryService;
		public TestImage(ICloudinaryService cloudinaryService)
		{
			_cloudinaryService = cloudinaryService;
		}
		[HttpPost("upload")]
		public async Task<IActionResult> UploadPhoto(CreateImageDto createdimageDto)
		{
			if (createdimageDto == null)
			{
				return BadRequest("No file uploaded.");
			}

			SavedImageDto imageUrl = await _cloudinaryService.UploadImage(createdimageDto);
			return Ok(imageUrl);
		}
		[HttpPost("DeleteImage")]
		public async Task<IActionResult> DeletePhoto(string id)
		{
			if (id == null)
			{
				return BadRequest("No file uploaded.");
			}

			SavedImageDto Deleted = await _cloudinaryService.DeleteImage(id);
			return Ok(Deleted);
		}
	}
}
