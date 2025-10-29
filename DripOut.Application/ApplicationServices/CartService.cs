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
		private readonly IUnitOfWork _unitOfWork; 
		public CartService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<Result> AddToCart(AddToCartDTO model, string userId)
		{
			if (string.IsNullOrEmpty(userId)) 
				return Result.Failure(new List<string> { "Unauthorized" });

			if (model.Quantity <= 0) 
				return Result.Failure(new List<string> { "Quantity must be greater than zero" });

			var variant = await _unitOfWork.Variants.FindAsync(
				x => x.Id == model.Id,
				q => q.Include(v => v.StockReservations)
			);

			if (variant == null)
				return Result.Failure(new List<string> { "Product variant not found" });

			var availableStock = GetAvailableStock(variant);

			if (availableStock < model.Quantity)
				return Result.Failure(new List<string> { "Requested quantity exceeds available stock" });

			var cart = await _unitOfWork.Carts.FindAsync(
				x => x.AppUserId == userId,
				q => q.Include(c => c.CartItems)
			);

			if (cart != null)
			{
				var cartItem = cart.CartItems.FirstOrDefault(x => x.ProductVariantId == model.Id);
				if (cartItem != null)
				{
					int newQuantity = cartItem.Quantity + model.Quantity;
					if (newQuantity > availableStock)
						return Result.Failure(new List<string> { "Total quantity exceeds available stock" });

					cartItem.Quantity = newQuantity;
					cart.UpdatedOn = DateTime.UtcNow;
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
					cart.UpdatedOn = DateTime.UtcNow;
					await _unitOfWork.Carts.UpdateAsync(cart);
				}
			}
			else
			{
				cart = new Cart
				{
					AppUserId = userId,
					CreatedOn = DateTime.UtcNow,
					UpdatedOn = DateTime.UtcNow
				};
				await _unitOfWork.Carts.AddAsync(cart);

				var cartItem = new CartItem
				{
					Cart = cart,
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

			// Get cart item with cart in one query
			var cartItem = await _unitOfWork.CartItems.FindAsync(
				x => x.Id == cartItemId,
				q => q.Include(ci => ci.Cart)
			);

			if (cartItem == null)
				return Result.Failure(new List<string> { "Cart item not found" });

			// Verify the cart item belongs to the user
			if (cartItem.Cart == null || cartItem.Cart.AppUserId != userId)
				return Result.Failure(new List<string> { "Unauthorized to delete this cart item" });

			await _unitOfWork.CartItems.DeleteAsync(cartItem);

			cartItem.Cart.UpdatedOn = DateTime.UtcNow; // Use UtcNow for consistency
			await _unitOfWork.Carts.UpdateAsync(cartItem.Cart);

			await _unitOfWork.SaveChangesAsync();
			return Result.Success("Cart item deleted successfully");
		}

		public async Task<Result<CartReturnDto>> GetAllCartItems(string userId)
		{
			if (string.IsNullOrEmpty(userId))
				return Result<CartReturnDto>.Failure(new List<string> { "User Unauthorized" });

			var cartFound = await _unitOfWork.Carts.FindAsync(
				x => x.AppUserId == userId,
				q => q.Include(c => c.CartItems)
					.ThenInclude(ci => ci.ProductVariant)
						.ThenInclude(pv => pv.Product)
							.ThenInclude(p => p!.Images));

			if (cartFound == null || !cartFound.CartItems.Any())
				return Result<CartReturnDto>.Success(new CartReturnDto
				{
					Items = new List<CartItemDto>(),
					TotalItems = 0,
					CartSubtotal = 0,
					TotalDiscount = 0,
					CartTotal = 0
				}, "Cart is empty");

			var cartItems = new List<CartItemDto>();
			decimal cartSubtotal = 0;
			decimal totalDiscount = 0;

			foreach (var item in cartFound.CartItems)
			{
				var product = item.ProductVariant?.Product;
				if (product == null) continue; // Skip invalid items

				decimal itemSubtotal = product.Price * item.Quantity;
				decimal itemDiscount = itemSubtotal * (decimal)(product.Discount / 100.0);

				var cartItemDto = new CartItemDto
				{
					Id = item.Id,
					VarientName = product.Title,
					Quantity = item.Quantity,
					VariantId = item.ProductVariantId,
					UnitPrice = product.Price,
					Subtotal = itemSubtotal,
					DiscountApplied = itemDiscount,
					Total = itemSubtotal - itemDiscount,
					ImageUrl = product.Images?
						.Where(x => !string.IsNullOrEmpty(x.ImageUrl))
						.Select(x => x.ImageUrl)
						.FirstOrDefault() ?? string.Empty
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

			if (quantity <= 0)
				return Result.Failure(new List<string> { "Quantity must be greater than zero" });

			var cartItem = await _unitOfWork.CartItems.FindAsync(
				x => x.Id == cartItemId,
				q => q.Include(ci => ci.Cart)
					.Include(ci => ci.ProductVariant)
						.ThenInclude(pv => pv.StockReservations)
			);

			if (cartItem == null)
				return Result.Failure(new List<string> { "Cart item not found" });

			if (cartItem.Cart == null || cartItem.Cart.AppUserId != userId)
				return Result.Failure(new List<string> { "Unauthorized to update this cart item" });

			if (cartItem.ProductVariant == null)
				return Result.Failure(new List<string> { "Product variant not found" });

			var availableStock = GetAvailableStock(cartItem.ProductVariant);
			if (availableStock < quantity)
				return Result.Failure(new List<string> { "Requested quantity exceeds available stock" });

			cartItem.Quantity = quantity;
			cartItem.Cart.UpdatedOn = DateTime.UtcNow;

			await _unitOfWork.CartItems.UpdateAsync(cartItem);
			await _unitOfWork.SaveChangesAsync();

			return Result.Success("Cart item quantity updated successfully");
		}

		public async Task<Result> ClearCart(string? userId)
		{
			if (string.IsNullOrEmpty(userId))
				return Result.Failure("User unauthorized");

			var cart = await _unitOfWork.Carts.FindAsync(
				x => x.AppUserId == userId,
				x => x.Include(c => c.CartItems)
			);

			if (cart == null || !cart.CartItems.Any())
				return Result.Success("Cart is already empty");

			// Use bulk delete instead of individual deletes
			await _unitOfWork.CartItems.DeleteRangeAsync(cart.CartItems);

			cart.UpdatedOn = DateTime.UtcNow;
			await _unitOfWork.Carts.UpdateAsync(cart);
			await _unitOfWork.SaveChangesAsync();

			return Result.Success("Cart cleared successfully");
		}

		private static int GetAvailableStock(ProductVariant variant)
		{
			
			var activeReservations = variant.StockReservations
				?.Where(r => !r.IsExpired)
				.Sum(r => r.Quantity) ?? 0;

			return variant.StockQuantity - activeReservations;
		}
	}
}