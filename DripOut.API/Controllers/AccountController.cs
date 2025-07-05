using DripOut.Application.DTOs.Account;
using DripOut.Application.BusinessLogic;
using DripOut.Domain.Consts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AuthenticationService = DripOut.Application.BusinessLogic.AuthenticationService;
using DripOut.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DripOut.API.Controllers
{
	[Route("api/Account")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly AuthenticationService _authService;

		public AccountController(AuthenticationService authService )
		{
			_authService = authService;
		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var created = await _authService.RegisterAsync(model);
			if (created.IsSucceeded)
			{
				return Ok(new
				{
					Message = "Email Sent Successfully"
				});
			}
			else
			{
				return BadRequest(new { errors = created.Errors });
			}
		}
		[HttpPost("Verify")]
		public async Task<IActionResult> VerifyCode( string email,  string code)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var confirmed = await _authService.VerifyUser(email, code);
			if (confirmed.IsSucceeded) {
				return Ok(new
				{
					Token = confirmed.Data!.Token,
					RefreshToken = confirmed.Data!.RefreshToken
				}
				);

			}
			else
			{

			return BadRequest(new { errors = confirmed.Errors });
				
			}
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login([FromBody] LoginDto model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			var loggedin = await _authService.LoginAsync(model);
			if (loggedin.IsSucceeded)
			{
				return Ok(new
				{
					refreshToken= loggedin.Data!.RefreshToken,
					token=loggedin.Data.Token,

				});
			}
			return BadRequest( new{ message = loggedin.Message + string.Join(",", loggedin.Errors)});

		}
		[HttpPost]
		[Route("GenrateAccessToken")]
		public async Task<IActionResult> GenrateAccessToken([FromBody] string RefreshToken)
		{
			if (RefreshToken != null)
			{
				var result = await _authService.AccessRefreshToken(RefreshToken);
				if (!result.IsSucceeded)
				{
					return BadRequest(
					new
					{
						message = result.Errors
					});
				}
				return Ok(new
				{
					token = result.Data!.Token
				});
			}
			return BadRequest(new { error = "RefreshToken Can't be NULL" });
		}

		[HttpPost]
		[Route("LogOut")]
		public async Task<IActionResult> LogOut([FromBody] string RefreshToken)
		{

			if (RefreshToken == null)
				return BadRequest(new { error = "RefreshToken Can not be Null" });
			else
			{
				var result = await _authService.LogOutAsync(RefreshToken!);
				if (!result.IsSucceeded)
					return BadRequest(new { message = "failure" });
				return Ok(new { message = "success" });

			}

		}	
		[HttpPost("Google-signin")]
		public async Task<IActionResult> Google_signin([FromBody] GoogleSignupTokenDto model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _authService.SigninExternal(model.idToken);

			if (!result.IsSucceeded)
				return Unauthorized(new { Errors = result.Errors });

			return Ok(
				new
				{
					token = result.Data!.Token,
					refreshToken = result.Data.RefreshToken
				}
				
				);
		}
	}
}

