using DripOut.Application.Common;
using DripOut.Application.DTOs.Cart;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Application.Interfaces.Services;
using DripOut.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
					Cart=cart,
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
			cart.UpdatedOn = DateTime.Now;
			await _unitOfWork.Carts.UpdateAsync(cart);
			await _unitOfWork.SaveChangesAsync();
			return Result.Success("Cart item deleted successfully");
		}

		public async Task<Result<CartReturnDto>> GetAllCartItems(string userId)
		{
			if (string.IsNullOrEmpty(userId))
				return Result<CartReturnDto>.Failure(new List<string> { "User Unauthorized" });

			var cartfound = await _unitOfWork.Carts.FindAsync(
			x => x.AppUserId == userId,
			q => q.Include(c => c.CartItems)
			  .ThenInclude(ci => ci.ProductVariant)
				  .ThenInclude(pv => pv.Product)
					  .ThenInclude(p => p!.Images));



			if (cartfound == null)
				return Result<CartReturnDto>.Failure(new List<string> { "User doesn't have a cart yet" });

			var cartItems = new List<CartItemDto>();
			decimal cartSubtotal = 0;
			decimal totalDiscount = 0;

			foreach (var item in cartfound.CartItems)
			{
				var product = item.ProductVariant.Product;
				decimal itemSubtotal = product!.Price * item.Quantity;
				decimal itemDiscount = itemSubtotal * (decimal)(product.Discount / 100);

				var cartItemDto = new CartItemDto
				{
					CartId = item.Id,
					VarientName = product.Title,
					Quantity = item.Quantity,
					VariantId = item.ProductVariantId,
					UnitPrice = product.Price,
					Subtotal = itemSubtotal,
					DiscountApplied = itemDiscount,
					Total = itemSubtotal - itemDiscount,
					ImageUrl = product.Images?
						.Select(x => x.ImageUrl)
						.FirstOrDefault(u => !string.IsNullOrEmpty(u)) ?? string.Empty
				};

				cartItems.Add(cartItemDto);
				cartSubtotal += itemSubtotal;
				totalDiscount += itemDiscount;
			}

			var result = new CartReturnDto
			{
				Items = cartItems,
				TotalItems = cartItems.Count,
				CartSubtotal = cartSubtotal,
				TotalDiscount = totalDiscount,
				CartTotal = cartSubtotal - totalDiscount
			};

			return Result<CartReturnDto>.Success(result, "Cart items retrieved successfully");
		}

		public async Task<Result> UpdateCartItemQuantity(int cartItemId, int quantity, string? userId = null)
		{
			if (string.IsNullOrEmpty(userId))
				return Result.Failure(new List<string> { "Unauthorized" });

			if (cartItemId <= 0)
				return Result.Failure(new List<string> { "Invalid cart item id" });

			var cartItem = await _unitOfWork.CartItems.FindAsync(x => x.Id == cartItemId);
			if (cartItem == null)
				return Result.Failure(new List<string> { "Cart item not found" });

			var cart = await _unitOfWork.Carts.FindAsync(x => x.Id == cartItem.CartId);
			if (cart == null || cart.AppUserId != userId)
				return Result.Failure(new List<string> { "Unauthorized to update this cart item" });


			var variant = await _unitOfWork.Variants.FindAsync(x => x.Id == cartItem.ProductVariantId);
			if (variant == null)
				return Result.Failure(new List<string> { "Product variant not found" });

			if (variant.StockQuantity < quantity)
				return Result.Failure(new List<string> { "Requested quantity exceeds available stock" });

			cartItem.Quantity = quantity;
			cart.UpdatedOn = DateTime.UtcNow;

			await _unitOfWork.CartItems.UpdateAsync(cartItem);
			await _unitOfWork.SaveChangesAsync();

			return Result.Success("Cart item quantity updated successfully");
		}

		public async Task<Result> ClearCart(string? userId)
		{
			if (string.IsNullOrEmpty(userId))
				return Result.Failure("User unauthorized");
			var cart = await _unitOfWork.Carts.FindAsync(x => x.AppUserId == userId, x=>x.CartItems);
			if (cart==null)
				return Result.Failure("there is no cart to be cleared");
			foreach (var item in cart.CartItems)
			{
				await _unitOfWork.CartItems.DeleteAsync(item);

			}
			cart.UpdatedOn = DateTime.UtcNow;
			await _unitOfWork.Carts.UpdateAsync(cart);
			await _unitOfWork.SaveChangesAsync();
			return Result.Success("Cart is cleared successfully");
		}
	}
}
