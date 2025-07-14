using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Image
{
	public class SavedMultipleImagesDto : ServiceResponse
	{
		public List<ImageUploadResult> UploadResults { get; set; } = new();
		public int SuccessfulUploads { get; set; }
		public int FailedUploads { get; set; }
	}

}
