using DripOut.Application.Common;
using DripOut.Application.DTOs.Cart;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Application.Interfaces.Services;
using DripOut.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.ApplicationServices
{
	public class CartService : ICartService
	{
		public readonly IUnitOfWork _unitOfWork;
		public CartService(IUnitOfWork unitOfWork)
		{
		_unitOfWork = unitOfWork;	
		}
		public async Task<Result> AddToCart(AddToCartDTO model, string userId)
		{
			if (userId == null)
				return Result.Failure(new List<string> { "Un Authorized" });

			var variant = await _unitOfWork.Variants.FindAsync(x => x.Id == model.Id);
			if (variant == null)
				return Result.Failure(new List<string> { "Product variant not found" });

			if (variant.StockQuantity < model.Quantity)
				return Result.Failure(new List<string> { "Requested quantity exceeds available stock" });

			var cart = await _unitOfWork.Carts.FindAsync(x => x.AppUserId == userId);
			
			if (cart != null)
			{
				var cartItem = await _unitOfWork.CartItems.FindAsync(x => x.CartId == cart.Id && x.ProductVariantId == model.Id);
				if (cartItem != null)
				{
					int newQuantity = cartItem.Quantity + model.Quantity;
					if (newQuantity > variant.StockQuantity)
						return Result.Failure(new List<string> { "Total quantity exceeds available stock" });

					cartItem.Quantity = newQuantity;
					cart.UpdatedOn=DateTime.UtcNow;
					await _unitOfWork.CartItems.UpdateAsync(cartItem);
				}
				else
				{
					cartItem = new CartItem
					{
						
						CartId = cart.Id,
						ProductVariantId = model.Id,
						Quantity = model.Quantity
					};
					await _unitOfWork.CartItems.AddAsync(cartItem);
				}
			}
			else
			{
				cart = new Cart { AppUserId = userId ,CreatedOn=DateTime.UtcNow };
				await _unitOfWork.Carts.AddAsync(cart);
				
				var cartItem = new CartItem
				{
					CartId = cart.Id,
					ProductVariantId = model.Id,
					Quantity = model.Quantity
				};
				await _unitOfWork.CartItems.AddAsync(cartItem);
			}

			await _unitOfWork.SaveChangesAsync();
			return Result.Success("Item added to cart successfully");
		}

		public async Task<Result> DeleteCartItem(int cartItemId, string userId)
		{
			if (string.IsNullOrEmpty(userId))
				return Result.Failure(new List<string> { "Unauthorized" });

			if (cartItemId <= 0)
				return Result.Failure(new List<string> { "Invalid cart item id" });

			var cartItem = await _unitOfWork.CartItems.FindAsync(x => x.Id == cartItemId);
			if (cartItem == null)
				return Result.Failure(new List<string> { "Cart item not found" });

			// Verify the cart item belongs to the user
			var cart = await _unitOfWork.Carts.FindAsync(x => x.Id == cartItem.CartId);
			if (cart == null || cart.AppUserId != userId)
				return Result.Failure(new List<string> { "Unauthorized to delete this cart item" });

			await _unitOfWork.CartItems.DeleteAsync(cartItem);
			await _unitOfWork.SaveChangesAsync();
			return Result.Success("Cart item deleted successfully");
		}
	}
}
