using DripOut.Application.DTOs.Account;
using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces
{
	public interface IJWTService
	{
		Task<JwtResponseDto> GenerateJWTTokenAsync(string email);
		Task<JwtResponseDto> GenerateRefreshTokenAsync(string email);
		Task<JwtResponseDto> FindEmailByRefreshToken(string refreshToken);
		Task<JwtResponseDto> RevokeAllRefreshTokensByEmailAsync(string email);
		Task<JwtResponseDto> RevokeRefreshTokenAsync(string refreshToken);
	}
}
