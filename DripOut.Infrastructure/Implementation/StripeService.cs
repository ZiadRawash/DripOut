using DripOut.Application.Common.Settings;
using DripOut.Application.DTOs.Payment;
using DripOut.Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Infrastructure.Implementation
{
	public class StripeService : IStripeService
	{
		private readonly StripeSettings _StripeSettings;
		private readonly PaymentIntentService _paymentIntentService;
		private readonly RefundService _refundService;
		public StripeService(IOptions<StripeSettings>Settings)
		{
			_StripeSettings=Settings.Value;
			var client = new StripeClient(_StripeSettings.Secretkey);
			_paymentIntentService = new PaymentIntentService(client);
			_refundService = new RefundService(client);
		}
		public async Task<StripePaymentResultDto> CancelPaymentAsync(string paymentIntentId)
		{
			try
			{
				var paymentIntent = await _paymentIntentService.CancelAsync(paymentIntentId);

				return new StripePaymentResultDto
				{
					IsSucceeded = paymentIntent.Status == "canceled",
					PaymentIntentId = paymentIntent.Id,
					Status = paymentIntent.Status
				};
			}
			catch (StripeException ex)
			{
				return new StripePaymentResultDto
				{
					IsSucceeded = false,
					ErrorMessage = ex.Message
				};
			}
		}

		public async Task<StripePaymentResultDto> ConfirmPaymentAsync(string paymentIntentId)
		{
			try
			{
				var paymentIntent = await _paymentIntentService.ConfirmAsync(paymentIntentId);

				return new StripePaymentResultDto
				{
					IsSucceeded = paymentIntent.Status == "succeeded",
					PaymentIntentId = paymentIntent.Id,
					Status = paymentIntent.Status,
					Amount = paymentIntent.Amount / 100m, 
					Currency = paymentIntent.Currency
				};
			}
			catch (StripeException ex)
			{
				return new StripePaymentResultDto
				{
					IsSucceeded = false,
					ErrorMessage = ex.Message
				};
			}
		}

		public async Task<StripePaymentResultDto> CreatePaymentIntentAsync(int orderId, decimal amount, string currency )
		{
			try
			{
				var amountInCents = (long)(amount * 100);

				var options = new PaymentIntentCreateOptions
				{
					Amount = amountInCents,
					Currency = currency,
					Metadata = new Dictionary<string, string>
					{
						{ "order_id", orderId.ToString() }
					},
					AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
					{
						Enabled = true
					}
				};

				var paymentIntent = await _paymentIntentService.CreateAsync(options);

				return new StripePaymentResultDto
				{
					IsSucceeded = true,
					PaymentIntentId = paymentIntent.Id,
					ClientSecret = paymentIntent.ClientSecret,
					Status = paymentIntent.Status,
					Amount = amount,
					Currency = currency
				};
			}
			catch (StripeException ex)
			{
				return new StripePaymentResultDto
				{
					IsSucceeded = false,
					ErrorMessage = ex.Message
				};
			}
		}

		public Task<StripePaymentResultDto> ProcessRefundAsync(string paymentIntentId, decimal? amount = null)
		{
			throw new NotImplementedException();
		}
	}
}
