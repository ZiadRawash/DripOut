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

namespace DripOut.Infrastructure
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

		public async Task<string> UploadImage(CreateImageDto model)
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
					return uploadResult.SecureUrl.ToString();
				}
			}
			return null;
		}
	}
}
