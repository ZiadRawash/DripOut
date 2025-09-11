using DripOut.Application.Common;
using DripOut.Application.DTOs.Governorate;
using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.Services
{
	public interface IGovernorateService
	{
		Task<Result<IEnumerable<GovernorateDto>>> GetGovernoratesAsync(bool? isActive = null);
		Task<Result<GovernorateDto>> GetGovernorateByIdAsync(int id);
		Task<Result<GovernorateDto>> CreateGovernorateAsync(CreateGovernorateDto createDto);
		Task<Result> UpdateGovernorateAsync(int id, UpdateGovernorateDto updateDto);
		Task<Result<IEnumerable<GovernorateWithOrdersDTO>>> GetGovernorateOrdersAsync(int id);
		Task<Result> UpdateShippingCostAsync(int id, decimal shippingCost);
		Task<Result> UpdateDiscountPercentageAsync(int id, decimal discountPercentage);
		Task<Result> ActivateGovernorateAsync(int id);
		Task<Result> DeactivateGovernorateAsync(int id);
	}
}
