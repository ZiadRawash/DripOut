using DripOut.Application.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.Services
{
	public interface IStripeService
	{

		/// <summary>
		/// Creates a payment intent for the order
		/// </summary>
		/// <param name="orderId">Order ID</param>
		/// <param name="amount">Amount in cents (e.g., $10.50 = 1050)</param>
		/// <param name="currency">Currency code (default: "usd")</param>
		/// <returns>Payment intent with client secret</returns>
		Task<StripePaymentResultDto> CreatePaymentIntentAsync(int orderId, decimal amount, string currency);

		/// <summary>
		/// Confirms a payment intent
		/// </summary>
		/// <param name="paymentIntentId">Stripe payment intent ID</param>
		/// <returns>Payment confirmation result</returns>
		Task<StripePaymentResultDto> ConfirmPaymentAsync(string paymentIntentId);

		/// <summary>
		/// Cancels a payment intent
		/// </summary>
		/// <param name="paymentIntentId">Stripe payment intent ID</param>
		/// <returns>Cancellation result</returns>
		Task<StripePaymentResultDto> CancelPaymentAsync(string paymentIntentId);

		/// <summary>
		/// Processes refund for a payment
		/// </summary>
		/// <param name="paymentIntentId">Stripe payment intent ID</param>
		/// <param name="amount">Refund amount in cents (null for full refund)</param>
		/// <returns>Refund result</returns>
		Task<StripePaymentResultDto> ProcessRefundAsync(string paymentIntentId, decimal? amount = null);

	}
}
