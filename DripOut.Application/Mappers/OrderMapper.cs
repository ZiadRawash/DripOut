
using DripOut.Domain.Models;
using System.Collections.Generic;

namespace DripOut.Application.Mappers
	{
		public class OrderMapper
		{
			public static List<OrderItem> MapCartItemsToOrderItems(
				List<CartItem> cartItems,
				Order theOrder)
			{
				var orderItems = new List<OrderItem>();

				foreach (var ci in cartItems)
				{
					var product = ci.ProductVariant.Product;

					decimal unitPrice = product!.Price;
					decimal discountPercent = (decimal)product.Discount; 
					decimal itemSubtotal = unitPrice * ci.Quantity;
					decimal itemDiscountValue = itemSubtotal * (discountPercent / 100); 
					decimal itemTotal = itemSubtotal - itemDiscountValue;
					var orderItem = new OrderItem
					{
						Order = theOrder,                      
						ProductVariantId = ci.ProductVariantId, 
						ProductName = product.Title,
						UnitPrice = unitPrice,
						Quantity = ci.Quantity,
						TotalPrice = itemTotal,
						DiscountApplied = discountPercent,
					
					};

					orderItems.Add(orderItem);
				}

				return orderItems;
			}
		}
}

