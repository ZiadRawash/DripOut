using DripOut.Application.Common;
using DripOut.Application.DTOs.Order;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Application.Interfaces.Services;
using DripOut.Application.Mappers;
using DripOut.Domain.Consts;
using DripOut.Domain.Enums;
using DripOut.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.ApplicationServices
{
	public class OrderService : IOrderService
	{
		private readonly IUnitOfWork _unitOfWork;
		private const int ReservationMinutes = 15;
		private readonly IStripeService _stripeService;
		private readonly ICartService _cartService;

		public OrderService(IUnitOfWork unitOfWork, IStripeService stripeService, ICartService cartService	)
		{
			_unitOfWork = unitOfWork;
			_stripeService = stripeService;
			_cartService = cartService;
		}

		public async Task<Result<CheckoutResponseDTO>> CheckOut(PostShippingOrderDTO dto, string userId)
		{
			// validate address
			var addressValidated = await ValidateGovernorate(dto.GovernorateId);
			if (!addressValidated)
			{
				return Result<CheckoutResponseDTO>.Failure(
					message: $"GovernorateId {dto.GovernorateId} is not available",
					Errors: new List<string> { "InvalidGovernorate" }
				);
			}

			// get cart first to avoid multiple db calls
			var cartEntity = await _unitOfWork.Carts.FindAsync(
				x => x.AppUserId == userId,
				q => q.Include(c => c.CartItems)
					  .ThenInclude(ci => ci.ProductVariant)
					  .ThenInclude(pv => pv.Product));

			if (cartEntity == null || !cartEntity.CartItems.Any())
			{
				return Result<CheckoutResponseDTO>.Failure(
					message: "Cart is empty",
					Errors: new List<string> { "EmptyCart" }
				);
			}

			// validate cart items availability
			var cartValidation = await ValidateCart(userId);
			if (!cartValidation.IsSucceeded)
			{
				var response = new CheckoutResponseDTO
				{
					IsSucceeded = false,
					Message = "Some items are not available in the requested quantity",
					InvalidItems = cartValidation.Data?.InvalidItems ?? new List<InvalidCartItemDTO>()
				};

				return Result<CheckoutResponseDTO>.Success(response);
			}

			var order = new Order();

			// create order items
			var orderItems = OrderMapper.MapCartItemsToOrderItems(cartEntity.CartItems.ToList(), order);
			order.OrderItems = orderItems;

			// create reservations
			var stockReservations = orderItems.Select(oi => new StockReservation
			{
				UserId = userId,
				CreatedAt = DateTime.UtcNow,
				Quantity = oi.Quantity,
				ProductVariantId = oi.ProductVariantId,
				ExpiresAt = DateTime.UtcNow.AddMinutes(ReservationMinutes),
				OrderItem = oi
			}).ToList();

			// get governorate
			var shippingGovernorate = await _unitOfWork.Governorates.FindAsync(dto.GovernorateId);
			if (shippingGovernorate == null)
			{
				return Result<CheckoutResponseDTO>.Failure(
					message: "Governorate not found",
					Errors: new List<string> { "GovernorateNotFound" }
				);
			}

			// create order
			var subtotal = orderItems.Sum(x => x.TotalPrice);
			var shippingCostBeforeDiscount = shippingGovernorate.ShippingCost;
			var shippingCostAfterDiscount = CalculateShippingDiscount(
				subtotal,
				shippingGovernorate.Threshold,
				shippingGovernorate.DiscountPercentage,
				shippingGovernorate.ShippingCost
			);
			var shippingDiscount = shippingCostBeforeDiscount - shippingCostAfterDiscount;

			order.OrderDate = DateTime.UtcNow;
			order.TotalCost = subtotal+shippingCostAfterDiscount;
			order.ShippingCostSnapShot = shippingCostAfterDiscount;
			order.Status = OrderStatus.Pending;
			order.ShippingPhone = dto.ShippingPhone;
			order.ShippingAddress = dto.ShippingAddress;
			order.AppUserId = userId;
			order.GovernorateId = dto.GovernorateId;
			order.GovernorateNameSnapshot = shippingGovernorate.Name;
			order.SubTotal = subtotal;

			await _unitOfWork.Orders.AddAsync(order);
			await _unitOfWork.OrderItems.AddRangeAsync(orderItems);
			await _unitOfWork.StockReservations.AddRangeAsync(stockReservations);
			await _unitOfWork.SaveChangesAsync();

			var responseSuccess = new CheckoutResponseDTO
			{
				IsSucceeded = true,
				Message = "Order created successfully",
				OrderId = order.Id,
				Subtotal = subtotal,
				ShippingCostBeforeDiscount = shippingCostBeforeDiscount,
				ShippingDiscount = shippingDiscount,
				ShippingCostAfterDiscount = shippingCostAfterDiscount,
				TotalCost = subtotal + shippingCostAfterDiscount
			};

			return Result<CheckoutResponseDTO>.Success(responseSuccess);
		}
		public async Task<Result> ConfirmOrder(int orderId)
		{
			// Get order with its items and reservations
			var order = await _unitOfWork.Orders.FindAsync(
				x=>x.Id==orderId,
				q => q.Include(o => o.OrderItems)
					.ThenInclude(oi => oi.StockReservation)
			);

			if (order == null)
			{
				return Result.Failure(
					message: "Order not found",
					Errors: new List<string> { "OrderNotFound" }
				);
			}

			var allReservationsActive = order.OrderItems
				.All(oi => oi.StockReservation != null && !oi.StockReservation.IsExpired);

			if (!allReservationsActive)
			{
				order.Status = OrderStatus.Cancelled;
				var reservationsToDelete = order.OrderItems
					.Where(oi => oi.StockReservation != null)
					.Select(oi => oi.StockReservation!)  
					.ToList();				
				if (reservationsToDelete.Any())
				{
					await _unitOfWork.StockReservations.DeleteRangeAsync(reservationsToDelete);
				}
				
				await _unitOfWork.SaveChangesAsync();

				return Result.Failure(
					message: "One or more item reservations have expired",
					Errors: new List<string> { "ExpiredReservations" }
				);
			}

			foreach (var orderItem in order.OrderItems)
			{
				if (orderItem.StockReservation != null)
				{
					orderItem.StockReservation.ExpiresAt = DateTime.UtcNow.AddMinutes(ReservationMinutes);
				}
			}
			var orderCreated = await _stripeService.CreatePaymentIntentAsync(orderId, order.TotalCost, Currency.Egypt);
			if (!orderCreated.IsSucceeded)
			{
				return Result.Failure(
					message: "Stripe Error",
					Errors: new List<string> { orderCreated.ErrorMessage?? "Error happened While Creating Payment with Stripe" }
				);
			}
			order.PaymentIntentId= orderCreated.PaymentIntentId;
			order.Status = OrderStatus.AwaitingPayment;
			await _unitOfWork.SaveChangesAsync();
			return Result.Success(orderCreated.ClientSecret);
		}
		public async Task<Result> ProcessPaymentWebhook(int orderId, bool paymentSucceeded)
		{
			var order = await _unitOfWork.Orders.FindAsync(
				x => x.Id == orderId,
				q => q.Include(o => o.OrderItems)
					.ThenInclude(oi => oi.StockReservation)
			);

			if (order == null)
			{
				return Result.Failure("Order not found", new List<string> { "OrderNotFound" });
			}

			if (paymentSucceeded)
			{	
				foreach (var orderItem in order.OrderItems)
				{
					var variant = await _unitOfWork.Variants.FindAsync(orderItem.ProductVariantId);
					if (variant != null)
					{
						variant.StockQuantity -= orderItem.Quantity;
					}

					if (orderItem.StockReservation != null)
					{
						await _unitOfWork.StockReservations.DeleteAsync(orderItem.StockReservation);
					}
				}

				order.Status = OrderStatus.Confirmed;
			}
			else
			{
				// Release reservations
				var reservationsToDelete = order.OrderItems
					.Where(oi => oi.StockReservation != null)
					.Select(oi => oi.StockReservation!)
					.ToList();

				if (reservationsToDelete.Any())
				{
					await _unitOfWork.StockReservations.DeleteRangeAsync(reservationsToDelete);
				}

				order.Status = OrderStatus.Cancelled;
			}
			await _cartService.ClearCart(order.AppUserId);
			await _unitOfWork.SaveChangesAsync();
			return Result.Success("Payment processed successfully");
		}
		// validate governorate
		private async Task<bool> ValidateGovernorate(int governorateId)
		{
			var found = await _unitOfWork.Governorates.FindAsync(governorateId);
			return found?.IsActive ?? false;
		}

		// calculate quantity of stock-reservation
		private async Task<Dictionary<int, int>> QuantityMinusReservation(List<int> productVariantIds)
		{
			var variants = await _unitOfWork.Variants.GetAllAsync(
				v => productVariantIds.Contains(v.Id),
				v => v.StockReservations
			);

			return variants.ToDictionary(
				v => v.Id,
				v => v.StockQuantity - v.StockReservations
					.Where(r => !r.IsExpired)
					.Sum(r => r.Quantity)
			);
		}

		// validate cart
		private async Task<Result<CartValidationResultDto>> ValidateCart(string userId)
		{
			var cart = await _unitOfWork.Carts
				.FindAsync(x => x.AppUserId == userId, x => x.CartItems);

			if (cart == null || !cart.CartItems.Any())
			{
				return Result<CartValidationResultDto>.Success(new CartValidationResultDto { IsValid = true });
			}

			var cartItems = cart.CartItems
				.Select(x => new VarientIdWithQuantityDTO
				{
					VariantID = x.ProductVariantId,
					Quantity = x.Quantity
				})
				.ToList();

			var variantIds = cartItems.Select(x => x.VariantID).ToList();
			var availableQuantities = await QuantityMinusReservation(variantIds);
			var invalidItems = new List<InvalidCartItemDTO>();

			foreach (var item in cartItems)
			{
				if (availableQuantities.TryGetValue(item.VariantID, out var availableStock))
				{
					if (item.Quantity > availableStock)
					{
						invalidItems.Add(new InvalidCartItemDTO
						{
							VariantId = item.VariantID,
							RequestedQuantity = item.Quantity,
							AvailableQuantity = availableStock
						});
					}
				}
			}

			if (invalidItems.Any())
			{				
				var validationResult = new CartValidationResultDto
				{
					IsValid = false,
					InvalidItems = invalidItems
				};
				var result = Result<CartValidationResultDto>.Success(validationResult);
				result.IsSucceeded = false;
				result.Message = "Some items are not available in requested quantities";
				result.Errors = new List<string> { "InsufficientStock" };
				return result;
			}

			return Result<CartValidationResultDto>.Success(new CartValidationResultDto { IsValid = true });
		}

		//Calculate shipping
		public decimal CalculateShippingDiscount(decimal totalPrice,decimal threshold,decimal discount,decimal shippingPrice)
		{
			if (totalPrice >= threshold)
			{
				var discountValue = (discount / 100m) * shippingPrice;
				return shippingPrice - discountValue;
			}
			return shippingPrice;
		}
		public async Task<Result> ValidateOrderBeforePayment(int orderId)
		{
			var order = await _unitOfWork.Orders.FindAsync(
				x => x.Id == orderId,
				q => q.Include(o => o.OrderItems)
					  .ThenInclude(oi => oi.StockReservation)
			);

			if (order == null || order.Status != OrderStatus.AwaitingPayment)
			{
				return Result.Failure("Order not valid", new List<string> { "InvalidOrder" });
			}

			// Check if any reservation expired
			var anyExpired = order.OrderItems.Any(oi =>
				oi.StockReservation == null || oi.StockReservation.IsExpired);

			if (anyExpired)
			{
				// Cancel order and payment
				if (!string.IsNullOrEmpty(order.PaymentIntentId))
				{
					await _stripeService.CancelPaymentAsync(order.PaymentIntentId);
				}

				var reservations = order.OrderItems
					.Where(oi => oi.StockReservation != null)
					.Select(oi => oi.StockReservation!)
					.ToList();

				if (reservations.Any())
				{
					await _unitOfWork.StockReservations.DeleteRangeAsync(reservations);
				}

				order.Status = OrderStatus.Cancelled;
				await _unitOfWork.SaveChangesAsync();

				return Result.Failure("Reservation expired", new List<string> { "ExpiredReservation" });
			}

			return Result.Success("Order is valid");
		}

	}

	
}