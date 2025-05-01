using DripOut.Application.DTOs.Account;
using DripOut.Application.Interfaces.Services;
using DripOut.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace DripOut.Infrastructure.Implementation
{
	public class IdentityService : IIdentityService
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;

		public IdentityService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}
		public async Task<IdentityDto> CreateUserAsync(RegisterDto model, string role)
		{
			try
			{
				var user = new AppUser
				{
					UserName = Guid.NewGuid().ToString(),
					Email = model.Email,
					FirstName = model.FirstName,
					LastName = model.LastName
				};

				var result = await _userManager.CreateAsync(user, model.Password!);
				if (result.Succeeded)
				{
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
				//var result = await _signInManager.PasswordSignInAsync(model.Email!, model.Password!, isPersistent: false, lockoutOnFailure: false);
				var found = await _userManager.FindByEmailAsync(model.Email!);
				if (found == null)
					return new IdentityDto { IsSucceeded = false, Errors = { "Invalid email or password" } };

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
		public async Task<IdentityDto> FindUserByEmailAsync(string email)
		{
			if (string.IsNullOrEmpty(email))
				return new IdentityDto { IsSucceeded = false, Errors = { "Email Can Not Be Null" } };

			try
			{
				var found = await _userManager.FindByEmailAsync(email);
				if (found != null) return new IdentityDto { IsSucceeded = true };
				else { return new IdentityDto { Email = found!.Email, IsSucceeded = true }; }
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
	}
}