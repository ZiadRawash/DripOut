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
		[HttpPost("upload-multiple")]
		public async Task<IActionResult> UploadMultipleImages([FromForm] CreateMultipleImagesDto model)
		{
			try
			{
				var result = await _cloudinaryService.UploadMultipleFiles(model);

				if (result.IsSucceeded)
				{
					// Get successful uploads info
					var successfulUploads = result.UploadResults
						.Where(r => r.IsSucceeded)
						.Select(r => new
						{
							imageUrl = r.ImageUrl,
							deleteId = r.DeleteId
						})
						.ToList();

					return Ok(new
					{
						success = true,
						message = result.Message,
						totalUploaded = result.SuccessfulUploads,
						totalFailed = result.FailedUploads,
						images = successfulUploads
					});
				}

				return BadRequest(new
				{
					success = false,
					message = result.Message,
					totalUploaded = result.SuccessfulUploads,
					totalFailed = result.FailedUploads
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					success = false,
					message = "An error occurred while uploading images",
					error = ex.Message
				});
			}
		}
	}
}
