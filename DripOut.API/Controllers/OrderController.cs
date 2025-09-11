using DripOut.Application.DTOs.Order;
using DripOut.Application.Validators.Order;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DripOut.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly IValidator<PostShippingOrderDTO> _validator;
		public OrderController(IValidator<PostShippingOrderDTO> validator)
		{
			_validator = validator;
		}
		[HttpPost("shipping-info")]
		public async Task<IActionResult> CreateOrder([FromBody] PostShippingOrderDTO dto)
		{
			var validationResult = await _validator.ValidateAsync(dto);

			if (!validationResult.IsValid)
			{
				foreach (var error in validationResult.Errors)
				{
					ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
				}
				return BadRequest(ModelState);
			}
			return Ok();
		}
	}
}
