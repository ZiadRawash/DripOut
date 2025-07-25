﻿using CloudinaryDotNet.Actions;
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
using ImageUploadResult = DripOut.Application.DTOs.Image.ImageUploadResult;

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
						return new SavedImageDto { IsSucceeded = true,  ImageUrl = Url, deleteID = PublicId, Message = uploadResult.StatusCode.ToString() + "____" + StatusCodes.Status200OK.ToString() };
					}
					return new SavedImageDto {  IsSucceeded = false, Message = uploadResult.StatusCode.ToString() + "____" + StatusCodes.Status200OK.ToString() };
				}
			}
			return new SavedImageDto {  IsSucceeded = false, Message = "Un appropriate Image format" };
		}

		//public async Task<SavedMultipleImagesDto> UploadMultipleFiles(CreateMultipleImagesDto model)
		//{
		//	var response = new SavedMultipleImagesDto();
		//	if (model == null || !model.FormFiles.Any())
		//	{
		//		response.Message = "No files uploaded";
		//		response.IsSucceeded = false;
		//		return response;
		//	}
		//	var uploadTasks = model.FormFiles.Select(async file =>
		//	{
		//		if (file == null)
		//		{
		//			return new ImageUploadResult
		//			{
		//				IsSucceeded = false,
		//			};
		//		}

		//		try
		//		{
		//			using (var stream = new MemoryStream())
		//			{
		//				await file.CopyToAsync(stream);
		//				stream.Position = 0;

		//				var uploadParams = new ImageUploadParams
		//				{
		//					File = new FileDescription(Guid.NewGuid().ToString(), stream),
		//					Transformation = new Transformation()
		//						.FetchFormat("auto"),
		//					Folder = "products"
		//				};

		//				var uploadResult = _cloudinary.Upload(uploadParams);

		//				if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
		//				{
		//					return new ImageUploadResult
		//					{
		//						IsSucceeded = true,
		//						ImageUrl = uploadResult.SecureUrl.ToString(),
		//						DeleteId = uploadResult.PublicId,
		//					};
		//				}

		//				return new ImageUploadResult
		//				{
		//					IsSucceeded = false,
		//					ImageUrl = null,
		//					DeleteId = null,
		//				};
		//			}
		//		}
		//		catch 
		//		{
		//			return new ImageUploadResult
		//			{

		//				IsSucceeded = false,
		//			};
		//		}
		//	});

		//	var results = await Task.WhenAll(uploadTasks);

		//	response.UploadResults = results.ToList();

		//	response.IsSucceeded = response.SuccessfulUploads > 0;

		//	return response;
		//}		
		public async Task<SavedMultipleImagesDto> UploadMultipleFiles(CreateMultipleImagesDto model)
		{
			var response = new SavedMultipleImagesDto();

			if (model == null || model.FormFiles == null || !model.FormFiles.Any())
			{
				response.Message = "No files uploaded";
				response.IsSucceeded = false;
				response.SuccessfulUploads = 0;
				response.FailedUploads = 0;
				return response;
			}

			var uploadTasks = model.FormFiles.Select(async file =>
			{
				if (file == null || file.Length == 0)
				{
					return new ImageUploadResult
					{
						IsSucceeded = false,
						ImageUrl = null,
						DeleteId = null,
					};
				}

				try
				{
					using (var stream = new MemoryStream())
					{
						await file.CopyToAsync(stream);
						stream.Position = 0;

						var uploadParams = new ImageUploadParams
						{
							File = new FileDescription(Guid.NewGuid().ToString(), stream),
							Transformation = new Transformation()
								.FetchFormat("auto"),
							Folder = "products"
						};

						var uploadResult = await _cloudinary.UploadAsync(uploadParams);

						if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
						{
							return new ImageUploadResult
							{
								IsSucceeded = true,
								ImageUrl = uploadResult.SecureUrl.ToString(),
								DeleteId = uploadResult.PublicId,
							};
						}

						return new ImageUploadResult
						{
							IsSucceeded = false,
							ImageUrl = null,
							DeleteId = null,
						};
					}
				}
				catch (Exception ex)
				{

					return new ImageUploadResult
					{
						IsSucceeded = false,
						ImageUrl = null,
						DeleteId = null,
					};
				}
			});

			var results = await Task.WhenAll(uploadTasks);

			response.UploadResults = results.ToList();
			response.SuccessfulUploads = results.Count(r => r.IsSucceeded);
			response.FailedUploads = results.Count(r => !r.IsSucceeded);
			response.IsSucceeded = response.SuccessfulUploads > 0;


			if (response.SuccessfulUploads == results.Length)
			{
				response.Message = "All files uploaded successfully";
			}
			else if (response.SuccessfulUploads > 0)
			{
				response.Message = $"{response.SuccessfulUploads} files uploaded successfully, {response.FailedUploads} failed";
			}
			else
			{
				response.Message = "All file uploads failed";
			}

			return response;
		}

	}
}
