using DripOut.Application.DTOs.Account;
using DripOut.Application.Interfaces.Services;
using DripOut.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace DripOut.Infrastructure
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
					UserName = model.Email,
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
						return new IdentityDto { User = user, IsSucceeded = true };
					}
					return new IdentityDto { IsSucceeded = false, Errors = roleSuccess.Errors.Select(e => e.Description) };
				}
				return new IdentityDto { IsSucceeded = false, Errors = result.Errors.Select(e => e.Description) };
			}
			catch (Exception ex)
			{
				return new IdentityDto { IsSucceeded = false, Errors = new List<string> { ex.Message } };
			}

		}
		public async Task<IdentityDto> ValidateUserCredentialsAsync(LoginDto model)
		{
			try
			{
				var result = await _signInManager.PasswordSignInAsync(model.Email!,model.Password!, isPersistent: false, lockoutOnFailure: false);
				if (result.Succeeded) { return new IdentityDto { IsSucceeded = true }; }
				else { return new IdentityDto { IsSucceeded = false  }; }
			}
			catch (Exception ex)
			{
				return new IdentityDto
				{
					IsSucceeded = false,
					Errors = new List<string> { ex.Message }
				};
			}
		}
		public async Task<IdentityDto> FindUserByEmailAsync(RegisterDto model)
		{
			try
			{
				var found = await _userManager.FindByEmailAsync(model.Email!);
				if (found is not null)
				{
					return new IdentityDto { IsSucceeded = true };
				}
				else { return new IdentityDto {User=found, IsSucceeded = true }; }
			}
			catch (Exception ex)
			{
				return new IdentityDto
				{
					IsSucceeded = false,
					Errors = new List<string> { ex.Message }
				};
			}
		}
	}
}
