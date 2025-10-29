using DripOut.Application.ApplicationServices;
using DripOut.Application.Common;
using DripOut.Application.DTOs;
using DripOut.Application.DTOs.Order;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Application.Interfaces.Services;
using DripOut.Application.Validators.Order;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;
namespace DripOut.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly IValidator<PostShippingOrderDTO> _validator;
		private readonly IOrderService _orderService;
		private const string webHook = "whsec_8421ff06b9d627964ab9afd98f171a38af9ccabcf30fbe493534fe27469ada0a";
		private readonly ILogger<OrderController> _logger;
		public OrderController(IValidator<PostShippingOrderDTO> validator, IOrderService orderService, ILogger<OrderController> logger  )
		{
			_validator = validator;
			_orderService = orderService;
			_logger = logger;
		}
		[HttpPost("checkout")]
		public async Task<ActionResult<ApiResponse<CheckoutResponseDTO>>> checkout([FromBody] PostShippingOrderDTO dto)
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
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(new ApiResponse
				{
					Success = false,
					Message = "User not authenticated"
				});
			}
			var orderCheckedOut = await _orderService.CheckOut(dto, userId);
			var response = new ApiResponse<CheckoutResponseDTO>
			{
				Success = true,
				Message = orderCheckedOut.Message,
				Errors = orderCheckedOut.Errors,
				Data = orderCheckedOut.Data
			};
			if (orderCheckedOut.IsSucceeded)
				return Ok(response);		
			return BadRequest(response);

		}

		[HttpPost("{orderId}/confirm")]
		public async Task<ActionResult<ApiResponse>> Confirm(int orderId)
		{
			Console.WriteLine("i enterded the cofirm");
			var result = await _orderService.ConfirmOrder(orderId);

			var response = new ApiResponse() { Errors = result.Errors,
			Message=result.Message,
			Success=result.IsSucceeded};
			if (result.IsSucceeded)
				return Ok(response);
			return BadRequest(response);
		}
		[HttpPost("stripe/webhook")]
		public async Task<IActionResult> StripeWebhook()
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
			var stripeSignature = Request.Headers["Stripe-Signature"];
			try
			{
				var stripeEvent = EventUtility.ConstructEvent(
					json,
					stripeSignature,
					"whsec_8421ff06b9d627964ab9afd98f171a38af9ccabcf30fbe493534fe27469ada0a"
				);

				if (stripeEvent.Type == "payment_intent.succeeded")
				{
					var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
					var orderId = int.Parse(paymentIntent.Metadata["order_id"]);
					_logger.LogInformation("payment with this stripeEvent {stripeEvent} successed and will enter the function", stripeEvent);
					var result = await _orderService.ProcessPaymentWebhook(orderId, paymentSucceeded: true);
					_logger.LogInformation("payment with this stripeEvent {stripeEvent} successed and will get out of the function", stripeEvent);
				}
				else if (stripeEvent.Type == "payment_intent.payment_failed")
				{
					var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
					var orderId = int.Parse(paymentIntent.Metadata["order_id"]);
					_logger.LogInformation("payment with this stripeEvent {stripeEvent} failed and will enter the function", stripeEvent);
					var result = await _orderService.ProcessPaymentWebhook(orderId, paymentSucceeded: false);
					_logger.LogInformation("payment with this stripeEvent {stripeEvent} failed and will get out of the function", stripeEvent);
				}
				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, " error occuared with this payment ");
				return BadRequest();
			}
			}
	}
	
}
