using DripOut.Application.DTOs.Image;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.Services
{
	/// <summary>
	/// Defines operations for managing images with Cloudinary,
	/// including upload, deletion, and handling multiple files.
	/// </summary>
	public interface ICloudinaryService
	{
		/// <summary>
		/// Uploads a single image to Cloudinary.
		/// </summary>
		/// <param name="model">The image data and file to be uploaded.</param>
		/// <returns>
		/// A result containing the uploaded image information (URL, public ID)
		/// or an error if the upload fails.
		/// </returns>
		Task<SavedImageDto> UploadImage(CreateImageDto model);

		/// <summary>
		/// Deletes an image from Cloudinary using its public ID.
		/// </summary>
		/// <param name="publicID">The Cloudinary public ID of the image.</param>
		/// <returns>
		/// A result indicating whether the image was successfully deleted.
		/// </returns>
		Task<SavedImageDto> DeleteImage(string publicID);

		/// <summary>
		/// Uploads multiple image files to Cloudinary.
		/// </summary>
		/// <param name="model">The collection of image files to be uploaded.</param>
		/// <returns>
		/// A result containing a list of upload results for each file,
		/// including success and failure details.
		/// </returns>
		Task<SavedMultipleImagesDto> UploadMultipleFiles(CreateMultipleImagesDto model);
	}
}
