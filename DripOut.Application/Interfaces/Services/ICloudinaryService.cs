using DripOut.Application.DTOs.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.Services
{
	public interface ICloudinaryService
	{
		Task<SavedImageDto> UploadImage(CreateImageDto model);
		
		Task<SavedImageDto> DeleteImage(string publicID);

	}
}
	