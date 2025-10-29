using DripOut.Application.Common;
using DripOut.Application.DTOs.Governorate;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Application.Interfaces.Services;
using DripOut.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.ApplicationServices
{
	public class GovernorateService:IGovernorateService
	{
		private readonly IUnitOfWork _unitOfWork;
		public GovernorateService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public async Task<Result<IEnumerable<GovernorateDto>>> GetGovernoratesAsync(bool? isActive = null)
		{
			try
			{
				IEnumerable<Governorate> governorates;

				if (isActive.HasValue)
					governorates = await _unitOfWork.Governorates.GetAllAsync(x => x.IsActive == isActive.Value);
				else
					governorates = await _unitOfWork.Governorates.GetAllAsync();

				if (!governorates.Any())
				{
					return Result<IEnumerable<GovernorateDto>>.Success(
						Enumerable.Empty<GovernorateDto>(),
						"No governorates found"
					);
				}

				var governorateDtos = governorates.Select(g => new GovernorateDto
				{
					Id = g.Id,
					Name = g.Name,
					ShippingCost = g.ShippingCost,
					MinimumOrderForFreeShipping = g.Threshold,
					DiscountPercentage = g.DiscountPercentage,
					IsActive = g.IsActive,

				}).ToList();

				return Result<IEnumerable<GovernorateDto>>.Success(governorateDtos, "Governorates retrieved successfully");
			}
			catch (Exception ex)
			{
				return Result<IEnumerable<GovernorateDto>>.Failure(
					"An error occurred while retrieving governorates",
					new List<string> { ex.Message });
			}
		}


		public async Task<Result<GovernorateDto>> GetGovernorateByIdAsync(int id)
		{
			try
			{
				if (id <= 0)
				{
					return Result<GovernorateDto>.Failure("Invalid governorate ID",
						new List<string> { "ID must be greater than 0" });
				}

				var governorate = await _unitOfWork.Governorates.FindAsync(id);

				if (governorate == null)
				{
					return Result<GovernorateDto>.Failure($"Governorate with ID {id} not found");
				}

				var governorateDto = new GovernorateDto
				{
					Id = governorate.Id,
					Name = governorate.Name,
					ShippingCost = governorate.ShippingCost,
					MinimumOrderForFreeShipping = governorate.Threshold,
					DiscountPercentage = governorate.DiscountPercentage,
					IsActive=governorate.IsActive,

				};

				return Result<GovernorateDto>.Success(governorateDto, "Governorate retrieved successfully");
			}
			catch (Exception ex)
			{
				return Result<GovernorateDto>.Failure("An error occurred while retrieving the governorate",
					new List<string> { ex.Message });
			}
		}
		public async Task<Result<GovernorateDto>> CreateGovernorateAsync(CreateGovernorateDto createDto)
		{
			try
			{

				var existingGovernorate = await _unitOfWork.Governorates
					.FindAsync(g => g.Name.ToLower() == createDto.Name.ToLower());

				if (existingGovernorate!=null)
				{
					return Result<GovernorateDto>.Failure($"Governorate with name '{createDto.Name}' already exists");
				}

				var governorate = new Governorate
				{
					Name = createDto.Name.Trim(),
					ShippingCost = createDto.ShippingCost,
					Threshold = createDto.MinimumOrderForFreeShipping,
					DiscountPercentage = createDto.DiscountPercentage,
					IsActive=true,
					
				};

				await _unitOfWork.Governorates.AddAsync(governorate);
				await _unitOfWork.SaveChangesAsync();

				var governorateDto = new GovernorateDto
				{
					Id = governorate.Id,
					Name = governorate.Name,
					ShippingCost = governorate.ShippingCost,
					MinimumOrderForFreeShipping = governorate.Threshold,
					DiscountPercentage = governorate.DiscountPercentage
				};

				return Result<GovernorateDto>.Success(governorateDto, "Governorate created successfully");
			}
			catch (Exception ex)
			{
				return Result<GovernorateDto>.Failure("An error occurred while creating the governorate",
					new List<string> { ex.Message });
			}
		}
		public async Task<Result> UpdateGovernorateAsync(int id, UpdateGovernorateDto updateDto)
		{
			try
			{
				if (id <= 0)
				{
					return Result.Failure("Invalid governorate ID",
						new List<string> { "ID must be greater than 0" });
				}


				var governorate = await _unitOfWork.Governorates.FindAsync(id);
				if (governorate == null)
				{
					return Result.Failure($"Governorate with ID {id} not found");
				}


				var existingGovernorate = await _unitOfWork.Governorates
					.FindAsync(g => g.Name.ToLower() == updateDto.Name.ToLower() && g.Id != id);

				if (existingGovernorate != null)
				{
					return Result.Failure($"Another governorate with name '{updateDto.Name}' already exists");
				}

				governorate.Name = updateDto.Name.Trim();
				governorate.ShippingCost = updateDto.ShippingCost;
				governorate.Threshold = updateDto.MinimumOrderForFreeShipping;
				governorate.DiscountPercentage = updateDto.DiscountPercentage;

				await _unitOfWork.Governorates.UpdateAsync(governorate);
				await _unitOfWork.SaveChangesAsync();

				return Result.Success("Governorate updated successfully");
			}
			catch (Exception ex)
			{
				return Result.Failure("An error occurred while updating the governorate",
					new List<string> { ex.Message });
			}
		}
		public async Task<Result> UpdateDiscountPercentageAsync(int id, decimal discountPercentage)
		{
			try
			{
				if (id <= 0)
				{
					return Result.Failure("Invalid governorate ID",
						new List<string> { "ID must be greater than 0" });
				}

				if (discountPercentage < 0 || discountPercentage > 100)
				{
					return Result.Failure("Invalid discount percentage",
						new List<string> { "Discount percentage must be between 0 and 100" });
				}

				var governorate = await _unitOfWork.Governorates.FindAsync(id);
				if (governorate == null)
				{
					return Result.Failure($"Governorate with ID {id} not found");
				}

				governorate.DiscountPercentage = discountPercentage;
				await _unitOfWork.Governorates.UpdateAsync(governorate);
				await _unitOfWork.SaveChangesAsync();

				return Result.Success("Discount percentage updated successfully");
			}
			catch (Exception ex)
			{
				return Result.Failure("An error occurred while updating discount percentage",
					new List<string> { ex.Message });
			}
		}
		public async Task<Result<IEnumerable<GovernorateWithOrdersDTO>>> GetGovernorateOrdersAsync(int id)
		{
			try
			{
				if (id <= 0)
				{
					return Result<IEnumerable<GovernorateWithOrdersDTO>>.Failure("Invalid governorate ID",
						new List<string> { "ID must be greater than 0" });
				}

				var governorate = await _unitOfWork.Governorates.FindAsync(y=>y.Id==id, x=>x.Orders);
				if (governorate == null)
				{
					return Result<IEnumerable<GovernorateWithOrdersDTO>>.Failure($"Governorate with ID {id} not found");
				}

				var orders = governorate.Orders.Select(o =>new  GovernorateWithOrdersDTO
				{
					date=o.OrderDate,
					GovernorateName=o.Governorate.Name,
					OrderId=o.Id,
					Status=o.Status,
					TotalCost=o.TotalCost,
				}).ToList();

				return Result<IEnumerable<GovernorateWithOrdersDTO>>.Success(orders, "Orders retrieved successfully");
			}
			catch (Exception ex)
			{
				return Result<IEnumerable<GovernorateWithOrdersDTO>>.Failure("An error occurred while retrieving governorate orders",
					new List<string> { ex.Message });
			}
		}

		public async Task<Result> UpdateShippingCostAsync(int id, decimal shippingCost)
		{
			try
			{
				if (id <= 0)
				{
					return Result.Failure("Invalid governorate ID",
						new List<string> { "ID must be greater than 0" });
				}

				if (shippingCost < 0)
				{
					return Result.Failure("Invalid shipping cost",
						new List<string> { "Shipping cost cannot be negative" });
				}

				var governorate = await _unitOfWork.Governorates.FindAsync(id);
				if (governorate == null)
				{
					return Result.Failure($"Governorate with ID {id} not found");
				}

				governorate.ShippingCost = shippingCost;
				await _unitOfWork.Governorates.UpdateAsync(governorate);
				await _unitOfWork.SaveChangesAsync();

				return Result.Success("Shipping cost updated successfully");
			}
			catch (Exception ex)
			{
				return Result.Failure("An error occurred while updating shipping cost",
					new List<string> { ex.Message });
			}
		}
		public async Task<Result> ActivateGovernorateAsync(int id)
		{
			try
			{
				if (id <= 0)
					return Result.Failure("Invalid governorate ID", new List<string> { "ID must be greater than 0" });

				var governorate = await _unitOfWork.Governorates.FindAsync(id);
				if (governorate == null)
					return Result.Failure($"Governorate with ID {id} not found");

				if (governorate.IsActive)
					return Result.Success("Governorate is already active");

				governorate.IsActive = true;
				await _unitOfWork.Governorates.UpdateAsync(governorate);
				await _unitOfWork.SaveChangesAsync();

				return Result.Success("Governorate activated successfully");
			}
			catch (Exception ex)
			{
				return Result.Failure("An error occurred while activating governorate",
					new List<string> { ex.Message });
			}
		}

		public async Task<Result> DeactivateGovernorateAsync(int id)
		{
			try
			{
				if (id <= 0)
					return Result.Failure("Invalid governorate ID", new List<string> { "ID must be greater than 0" });

				var governorate = await _unitOfWork.Governorates.FindAsync(id);
				if (governorate == null)
					return Result.Failure($"Governorate with ID {id} not found");

				if (!governorate.IsActive)
					return Result.Success("Governorate is already inactive");

				governorate.IsActive = false;
				await _unitOfWork.Governorates.UpdateAsync(governorate);
				await _unitOfWork.SaveChangesAsync();

				return Result.Success("Governorate deactivated successfully");
			}
			catch (Exception ex)
			{
				return Result.Failure("An error occurred while deactivating governorate",
					new List<string> { ex.Message });
			}
		}

	}
}
