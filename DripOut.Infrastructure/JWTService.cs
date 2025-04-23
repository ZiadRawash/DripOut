using DripOut.Application.Common.Settings;
using DripOut.Application.DTOs.Account;
using DripOut.Application.Interfaces.Services;
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

namespace DripOut.Infrastructure
{
	public class JWTService : IJWTService
	{
		private readonly JWTSettings _Options;
		private readonly UserManager<AppUser> _UserManager;
		private readonly SymmetricSecurityKey _key;

		public JWTService(IOptions<JWTSettings> Options , UserManager<AppUser> UserManager )
		{
			_UserManager = UserManager;
			_Options = Options.Value;
			_key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Options.SignInKey!));	
		}
		
		public async Task<IdentityDto> GenerateJWTTokenAsync(string email)
		{
			var User =await _UserManager.FindByEmailAsync(email);
			if (User == null) {
				return (new IdentityDto { IsSucceeded = false, Errors = new List<string> { "User Not Found" } });
			}
			try
			{
			var Claims = new List<Claim>{
			new Claim(JwtRegisteredClaimNames.Sub,User.Id),
			new Claim(JwtRegisteredClaimNames.Email,User.Email!)
			};
				var Roles = await _UserManager.GetRolesAsync(User);
				Claims.AddRange(Roles.Select(role => new Claim(ClaimTypes.Role, role)));
				var Creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
				var Descriptor = new SecurityTokenDescriptor
				{
					Subject = new ClaimsIdentity(Claims),
					Audience = _Options.Audiance,
					Issuer = _Options.Issuer,
					Expires = DateTime.UtcNow.AddMinutes(int.Parse(_Options.AccessTokenExpiryInMinutes)),
					SigningCredentials = Creds,
				};
				var TokenHandler = new JwtSecurityTokenHandler();
				var SecToken = TokenHandler.CreateToken(Descriptor);
				var Token = TokenHandler.WriteToken(SecToken);
				return (new IdentityDto { Token = Token, IsSucceeded = true });
			}
			catch (Exception ex)
			{
				return (new IdentityDto { IsSucceeded = false, Errors= new List<string> { ex.Message } });
			}
		}

		public async Task<IdentityDto> GenerateRefreshTokenAsync(string email)
		{
			try
			{
				var user = await _UserManager.FindByEmailAsync(email);
				if (user == null)
				{
					return new IdentityDto
					{
						IsSucceeded = false,
						RefreshToken = null,
						Errors = new List<string> { "User Not Found" }
					};
				}
				var userToken = user.RefreshToken.FirstOrDefault(e => e.IsActive);
				if (userToken != null)
				{
					userToken.RevokedOn = DateTime.UtcNow;
				}
				var Tokenunique = Guid.NewGuid().ToString();
				var newRefreshToken = new RefreshToken
				{
					Token = Tokenunique
				};

				user.RefreshToken.Add(newRefreshToken);
				var updated = await _UserManager.UpdateAsync(user);

				return new IdentityDto
				{
					IsSucceeded = true,
					RefreshToken = Tokenunique
				};
			}
			catch(Exception ex)
			{
				return(new IdentityDto { IsSucceeded = false, Errors = new List<string> { ex.Message } });
			}
		}

		public async Task<IdentityDto> FindEmailByRefreshToken(string refreshToken)
		{
			// First get the user with the refresh token
			var user = await _UserManager.Users
				.Include(u => u.RefreshToken)
				.FirstOrDefaultAsync(x => x.RefreshToken.Any(z => z.Token == refreshToken));

			if (user == null)
			{
				return new IdentityDto { IsSucceeded = false, Message = "Not Found" };
			}

			
			var token = user.RefreshToken.FirstOrDefault(z => z.Token == refreshToken);
			if (token == null || !token.IsActive)
			{
				return new IdentityDto { IsSucceeded = false, Message = "Token not active" };
			}

			return new IdentityDto { Email = user.Email!, IsSucceeded = true, Message = "Found" };
		}


		public async Task<IdentityDto> RevokeRefreshTokenAsync(string refreshToken)
		{
			var user = await _UserManager.Users
				.Include(u => u.RefreshToken)
				.FirstOrDefaultAsync(x => x.RefreshToken.Any(z => z.Token == refreshToken));

			var token = user?.RefreshToken.FirstOrDefault(z => z.Token == refreshToken);

			if (token == null || !token.IsActive)
			{
				return new IdentityDto { IsSucceeded = false, Message = "Token already revoked or invalid" };
			}

			token.RevokedOn = DateTime.UtcNow;
			await _UserManager.UpdateAsync(user!);

			return new IdentityDto { IsSucceeded = true, Message = "Token Revoked" };
		}

	}
}
