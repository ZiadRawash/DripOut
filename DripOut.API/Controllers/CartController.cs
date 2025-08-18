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
		[HttpPost("Add")]
		public async Task<IActionResult> AddToCart(AddToCartDTO model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized(new ApiResponse
				{
					Success = false,
					Message = "User not authenticated"
				});

			var result = await _cartService.AddToCart(model, userId);
			
			return result.IsSucceeded 
				? Ok(new ApiResponse
				{
					Success = true,
					Message = result.Message
				})
				: BadRequest(new ApiResponse
				{
					Success = false,
					Message = result.Message,
					Errors = result.Errors
				});
		}
		[HttpDelete("Delete/{id}")]
		public async Task<IActionResult> DeleteCartItem(int id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var result = await _cartService.DeleteCartItem(id, userId);
			if (result.IsSucceeded)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = result.Message,
					Errors = result.Errors
				});
			}
			return BadRequest(new ApiResponse
			{
				Success = false,
				Errors = result.Errors,
				Message = result.Message

			});
		}
	}
}
