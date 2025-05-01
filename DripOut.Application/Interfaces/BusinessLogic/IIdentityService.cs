using DripOut.Application.DTOs.Account;
using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.BussinessLogic
{
	public interface IIdentityService
	{
		Task<IdentityDto> CreateUserAsync(RegisterDto model, string role);
		Task<IdentityDto> ValidateUserCredentialsAsync(LoginDto model);
		Task<IdentityDto> FindUserByEmailAsync(string email);
	}
}
