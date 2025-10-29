using DripOut.Application.Common;
using DripOut.Application.DTOs;
using DripOut.Application.DTOs.Account;
using DripOut.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DripOut.API.Controllers
{
	[Route("api/accounts")]
	[ApiController]
	public class AccountsController : ControllerBase
	{
		private readonly IAuthenticationService _authService;

		public AccountsController(IAuthenticationService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		public async Task<ActionResult<ApiResponse>> RegisterAsync([FromBody] RegisterDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(new ApiResponse<TokenWithRefreshtokenDTO> { Success = false, Errors = new() { "Invalid request data" } });

			var result = await _authService.RegisterAsync(dto);
			if (!result.IsSucceeded)
				return BadRequest(new ApiResponse { Success = false, Message = result.Message, Errors = result.Errors });

			return Ok(new ApiResponse { Success = true, Message = "Verification email sent successfully" });
		}

		[HttpPost("verify")]
		public async Task<ActionResult<ApiResponse<TokenWithRefreshtokenDTO>>> VerifyAsync([FromBody] VerifyCodeDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(new ApiResponse { Success = false, Errors = new() { "Invalid request data" } });

			var result = await _authService.VerifyUser(dto.email, dto.code);
			if (!result.IsSucceeded)
				return Unauthorized(new ApiResponse { Success = false, Message = result.Message, Errors = result.Errors });

			return Ok(new ApiResponse<TokenWithRefreshtokenDTO>
			{
				Success = true,
				Message = result.Message,
				Data = new TokenWithRefreshtokenDTO
				{
					RefreshToken = result.Data!.RefreshToken!,
					Token = result.Data.Token!
				}
			});
		}

		[HttpPost("login")]
		public async Task<ActionResult<ApiResponse<TokenWithRefreshtokenDTO>>> LoginAsync([FromBody] LoginDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(new ApiResponse { Success = false, Errors = new() { "Invalid request data" } });

			var result = await _authService.LoginAsync(dto);
			if (!result.IsSucceeded)
				return BadRequest(new ApiResponse<TokenWithRefreshtokenDTO> { Success = false, Message = result.Message, Errors = result.Errors });

			return Ok(new ApiResponse<TokenWithRefreshtokenDTO>
			{
				Success = true,
				Message = "Login successful",
				Data = new TokenWithRefreshtokenDTO
				{
					Token = result.Data!.Token!,
					RefreshToken = result.Data!.RefreshToken!,
				}
			});
		}

		[HttpPost("refresh-token")]
	
		public async Task<ActionResult<ApiResponse<TokenDto>>> RefreshTokenAsync([FromBody] RefreshTokenDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(new ApiResponse { Success = false, Errors = new() { "Invalid request data" } });

			var result = await _authService.AccessRefreshToken(dto.refreshToken);
			if (!result.IsSucceeded)
				return Unauthorized(new ApiResponse { Success = false, Errors = result.Errors });

			return Ok(new ApiResponse<TokenDto>
			{
				Success = true,
				Message = "Access token generated successfully",
				Data = new TokenDto { Token=result.Data!.Token!}
			});
		}

		[HttpPost("logout")]
		public async Task<ActionResult<ApiResponse>> LogoutAsync([FromBody] RefreshTokenDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(new ApiResponse { Success = false, Errors = new() { "Invalid request data" } });

			var result = await _authService.LogOutAsync(dto.refreshToken);
			if (!result.IsSucceeded)
				return BadRequest(new ApiResponse { Success = false, Message = "Logout failed" });

			return Ok(new ApiResponse<TokenWithRefreshtokenDTO> { Success = true, Message = "Logout successful" }
			);
		}

		[HttpPost("google-signin")]
		public async Task<ActionResult<ApiResponse<TokenWithRefreshtokenDTO>>> GoogleSigninAsync([FromBody] GoogleSignupTokenDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(new ApiResponse { Success = false, Errors = new() { "Invalid request data" } });

			var result = await _authService.SigninExternal(dto.idToken);
			if (!result.IsSucceeded)
				return Unauthorized(new ApiResponse { Success = false, Errors = result.Errors });

			return Ok(new ApiResponse<TokenWithRefreshtokenDTO>
			{
				Success = true,
				Message = "Google sign-in successfuly",
				Data = new TokenWithRefreshtokenDTO
				{
					Token = result.Data!.Token!,
					RefreshToken = result.Data.RefreshToken!
				}
			});
		}

		[HttpPost("resend-verification")]
		public async Task<ActionResult<ApiResponse>> ResendVerificationAsync([FromBody] EmailDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(new ApiResponse { Success = false, Errors = new() { "Invalid request data" } });

			var result = await _authService.ResendVerificationCodeAsync(dto.email);
			if (!result.IsSucceeded)
				return BadRequest(new ApiResponse { Success = false, Message = result.Message, Errors = result.Errors });

			return Ok(new ApiResponse { Success = true, Message = result.Message });
		}
	}
}
