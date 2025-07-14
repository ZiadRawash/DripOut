using DripOut.Application.Common;
using DripOut.Application.DTOs.Account;
using DripOut.Application.Interfaces;
using DripOut.Application.Interfaces.Services;
using DripOut.Domain.Consts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
		private readonly IMailService _mailService;


		public AuthenticationService(IIdentityService identityService, IJWTService iJWTServicee, IMailService mailService)
		{
			_iJWTService = iJWTServicee;
			_identityService = identityService;
			_mailService = mailService;

		}
		public async Task<Result<AuthReturnDto>> VerifyUser(string email, string code)
		{
			var verified = await _identityService.verifyConfirmationCode(email, code);
			if (!verified.IsSucceeded)
				return Result<AuthReturnDto>.Failure("Error Happend ", verified.Errors);
			var jwtResult = await _iJWTService.GenerateJWTTokenAsync(email!);
			if (!jwtResult.IsSucceeded)
				return Result<AuthReturnDto>.Failure(jwtResult.Message, jwtResult.Errors);
			var refreshResult = await _iJWTService.GenerateRefreshTokenAsync(email);
			if (!refreshResult.IsSucceeded)
				return Result<AuthReturnDto>.Failure(refreshResult.Message, refreshResult.Errors);
			var auth = new AuthReturnDto { Email = email, RefreshToken = refreshResult.RefreshToken, Token = jwtResult.Token, Role = Roles.User };
			return Result<AuthReturnDto>.Success(auth);

		}
		public async Task<Result<AuthReturnDto>> RegisterAsync(RegisterDto model)
		{

			var createdUser = await _identityService.CreateUserAsync(model, Roles.User);
			if (!createdUser.IsSucceeded)
				return Result<AuthReturnDto>.Failure(createdUser.Errors);
			var result = new AuthReturnDto
			{
				Email = model.Email,
				ConfigCode = createdUser.ConfirmationCode,
			};
			var emailsent = await _mailService.SendConfirmationAsync(model.Email!, "Signing Up Successfully", model.FirstName, model.Email!, createdUser.ConfirmationCode!);
			return Result<AuthReturnDto>.Success(result, "Email Sent Check ur inbox");
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
			var revokeOldToken = await _iJWTService.RevokeAllRefreshTokensByEmailAsync(model.Email!);
			if (!revokeOldToken.IsSucceeded)
				return Result<AuthReturnDto>.Failure(revokeOldToken.Errors);
			var refreshToken = await _iJWTService.GenerateRefreshTokenAsync(model.Email!);
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
				return Result<AuthReturnDto>.Failure(["refreshToken Can not be null"]);
			}

			var userfound = await _iJWTService.FindEmailByRefreshToken(refreshToken);
			if (!userfound.IsSucceeded)
				return Result<AuthReturnDto>.Failure(Errors: userfound.Errors);

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
				return Result.Failure(tokenRevoked.Errors.ToString());
			return Result.Success();
		}
		public async Task<Result<AuthReturnDto>> SigninExternal(string id)
		{
			var validated = await _identityService.ValidateGoogleSignin(id);
			if (!validated.IsSucceeded)
				return Result<AuthReturnDto>.Failure(validated.Errors);

			var emailFound = await _identityService.FindUserByEmailAsync(validated.Email);

			if (!emailFound.IsSucceeded)
			{
				var createdUser = await _identityService.CreateUserAsyncForExternal(validated, Roles.User);
				if (!createdUser.IsSucceeded)
					return Result<AuthReturnDto>.Failure(createdUser.Errors);
			}

			var accessToken = await _iJWTService.GenerateJWTTokenAsync(validated.Email!);
			if (!accessToken.IsSucceeded)
				return Result<AuthReturnDto>.Failure(accessToken.Errors);

			var revokeOldToken = await _iJWTService.RevokeAllRefreshTokensByEmailAsync(validated.Email);
			if (!revokeOldToken.IsSucceeded)
				return Result<AuthReturnDto>.Failure(revokeOldToken.Errors);

			var refreshToken = await _iJWTService.GenerateRefreshTokenAsync(validated.Email);
			if (!refreshToken.IsSucceeded)
				return Result<AuthReturnDto>.Failure(refreshToken.Errors);

			var result = new AuthReturnDto
			{
				Email = validated.Email,
				RefreshToken = refreshToken.RefreshToken,
				Token = accessToken.Token

			};

			return Result<AuthReturnDto>.Success(result, "Logged In Successfully");
		}
		public async Task<Result<AuthReturnDto>> ResendVerificationCodeAsync(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
				return Result<AuthReturnDto>.Failure("Email cannot be null or empty");

			var resendResult = await _identityService.ResendEmailVerificationCodeAsync(email);
			if (!resendResult.IsSucceeded)
				return Result<AuthReturnDto>.Failure("Failed to resend verification code", resendResult.Errors);

			var user = await _identityService.FindUserByEmailAsync(email);
			if (!user.IsSucceeded)
				return Result<AuthReturnDto>.Failure("User not found", user.Errors);

			// Extract first name from email if needed (similar to your existing logic)
			var firstName = email.Split('@')[0];

			var emailSent = await _mailService.SendConfirmationAsync(
				email,
				"Email Verification Code Resent",
				firstName,
				email,
				resendResult.ConfirmationCode!
			);

			if (!emailSent)
				return Result<AuthReturnDto>.Failure("Failed to send verification email");

			var result = new AuthReturnDto
			{
				Email = email,
				ConfigCode = resendResult.ConfirmationCode
			};

			return Result<AuthReturnDto>.Success(result, "Verification code resent successfully. Please check your inbox.");
		}

	}
}