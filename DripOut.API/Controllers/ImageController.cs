using DripOut.Application.DTOs.Image;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Application.Interfaces.Services;
using DripOut.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DripOut.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ImageController : ControllerBase
	{
		private readonly ICloudinaryService _cloudinaryService;
		private readonly IUnitOfWork _unitOfWork;
		public ImageController(ICloudinaryService cloudinaryService , IUnitOfWork unitOfWork)
		{
			_cloudinaryService = cloudinaryService;
			_unitOfWork = unitOfWork;
        }
		[HttpPost("uploadUserImage")]
		public async Task<IActionResult> UploadPhotoToUser(CreateImageDto createdimageDto)
		{
			if (createdimageDto == null)
			{
				return BadRequest("No file uploaded.");
			}
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            SavedImageDto imageUrl = await _cloudinaryService.UploadImage(createdimageDto);

			if (imageUrl.IsSucceeded && imageUrl.ImageUrl is not null)
			{
				var image = new Image()
				{
					ImageUrl = imageUrl.ImageUrl,
					PublicID = imageUrl.deleteID,
					AppUserId = userId
				};
                await _unitOfWork.Images.AddAsync(image);
				return Ok(imageUrl.Message);
            }
			return BadRequest(imageUrl.Message);
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
		[HttpPost("uploadProductImages")]
		public async Task<IActionResult> UploadMultipleImages([FromForm] CreateMultipleImagesDto model)
		{
			try
			{
				var result = await _cloudinaryService.UploadMultipleFiles(model);
				var product = await _unitOfWork.Products.FindAsync(model.ProductId);
				if(product is null)
					return BadRequest("Product not found");

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
					foreach(var image in successfulUploads)
					{
                        var newImage = new Image
                        {
                            ImageUrl = image.imageUrl!,
                            PublicID = image.deleteId,
                            ProductId = model.ProductId
                        };
                        await _unitOfWork.Images.AddAsync(newImage);
                    }
					return Ok();
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
