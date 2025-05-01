using DripOut.Application.Common.Settings;
using DripOut.Application.DTOs.Account;
using DripOut.Application.Interfaces;
using DripOut.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Infrastructure.Implementation
{
	
	public class JWTService : IJWTService
	{
		private readonly JWTSettings _options;
		private readonly UserManager<AppUser> _userManager;
		private readonly SymmetricSecurityKey _key;

		public JWTService(IOptions<JWTSettings> Options, UserManager<AppUser> UserManager)
		{
			_userManager = UserManager;
			_options = Options.Value;
			_key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SignInKey!));
		}

		public async Task<JwtResponseDto> GenerateJWTTokenAsync(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				return new JwtResponseDto
				{
					IsSucceeded = false,
					Errors = { "Email is required" }
				};
			}

			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return new JwtResponseDto
				{
					IsSucceeded = false,
					Errors = { "Email is required" }
				};
			}
			try
			{
				var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id),
				new Claim(JwtRegisteredClaimNames.Email, user.Email!)
			};

				var roles = await _userManager.GetRolesAsync(user);
				claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

				var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
				var tokenExpiration = DateTime.UtcNow.AddMinutes(int.Parse(_options.AccessTokenExpiryInMinutes));

				var descriptor = new SecurityTokenDescriptor
				{
					Subject = new ClaimsIdentity(claims),
					Audience = _options.Audience,
					Issuer = _options.Issuer,
					Expires = tokenExpiration,
					SigningCredentials = creds,
				};

				var tokenHandler = new JwtSecurityTokenHandler();
				var secToken = tokenHandler.CreateToken(descriptor);
				var token = tokenHandler.WriteToken(secToken);

				return new JwtResponseDto { Token = token, IsSucceeded = true };
			}
			catch (Exception ex)
			{
				return new JwtResponseDto { IsSucceeded = false, Errors = { "Error generating token: " + ex.Message } };
			}
		}

		public async Task<JwtResponseDto> GenerateRefreshTokenAsync(string email)
		{
			if (string.IsNullOrEmpty(email))
				return new JwtResponseDto { IsSucceeded = false, Errors = { "Email Not Found" } };
			try
			{
				var user = await _userManager.FindByEmailAsync(email);
				if (user == null)
				{
					return new JwtResponseDto
					{
						IsSucceeded = false,
						Errors = { "User Not Found" }
					};
				}
				var userToken = user.RefreshToken.FirstOrDefault(e => e.IsActive);
				if (userToken != null)
				{
					userToken.RevokedOn = DateTime.UtcNow;
				}
				var tokenValue = Guid.NewGuid().ToString();
				var newRefreshToken = new RefreshToken
				{
					Token = tokenValue,
					CreatedOn = DateTime.UtcNow,
					ExpiresOn = DateTime.UtcNow.AddDays(7),
				};

				user.RefreshToken.Add(newRefreshToken);
				var updated = await _userManager.UpdateAsync(user);

				return new JwtResponseDto
				{
					IsSucceeded = true,
					RefreshToken = tokenValue,
				};
			}
			catch (Exception ex)
			{
				return new JwtResponseDto { IsSucceeded = false, Errors = { "Error Happend with RefreshToken Generating" + ex.Message } };
			}
		}

		public async Task<JwtResponseDto> FindEmailByRefreshToken(string refreshToken)
		{
			if (string.IsNullOrEmpty(refreshToken))
				return new JwtResponseDto { IsSucceeded = false, Errors = { "RefreshToken Cant Be Null" } };
			try
			{
				var user = await _userManager.Users
				   .Include(u => u.RefreshToken)
				   .FirstOrDefaultAsync(x => x.RefreshToken.Any(z => z.Token == refreshToken));

				if (user == null)
					return new JwtResponseDto { IsSucceeded = false, Errors = { "User Not Found" } };

				var token = user.RefreshToken?.FirstOrDefault(z => z.Token == refreshToken);
				if (token == null)
				{
					return new JwtResponseDto
					{
						IsSucceeded = false,
						Message = "Token not found"
					};
				}

				if (!token.IsActive)
				{
					return new JwtResponseDto
					{
						IsSucceeded = false,
						Message = "Token is no longer active"
					};
				}
				return new JwtResponseDto { IsSucceeded = true, Email = user.Email };
			}
			catch (Exception ex)
			{
				return new JwtResponseDto { IsSucceeded = false, Errors = { ex.Message } };
			}
		}

		public async Task<JwtResponseDto> RevokeAllRefreshTokensByEmailAsync(string email)
		{
			if (string.IsNullOrEmpty(email))
				return new JwtResponseDto { IsSucceeded = false, Errors = { "Email cannot be null or empty" } };

			try
			{
				var user = await _userManager.Users
					.Include(u => u.RefreshToken)
					.FirstOrDefaultAsync(x => x.Email == email);

				if (user == null)
					return new JwtResponseDto { IsSucceeded = false, Errors = { "No user found with this email" } };

				var tokens = user.RefreshToken.ToList();

				if (!tokens.Any())
				{
					return new JwtResponseDto { IsSucceeded = true, Message = "No refresh tokens found for this user" };
				}

				foreach (var token in tokens)
				{
					// Revoke all tokens by setting IsActive to false and adding a revoked date
					token.RevokedOn = DateTime.UtcNow;
				}

				await _userManager.UpdateAsync(user);

				return new JwtResponseDto { IsSucceeded = true, Message = "All refresh tokens have been revoked" };
			}
			catch (Exception ex)
			{
				return new JwtResponseDto { IsSucceeded = false, Errors = { "Error occurred while revoking tokens: " + ex.Message } };
			}
		}
		public async Task<JwtResponseDto> RevokeRefreshTokenAsync(string refreshToken)
		{
			if (string.IsNullOrEmpty(refreshToken))
				return new JwtResponseDto { IsSucceeded = false, Errors = { "Refresh token cannot be null or empty" } };

			try
			{
				var user = await _userManager.Users
					.Include(u => u.RefreshToken)
					.FirstOrDefaultAsync(x => x.RefreshToken.Any(z => z.Token == refreshToken));

				if (user == null)
					return new JwtResponseDto { IsSucceeded = false, Errors = { "No user associated with this refresh token" } };

				var foundToken = user.RefreshToken.FirstOrDefault(z => z.Token == refreshToken);

				if (foundToken == null)
					return new JwtResponseDto { IsSucceeded = false, Errors = { "Refresh token not found" } };

				foundToken.RevokedOn = DateTime.UtcNow;
				await _userManager.UpdateAsync(user);

				return new JwtResponseDto { IsSucceeded = true, Message = "Logged out successfully" };
			}
			catch (Exception ex)
			{
				return new JwtResponseDto { IsSucceeded = false, Errors = { "Error occurred while revoking tokens: " + ex.Message } };
			}
		}




	}
}