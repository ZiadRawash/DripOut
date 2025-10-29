using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Payment
{
	public class StripePaymentResultDto
	{
		public bool IsSucceeded { get; set; }
		public string? PaymentIntentId { get; set; }
		public string? ClientSecret { get; set; }
		public string? Status { get; set; } 
		public string? ErrorMessage { get; set; }
		public decimal? Amount { get; set; }
		public string? Currency { get; set; }
	}
}
