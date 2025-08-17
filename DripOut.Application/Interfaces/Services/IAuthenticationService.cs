using DripOut.Application.Common;
using DripOut.Application.DTOs.Account;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.Services
{
	/// <summary>
	/// Provides authentication and authorization related operations
	/// such as registration, login, token handling, and external sign-in.
	/// </summary>
	public interface IAuthenticationService
	{
		/// <summary>
		/// Verifies a user's email using a confirmation code.
		/// </summary>
		/// <param name="email">The email address of the user.</param>
		/// <param name="code">The confirmation code sent to the user's email.</param>
		/// <returns>
		/// A result containing authentication data if verification is successful,
		/// otherwise an error result.
		/// </returns>
		Task<Result<AuthReturnDto>> VerifyUser(string email, string code);

		/// <summary>
		/// Registers a new user in the system.
		/// </summary>
		/// <param name="model">The registration details including email, password, and personal info.</param>
		/// <returns>
		/// A result containing basic authentication data along with a confirmation code,
		/// or an error if registration fails.
		/// </returns>
		Task<Result<AuthReturnDto>> RegisterAsync(RegisterDto model);

		/// <summary>
		/// Logs in a user with their credentials.
		/// </summary>
		/// <param name="model">The login details including email and password.</param>
		/// <returns>
		/// A result containing authentication tokens and user info if login is successful,
		/// otherwise an error.
		/// </returns>
		Task<Result<AuthReturnDto>> LoginAsync(LoginDto model);

		/// <summary>
		/// Generates a new access token using a valid refresh token.
		/// </summary>
		/// <param name="refreshToken">The refresh token issued to the user.</param>
		/// <returns>
		/// A result containing a new access token if the refresh token is valid,
		/// otherwise an error.
		/// </returns>
		Task<Result<AuthReturnDto>> AccessRefreshToken(string refreshToken);

		/// <summary>
		/// Logs out the user by revoking the given refresh token.
		/// </summary>
		/// <param name="refreshToken">The refresh token to be revoked.</param>
		/// <returns>
		/// A result indicating whether the logout operation succeeded or failed.
		/// </returns>
		Task<Result> LogOutAsync(string refreshToken);

		/// <summary>
		/// Signs in a user using an external provider (e.g., Google).
		/// </summary>
		/// <param name="id">The external provider's identifier for the user.</param>
		/// <returns>
		/// A result containing authentication tokens if the sign-in is successful,
		/// otherwise an error.
		/// </returns>
		Task<Result<AuthReturnDto>> SigninExternal(string id);

		// <summary>
		/// Resends the email verification code to the specified email address.
		/// </summary>
		/// <param name="email">The email address of the user.</param>
		/// <returns>
		/// A result containing the new confirmation code if successful,
		/// otherwise an error.
		/// </returns>
		Task<Result<AuthReturnDto>> ResendVerificationCodeAsync(string email);
	}
}
