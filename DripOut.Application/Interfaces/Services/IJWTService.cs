using DripOut.Application.DTOs.Account;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.Services
{
	/// <summary>
	/// Provides operations for generating and managing JWT access and refresh tokens.
	/// </summary>
	public interface IJWTService
	{
		/// <summary>
		/// Generates a new JWT access token for the specified user.
		/// </summary>
		/// <param name="email">The email of the user for whom the token will be generated.</param>
		/// <returns>
		/// A <see cref="JwtResponseDto"/> containing the generated access token 
		/// and the result of the operation.
		/// </returns>
		Task<JwtResponseDto> GenerateJWTTokenAsync(string email);

		/// <summary>
		/// Generates a new refresh token for the specified user, 
		/// revoking any active refresh token if it exists.
		/// </summary>
		/// <param name="email">The email of the user for whom the refresh token will be generated.</param>
		/// <returns>
		/// A <see cref="JwtResponseDto"/> containing the new refresh token 
		/// and the result of the operation.
		/// </returns>
		Task<JwtResponseDto> GenerateRefreshTokenAsync(string email);

		/// <summary>
		/// Finds the email of the user associated with the given refresh token.
		/// </summary>
		/// <param name="refreshToken">The refresh token to search for.</param>
		/// <returns>
		/// A <see cref="JwtResponseDto"/> containing the associated email if found, 
		/// otherwise an error message.
		/// </returns>
		Task<JwtResponseDto> FindEmailByRefreshToken(string refreshToken);

		/// <summary>
		/// Revokes all refresh tokens for the user with the specified email.
		/// </summary>
		/// <param name="email">The email of the user whose refresh tokens will be revoked.</param>
		/// <returns>
		/// A <see cref="JwtResponseDto"/> containing the result of the revocation operation.
		/// </returns>
		Task<JwtResponseDto> RevokeAllRefreshTokensByEmailAsync(string email);

		/// <summary>
		/// Revokes a specific refresh token.
		/// </summary>
		/// <param name="refreshToken">The refresh token to revoke.</param>
		/// <returns>
		/// A <see cref="JwtResponseDto"/> containing the result of the revocation operation.
		/// </returns>
		Task<JwtResponseDto> RevokeRefreshTokenAsync(string refreshToken);
	}
}
