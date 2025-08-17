using DripOut.Application.DTOs.Account;
using DripOut.Domain.Models;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.Services
{
	/// <summary>
	/// Defines operations related to identity management such as creating users,
	/// validating credentials, and handling email confirmations.
	/// </summary>
	public interface IIdentityService
	{
		/// <summary>
		/// Creates a new user with the given registration details and assigns them a role.
		/// </summary>
		/// <param name="model">The registration details including email and password.</param>
		/// <param name="role">The role to assign to the user.</param>
		/// <returns>An <see cref="IdentityDto"/> containing the operation result and optional confirmation code.</returns>
		Task<IdentityDto> CreateUserAsync(RegisterDto model, string role);

		/// <summary>
		/// Creates a new user from external authentication data (Google) and assigns them a role.
		/// </summary>
		/// <param name="model">The Google account payload with user details.</param>
		/// <param name="role">The role to assign to the user.</param>
		/// <returns>An <see cref="IdentityDto"/> containing the operation result.</returns>
		Task<IdentityDto> CreateUserAsyncForExternal(GooglePayloadDto model, string role);

		/// <summary>
		/// Validates the provided login credentials against the system's user store.
		/// </summary>
		/// <param name="model">The login details including email and password.</param>
		/// <returns>An <see cref="IdentityDto"/> indicating whether validation succeeded.</returns>
		Task<IdentityDto> ValidateUserCredentialsAsync(LoginDto model);

		/// <summary>
		/// Verifies a user's email confirmation code.
		/// </summary>
		/// <param name="email">The email of the user to confirm.</param>
		/// <param name="code">The confirmation code provided to the user.</param>
		/// <returns>An <see cref="IdentityDto"/> containing the result of the verification.</returns>
		Task<IdentityDto> verifyConfirmationCode(string email, string code);

		/// <summary>
		/// Finds a user by their email address.
		/// </summary>
		/// <param name="email">The email to search for.</param>
		/// <returns>An <see cref="IdentityDto"/> containing the user information if found.</returns>
		Task<IdentityDto> FindUserByEmailAsync(string email);

		/// <summary>
		/// Validates a Google Sign-In token and extracts the payload information.
		/// </summary>
		/// <param name="id">The Google ID token to validate.</param>
		/// <returns>A <see cref="GooglePayloadDto"/> containing user details and validation status.</returns>
		Task<GooglePayloadDto> ValidateGoogleSignin(string id);

		/// <summary>
		/// Resends an email verification code to the specified user.
		/// </summary>
		/// <param name="email">The email of the user requesting a new confirmation code.</param>
		/// <returns>An <see cref="IdentityDto"/> containing the new confirmation code if successful.</returns>
		Task<IdentityDto> ResendEmailVerificationCodeAsync(string email);
	}
}
