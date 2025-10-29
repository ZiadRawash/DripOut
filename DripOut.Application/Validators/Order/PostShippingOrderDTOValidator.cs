using DripOut.Application.DTOs.Order;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Validators.Order
{
	public class PostShippingOrderDTOValidator : AbstractValidator<PostShippingOrderDTO>
		{
			public PostShippingOrderDTOValidator()
			{
				
				RuleFor(x => x.GovernorateId).NotEmpty().WithMessage("GovernorateId can not be empty")
					.GreaterThanOrEqualTo(1);

				RuleFor(x => x.ShippingPhone)
					.NotEmpty()
					.WithMessage("Shipping phone number is required. ")
					.Length(10, 15)
					.WithMessage("Phone number must be between 10 and 15 characters.")
					.Matches(@"^[\+]?[0-9\-\(\)\s]+$")
					.WithMessage("Phone number contains invalid characters. Use English numbers only.");

				RuleFor(x => x.ShippingAddress)
					.NotEmpty()
					.WithMessage("Shipping address is required.")
					.Length(10, 200)
					.WithMessage("Shipping address must be between 10 and 200 characters.")
					.Must(address => ContainAddressComponents(address))
					.WithMessage("Address must contain both street information and location details. ");
			}

			private bool ContainAddressComponents(string address)
			{
				if (string.IsNullOrWhiteSpace(address))
					return false;

				// Check for numbers (English digits only for addresses with numbers)
				// But also accept Arabic addresses that might not have numbers
				bool hasEnglishNumbers = address.Any(c => c >= '0' && c <= '9');
				// Check for letters (Arabic or English)
				bool hasLetters = address.Any(c => char.IsLetter(c) || (c >= '\u0600' && c <= '\u06FF'));

				// Accept if it has letters and either has English numbers or is purely Arabic text
				return hasLetters && (hasEnglishNumbers || address.Any(c => c >= '\u0600' && c <= '\u06FF'));
			
		}
	}
}