using DripOut.Application.DTOs;
using DripOut.Application.DTOs.Cart;
using DripOut.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DripOut.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CartController : ControllerBase
	{
		private readonly ICartService _cartService;

		public CartController(ICartService cartService)
		{
			_cartService = cartService;
		}

		/// <summary>
		/// Add item to cart
		/// </summary>
		[HttpPost("items")]
		public async Task<ActionResult<ApiResponse>> AddItemToCart(AddToCartDTO model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(new ApiResponse
				{
					Success = false,
					Message = "User not authenticated"
				});
			}

			var result = await _cartService.AddToCart(model, userId);
			if (result.IsSucceeded)
			{
				return Ok(new ApiResponse
				{
					Success = true,
					Message = result.Message
				});
			}

			return BadRequest(new ApiResponse
			{
				Success = false,
				Message = result.Message,
				Errors = result.Errors
			});
		}

		/// <summary>
		/// Delete specific cart item
		/// </summary>
		[HttpDelete("items/{cartItemId:int}")]
		public async Task<ActionResult<ApiResponse>> DeleteCartItem(int cartItemId)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(new ApiResponse
				{
					Success = false,
					Message = "User not authenticated"
				});
			}

			var result = await _cartService.DeleteCartItem(cartItemId, userId);
			if (result.IsSucceeded)
			{
				return Ok(new ApiResponse
				{
					Success = true,
					Message = result.Message
				});
			}

			return BadRequest(new ApiResponse
			{
				Success = false,
				Message = result.Message,
				Errors = result.Errors
			});
		}

		/// <summary>
		/// Get all cart items for the authenticated user
		/// </summary>
		[HttpGet("items")]
		public async Task<ActionResult<ApiResponse<CartReturnDto>>> GetCartItems()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(new ApiResponse<CartReturnDto>
				{
					Success = false,
					Message = "User not authenticated"
				});
			}

			var result = await _cartService.GetAllCartItems(userId);
			if (result.IsSucceeded)
			{
				return Ok(new ApiResponse<CartReturnDto>
				{
					Success = true,
					Message = result.Message,
					Data = result.Data
				});
			}

			return BadRequest(new ApiResponse<CartReturnDto>
			{
				Success = false,
				Message = result.Message,
				Errors = result.Errors
			});
		}
		[HttpPut("items")]
		public async Task<ActionResult<ApiResponse>> UpdateCartItemQuantity([FromBody] UpdateCartItemDTO model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var result = await _cartService.UpdateCartItemQuantity(model.cartItemId,model.Quantity,userId);
			var response = new ApiResponse()
			{
				Success = result.IsSucceeded,
				Message = result.Message,
				Errors = result.Errors
			};
			if (result.IsSucceeded)
				return Ok(response);
			return BadRequest(response);
		}
		[HttpDelete("clear")]
		public async Task<ActionResult<ApiResponse>> ClearCart()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var result = await _cartService.ClearCart(userId);
			var response = new ApiResponse()
			{
				Success = result.IsSucceeded,
				Message = result.Message,
				Errors = result.Errors
			};
			if (result.IsSucceeded)
				return Ok(response);
			return BadRequest(response);
		}
	}
}