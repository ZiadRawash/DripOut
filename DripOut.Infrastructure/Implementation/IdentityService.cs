using DripOut.Application.BusinessLogic;
using DripOut.Application.Common.Settings;
using DripOut.Application.DTOs.Account;
using DripOut.Application.Interfaces;
using DripOut.Application.Interfaces.Services;
using DripOut.Domain.Models;
using DripOut.Persistence.Migrations;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DripOut.Infrastructure.Implementation
{
	public class IdentityService : IIdentityService
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly Authentication_google _googlesettings;

		public IdentityService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager , IOptions<Authentication_google> Authentication_google)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_googlesettings = Authentication_google.Value;
		}
		public async Task<IdentityDto> CreateUserAsync(RegisterDto model, string role)
		{
			try
			{
				var user = new AppUser
				{
					UserName = Guid.NewGuid().ToString(),
					Email = model.Email,
					FirstName = string.IsNullOrWhiteSpace(model.FirstName) ? model.Email!.Split('@')[0] : model.FirstName,
					LastName = string.IsNullOrWhiteSpace(model.FirstName) ? " " : model.LastName, 
				};

				var result = await _userManager.CreateAsync(user, model.Password!);
				if (result.Succeeded)
				{
					var roleSuccess = await _userManager.AddToRoleAsync(user, role);
					if (roleSuccess.Succeeded)
					{
						var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
						return new IdentityDto { Email = user.Email, ConfirmationCode=code, IsSucceeded = true };
					}
					return new IdentityDto { IsSucceeded = false, Errors = roleSuccess.Errors.Select(x => x.Description).ToList() };
				}
				return new IdentityDto { IsSucceeded = false, Errors = result.Errors.Select(e => e.Description).ToList() };
			}
			catch (Exception ex)
			{
				return new IdentityDto { IsSucceeded = false, Errors = { ex.Message } };
			}

		}

		public async Task<IdentityDto> CreateUserAsyncForExternal(GooglePayloadDto model, string role) {

			try
			{

				var user = new AppUser
				{
					UserName = Guid.NewGuid().ToString(),
					Email = model.Email,
					FirstName = string.IsNullOrWhiteSpace(model.GivenName) ? model.Email.Split('@')[0] : model.GivenName,
					LastName = model.FamilyName,
					EmailConfirmed = true,
					Image = new Image
					{
						ImageUrl = model.Picture
					}
				};

				string mixedGuid = Guid.NewGuid().ToString();
				mixedGuid = mixedGuid[..(mixedGuid.Length / 2)].ToUpper() + mixedGuid[(mixedGuid.Length / 2)..];

				var result = await _userManager.CreateAsync(user, mixedGuid);
				if (result.Succeeded)
				{
					var loginInfo = new UserLoginInfo("Google", model.GoogleId, "Google"); 
					var loginResult = await _userManager.AddLoginAsync(user, loginInfo);
					if (!loginResult.Succeeded)
						return new IdentityDto { IsSucceeded = false, Errors = loginResult.Errors.Select(e => e.Description).ToList() };
					var roleSuccess = await _userManager.AddToRoleAsync(user, role);

					if (roleSuccess.Succeeded)
					{
						
						return new IdentityDto { Email = user.Email, IsSucceeded = true };
					}
					return new IdentityDto { IsSucceeded = false, Errors = roleSuccess.Errors.Select(x => x.Description).ToList() };
				}
				return new IdentityDto { IsSucceeded = false, Errors = result.Errors.Select(e => e.Description).ToList() };
			}
			catch (Exception ex)
			{
				return new IdentityDto { IsSucceeded = false, Errors = { ex.Message } };
			}



		}
		public async Task<IdentityDto> ValidateUserCredentialsAsync(LoginDto model)
		{
			try
			{
				
				var found = await _userManager.FindByEmailAsync(model.Email!);
				if (found == null)
					return new IdentityDto { IsSucceeded = false, Errors = { "Invalid email or password" } };

				if (!found.EmailConfirmed)
				{
					return new IdentityDto { IsSucceeded = false, Errors = { "Email not confirmed yet." } };
				}

				var result = await _signInManager.CheckPasswordSignInAsync(found, model.Password!, false);

				if (result.Succeeded)
				{ return new IdentityDto { IsSucceeded = true, Email = model.Email }; }
				else
				{ return new IdentityDto { IsSucceeded = false, Errors = { "An error occurred while Loging in" } }; }

			}
			catch (Exception ex)
			{
				return new IdentityDto
				{
					IsSucceeded = false,
					Errors = { "An error occurred while Loging in " + ex.Message }
				};
			}
		}
		public async Task<IdentityDto> verifyConfirmationCode(string email, string code) {
		var found=await _userManager.FindByEmailAsync(email);
			if (found == null) return new IdentityDto { IsSucceeded = false };
			var confirmed = await _userManager.ConfirmEmailAsync(found, code);
			if (confirmed.Succeeded)
				return new IdentityDto { IsSucceeded = true };
			return
			new IdentityDto { IsSucceeded = false, Errors = confirmed.Errors.Select(x=>x.Description).ToList() };
		}	
		public async Task<IdentityDto> FindUserByEmailAsync(string email)
		{
			if (string.IsNullOrEmpty(email))
				return new IdentityDto { IsSucceeded = false, Errors = { "Email Can Not Be Null" } };

			try
			{
				
				var found = await _userManager.FindByEmailAsync(email);
				if (found != null)
				{
					return new IdentityDto
					{
						IsSucceeded = true,
						Email = found.Email
					};
				}
				return new IdentityDto
				{
					IsSucceeded = false,
					Errors = { "User not found." }
				};
			}

			catch (Exception ex)
			{
				return new IdentityDto
				{
					IsSucceeded = false,
					Errors = { ex.Message }
				};
			}
		}
		public async Task<GooglePayloadDto> ValidateGoogleSignin(string id)
		{
			try
			{ if (string.IsNullOrWhiteSpace(id))
					return new GooglePayloadDto { IsSucceeded = false ,Errors = { "empty string" } };
				var settings = new GoogleJsonWebSignature.ValidationSettings()
				{
					Audience = new[] { _googlesettings.Webclient_id , _googlesettings.Androidclient_id }
				};
				var payload = await GoogleJsonWebSignature.ValidateAsync(id, settings);
				if (payload != null)
				{
					return new GooglePayloadDto { 
						IsSucceeded = true,
						Message = "Validated",
						Email = payload.Email,
						GivenName = payload.GivenName,
						FamilyName = payload.FamilyName,
						GoogleId = payload.Subject,
						Picture = payload.Picture,
						
					};
				}
				return new GooglePayloadDto
				{ IsSucceeded = false };
			}
			catch (Exception ex)
			{
				return new GooglePayloadDto
				{
					IsSucceeded = false,
					Errors = {  ex.Message }
				};
			}
		}

    }
}