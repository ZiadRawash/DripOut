using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using DripOut.Application.Common.Settings;
using DripOut.Application.DTOs.Image;
using DripOut.Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using DripOut.Domain.Models;

namespace DripOut.Infrastructure.Implementation
{
	public class CloudinaryService: ICloudinaryService
	{
		private readonly CloudinarySettings _cloudinarySettings;
		private readonly Cloudinary _cloudinary;

		public CloudinaryService(IOptions<CloudinarySettings> CloudinarySettings)
		{
			_cloudinarySettings= CloudinarySettings.Value;
			var account = new Account(
			_cloudinarySettings.CloudName,
			_cloudinarySettings.ApiKey,
			_cloudinarySettings.ApiSecret);
			_cloudinary= new Cloudinary(account);

		}

		public async Task<SavedImageDto> DeleteImage(string productId)
		{
			if (string.IsNullOrEmpty(productId))
			{
				return new SavedImageDto { IsSucceeded = false, Errors = ["publicID Cant be Null"] };
			}

			var deletionParams = new DeletionParams(productId)
			{
				ResourceType = ResourceType.Image,
				Invalidate = true
			};

			var result = await _cloudinary.DestroyAsync(deletionParams);
			var Destroy = await _cloudinary.DestroyAsync(deletionParams);
			
			if (result.StatusCode == System.Net.HttpStatusCode.OK && result.Result == "ok")
			{
				return new SavedImageDto { IsSucceeded = true };
			}
			return new SavedImageDto { IsSucceeded = false,Errors = ["Error Happened while Uploading Image"] };
		}
		public async Task<SavedImageDto> UploadImage(CreateImageDto model)
		{
			if (model.FormFile != null)
			{
				using (var stream = new MemoryStream())
				{
					await model.FormFile.CopyToAsync(stream);
					stream.Position = 0;
					var uploadParams = new ImageUploadParams
					{
						File = new FileDescription(Guid.NewGuid().ToString(), stream),
						Transformation = new Transformation()					
						.FetchFormat("auto"),
						Folder = "products"
					};
					var uploadResult = _cloudinary.Upload(uploadParams);
					if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
					{
						var Url = uploadResult.SecureUrl.ToString();
						var PublicId = uploadResult.PublicId;
						return new SavedImageDto { ImageUrl = Url, ProductId = PublicId, IsSucceeded = true, Message = uploadResult.StatusCode.ToString() + "____" + StatusCodes.Status200OK.ToString() };
					}
					return new SavedImageDto {  IsSucceeded = false, Message = uploadResult.StatusCode.ToString() + "____" + StatusCodes.Status200OK.ToString() };


				}
			}
			return new SavedImageDto {  IsSucceeded = false, Message = "U Should Upload Image" };


		}
	
	}
}
