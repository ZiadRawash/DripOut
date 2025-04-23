using DripOut.Application.DTOs.Account;
using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.Services
{
	public interface IJWTService
	{
		Task<IdentityDto> GenerateJWTTokenAsync(string email);
		Task <IdentityDto> GenerateRefreshTokenAsync(string email);
		Task<IdentityDto> RevokeRefreshTokenAsync(string RefreshToken);
		Task<IdentityDto> FindEmailByRefreshToken(string RefreshToken);
	}
}
