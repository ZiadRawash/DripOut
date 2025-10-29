using DripOut.Application.Common;
using DripOut.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.Services
{
	public interface IOrderService
	{
		Task<Result<CheckoutResponseDTO>> CheckOut(PostShippingOrderDTO dto, string userId);
		Task <Result> ConfirmOrder(int orderId);
		Task<Result> ProcessPaymentWebhook(int orderId, bool paymentSucceeded);

	}
}
