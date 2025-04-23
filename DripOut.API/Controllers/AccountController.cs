using DripOut.Application.DTOs.Account;
using DripOut.Application.Services;
using DripOut.Domain.Consts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AuthenticationService = DripOut.Application.Services.AuthenticationService;

namespace DripOut.API.Controllers
{
	[Route("api/Account")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly AuthenticationService _authService;

		public AccountController(AuthenticationService authService)
		{
			_authService = authService;
		}

		[HttpPost]
		[Route("Register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var created = await _authService.RegisterAsync(model);
			if (created.IsSucceeded)
			{
				return Ok(created.Data);
			}
			else
			{
				return BadRequest(new{ Errors = string.Join(",", created.Errors) });
			}
		}
		[HttpPost]
		[Route("Login")]
		public async Task<IActionResult> Login([FromBody] LoginDto model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			var loggedin = await _authService.LoginAsync(model);
			if (loggedin.IsSucceeded)
			{
				return Ok(new
				{
					refreshToken= loggedin.Data.RefreshToken,
					token=loggedin.Data.Token,

				});
			}
			return BadRequest(loggedin.Message + string.Join(",", loggedin.Errors));

		}
		[HttpPost]
		[Route("GenrateAccessToken")]
		public async Task<IActionResult> GenrateAccessToken([FromBody] string RefreshToken)
		{
			if (RefreshToken == null)
			{
				return BadRequest("The Feild RefreshToken Can't be Empty ");
			}
			else
			{
				var token = await _authService.AccessRefreshToken(RefreshToken);
				if (token.IsSucceeded)
				{
					return Ok(token.Data.Token);
				}
				else
				{
					return BadRequest(
					new
					{
							message = token.Message,
							data = token.Data.Token
					});
					
				}

			}

		}
		[HttpGet("TryTokens")]
		[Authorize]
		public  IActionResult  TryTokens()
		{
			return  Ok(new
			{
				message = "Succeeded"
			});
		}



	}
}

