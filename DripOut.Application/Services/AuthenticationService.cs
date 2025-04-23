using DripOut.Application.Common;
using DripOut.Application.DTOs.Account;
using DripOut.Application.Interfaces.Services;
using DripOut.Domain.Consts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Services
{
	public class AuthenticationService
	{
		private readonly IIdentityService _identityService;
		private readonly IJWTService _IJWTService;

		public AuthenticationService(IIdentityService identityService, IJWTService ijWTServicee)
		{
			 _IJWTService= ijWTServicee;
			_identityService = identityService;
		}
		public async Task<Result<AuthReturnDto>> RegisterAsync(RegisterDto model)
		{
			
			var created = await _identityService.CreateUserAsync(model, Roles.User);
			if (!created.IsSucceeded)
				return Result<AuthReturnDto>.Failure(created.Errors.ToList());

			
			var jwtResult = await _IJWTService.GenerateJWTTokenAsync(model.Email!);
			if (!jwtResult.IsSucceeded)
				return Result<AuthReturnDto>.Failure("Error generating JWT", jwtResult.Errors.ToList());

			
			var refreshResult = await _IJWTService.GenerateRefreshTokenAsync(model.Email!);
			if (!refreshResult.IsSucceeded)
				return Result<AuthReturnDto>.Failure("Error generating Refresh Token", refreshResult.Errors.ToList());

			
			var result = new AuthReturnDto
			{
				Email = model.Email,
				Password = model.Password,
				Role = Roles.User,
				Token = jwtResult.Token,
				RefreshToken = refreshResult.RefreshToken,
			};

			return Result<AuthReturnDto>.Success(result, "User Created Successfully");
		}

		public async Task<Result<AuthReturnDto>> LoginAsync(LoginDto model)
		{
			var Logged = await _identityService.ValidateUserCredentialsAsync(model);
			if (!Logged.IsSucceeded)
			{
				return Result<AuthReturnDto>.Failure("Logged In Failed Email Or Password Is Wrong");
			}
			var AcessToken = await _IJWTService.GenerateJWTTokenAsync(model.Email);
			if (!AcessToken.IsSucceeded)
			{
				return Result<AuthReturnDto>.Failure("Logged In Failed");
			}
			var RefreshToken = await _IJWTService.GenerateRefreshTokenAsync(model.Email);
			if (!RefreshToken.IsSucceeded) { return Result<AuthReturnDto>.Failure("Logged In Failed"); }

			var result = new AuthReturnDto
			{
				Email = model.Email,
				Password = model.Password,
				Role = Roles.User,
				RefreshToken = RefreshToken.RefreshToken,
				Token = AcessToken.Token
			};
			return Result<AuthReturnDto>.Success(result, "Logged In Successfully");
		}
		public async Task<Result<AuthReturnDto>> AccessRefreshToken(string RefreshToken)
		{
			if (string.IsNullOrEmpty(RefreshToken))
			{
				return Result<AuthReturnDto>.Failure("Refresh token is required.");
			}

			var user = await _IJWTService.FindEmailByRefreshToken(RefreshToken);
			if (user.IsSucceeded)
			{
				var token = await _IJWTService.GenerateJWTTokenAsync(user.Email);
				if (!token.IsSucceeded)
				{
					return Result<AuthReturnDto>.Failure(token.Message);
				}

				var result = new AuthReturnDto
				{
					Token = token.Token
				};
				return Result<AuthReturnDto>.Success(result, "Token created successfully");
			}
			else { return Result<AuthReturnDto>.Failure(user.Message, user.Errors.ToList()); };

		}

	}
}
