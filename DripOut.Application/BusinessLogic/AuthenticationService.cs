using DripOut.Application.Common;
using DripOut.Application.DTOs.Account;
using DripOut.Application.Interfaces;
using DripOut.Application.Interfaces.Services;
using DripOut.Domain.Consts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DripOut.Application.BusinessLogic
{
	public class AuthenticationService
	{
		private readonly IIdentityService _identityService;
		private readonly IJWTService _iJWTService;

		public AuthenticationService(IIdentityService identityService, IJWTService iJWTServicee)
		{
			_iJWTService = iJWTServicee;
			_identityService = identityService;
		}
		public async Task<Result<AuthReturnDto>> RegisterAsync(RegisterDto model)
		{

			var createdUser = await _identityService.CreateUserAsync(model, Roles.User);
			if (!createdUser.IsSucceeded)
				return Result<AuthReturnDto>.Failure(createdUser.Errors);
			var jwtResult = await _iJWTService.GenerateJWTTokenAsync(model.Email!);
			if (!jwtResult.IsSucceeded)
				return Result<AuthReturnDto>.Failure(jwtResult.Message, jwtResult.Errors);
			var refreshResult = await _iJWTService.GenerateRefreshTokenAsync(model.Email!);
			if (!refreshResult.IsSucceeded)
				return Result<AuthReturnDto>.Failure(refreshResult.Message, refreshResult.Errors);
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
			var logged = await _identityService.ValidateUserCredentialsAsync(model);
			if (!logged.IsSucceeded)
			{
				return Result<AuthReturnDto>.Failure(logged.Errors);
			}
			var accessToken = await _iJWTService.GenerateJWTTokenAsync(model.Email!);
			if (!accessToken.IsSucceeded)
			{
				return Result<AuthReturnDto>.Failure(accessToken.Errors);
			}
			var revokeOldToken = await _iJWTService.RevokeAllRefreshTokensByEmailAsync(model.Email);
			if (!revokeOldToken.IsSucceeded)
				return Result<AuthReturnDto>.Failure(revokeOldToken.Errors);
			var refreshToken = await _iJWTService.GenerateRefreshTokenAsync(model.Email);
			if (!refreshToken.IsSucceeded) { return Result<AuthReturnDto>.Failure(refreshToken.Errors); }

			var result = new AuthReturnDto
			{
				Email = model.Email,
				Password = model.Password,
				Role = Roles.User,
				RefreshToken = refreshToken.RefreshToken,
				Token = accessToken.Token
			};
			return Result<AuthReturnDto>.Success(result, "Logged In Successfully");
		}
		public async Task<Result<AuthReturnDto>> AccessRefreshToken(string refreshToken)
		{
			if (string.IsNullOrEmpty(refreshToken))
			{
				return Result<AuthReturnDto>.Failure(  ["refreshToken Can not be null"]);
			}
			
			var userfound = await _iJWTService.FindEmailByRefreshToken(refreshToken);
			if (!userfound.IsSucceeded)
				return Result<AuthReturnDto>.Failure(Errors:userfound.Errors);

			var token = await _iJWTService.GenerateJWTTokenAsync(userfound.Email!);
			if (!token.IsSucceeded)
			{
				return Result<AuthReturnDto>.Failure(token.Errors);
			}
			var result = new AuthReturnDto
			{	
				Token = token.Token
			};
			return Result<AuthReturnDto>.Success(result, "Token created successfully");
		}



		public async Task<Result> LogOutAsync(string refreshToken)
		{
			if (string.IsNullOrEmpty(refreshToken))
				return Result.Failure(Errors: ["RefreshToken can not be Null"]);
			var tokenRevoked = await _iJWTService.RevokeRefreshTokenAsync(refreshToken);
			if (!tokenRevoked.IsSucceeded)
				return Result.Failure( tokenRevoked.Errors);
			return Result.Success();
		}

	}
}