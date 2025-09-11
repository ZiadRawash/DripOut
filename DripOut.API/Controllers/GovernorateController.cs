using DripOut.Application.DTOs;
using DripOut.Application.DTOs.Governorate;
using DripOut.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DripOut.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GovernorateController : ControllerBase
	{
		private readonly IGovernorateService _governorateService;
		public GovernorateController(IGovernorateService governorateService)
		{
			_governorateService = governorateService;
			
		}
		[HttpGet]
		public async Task<ActionResult<ApiResponse<IEnumerable<GovernorateDto>>>> GetAllGovernorates([FromQuery]bool? IsActive = null)
		{
			var result = await _governorateService.GetGovernoratesAsync();

			var response = new ApiResponse<IEnumerable<GovernorateDto>>
			{
				Success = result.IsSucceeded,
				Message = result.Message,
				Data = result.Data,
				Errors = result.Errors
			};

			if (result.IsSucceeded)
				return Ok(response);

			return BadRequest(response);
		}



		// GET: api/Governorate/5
		[HttpGet("{id}")]
		public async Task<ActionResult<ApiResponse<GovernorateDto>>> GetGovernorate(int id)
		{
			var result = await _governorateService.GetGovernorateByIdAsync(id);

			var response = new ApiResponse<GovernorateDto>
			{
				Success = result.IsSucceeded,
				Message = result.Message,
				Data = result.Data,
				Errors = result.Errors
			};

			if (result.IsSucceeded)
				return Ok(response);

			if (result.Message?.Contains("not found") == true)
				return NotFound(response);

			return BadRequest(response);
		}

		// POST: api/Governorate
		[HttpPost]
		public async Task<ActionResult<ApiResponse<GovernorateDto>>> CreateGovernorate([FromBody] CreateGovernorateDto createDto)
		{
			if (!ModelState.IsValid)
			{
				var response = new ApiResponse<GovernorateDto>
				{
					Success = false,
					Message = "Validation failed",
					Errors = ModelState.Values
						.SelectMany(v => v.Errors)
						.Select(e => e.ErrorMessage)
						.ToList()
				};
				return BadRequest(response);
			}

			var result = await _governorateService.CreateGovernorateAsync(createDto);

			var apiResponse = new ApiResponse<GovernorateDto>
			{
				Success = result.IsSucceeded,
				Message = result.Message,
				Data = result.Data,
				Errors = result.Errors
			};

			if (result.IsSucceeded)
				return CreatedAtAction(nameof(GetGovernorate), new { id = result.Data?.Id }, apiResponse);

			if (result.Message?.Contains("already exists") == true)
				return Conflict(apiResponse);

			return BadRequest(apiResponse);
		}

		// PUT: api/Governorate/5
		[HttpPut("{id}")]
		public async Task<ActionResult<ApiResponse>> UpdateGovernorate(int id, [FromBody] UpdateGovernorateDto updateDto)
		{
			if (!ModelState.IsValid)
			{
				var response = new ApiResponse
				{
					Success = false,
					Message = "Validation failed",
					Errors = ModelState.Values
						.SelectMany(v => v.Errors)
						.Select(e => e.ErrorMessage)
						.ToList()
				};
				return BadRequest(response);
			}

			var result = await _governorateService.UpdateGovernorateAsync(id, updateDto);

			var apiResponse = new ApiResponse
			{
				Success = result.IsSucceeded,
				Message = result.Message,
				Errors = result.Errors
			};

			if (result.IsSucceeded)
				return Ok(apiResponse);

			if (result.Message?.Contains("not found") == true)
				return NotFound(apiResponse);

			if (result.Message?.Contains("already exists") == true)
				return Conflict(apiResponse);

			return BadRequest(apiResponse);
		}

		// GET: api/Governorate/5/orders
		[HttpGet("{id}/orders")]
		public async Task<ActionResult<ApiResponse<IEnumerable<GovernorateWithOrdersDTO>>>> GetGovernorateOrders(int id)
		{
			var result = await _governorateService.GetGovernorateOrdersAsync(id);

			var response = new ApiResponse<IEnumerable<GovernorateWithOrdersDTO>>
			{
				Success = result.IsSucceeded,
				Message = result.Message,
				Data = result.Data,
				Errors = result.Errors
			};

			if (result.IsSucceeded)
				return Ok(response);

			if (result.Message?.Contains("not found") == true)
				return NotFound(response);

			return BadRequest(response);
		}

		// PATCH: api/Governorate/5/shipping-cost
		[HttpPatch("{id}/shipping-cost")]
		public async Task<ActionResult<ApiResponse>> UpdateShippingCost(int id, [FromBody] UpdateShippingCostDto request)
		{
			if (!ModelState.IsValid)
			{
				var response = new ApiResponse
				{
					Success = false,
					Message = "Validation failed",
					Errors = ModelState.Values
						.SelectMany(v => v.Errors)
						.Select(e => e.ErrorMessage)
						.ToList()
				};
				return BadRequest(response);
			}

			var result = await _governorateService.UpdateShippingCostAsync(id, request.ShippingCost);

			var apiResponse = new ApiResponse
			{
				Success = result.IsSucceeded,
				Message = result.Message,
				Errors = result.Errors
			};

			if (result.IsSucceeded)
				return Ok(apiResponse);

			if (result.Message?.Contains("not found") == true)
				return NotFound(apiResponse);

			return BadRequest(apiResponse);
		}

		// PATCH: api/Governorate/5/discount
		[HttpPatch("{id}/discount")]
		public async Task<ActionResult<ApiResponse>> UpdateDiscountPercentage(int id, [FromBody] UpdateDiscountPercentageDto request)
		{
			if (!ModelState.IsValid)
			{
				var response = new ApiResponse
				{
					Success = false,
					Message = "Validation failed",
					Errors = ModelState.Values
						.SelectMany(v => v.Errors)
						.Select(e => e.ErrorMessage)
						.ToList()
				};
				return BadRequest(response);
			}

			var result = await _governorateService.UpdateDiscountPercentageAsync(id, request.DiscountPercentage);

			var apiResponse = new ApiResponse
			{
				Success = result.IsSucceeded,
				Message = result.Message,
				Errors = result.Errors
			};

			if (result.IsSucceeded)
				return Ok(apiResponse);

			if (result.Message?.Contains("not found") == true)
				return NotFound(apiResponse);

			return BadRequest(apiResponse);
		}
	}
}
