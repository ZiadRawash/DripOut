using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DripOut.Domain.Consts;

namespace DripOut.Application.Validators.ProductVariant
{
    class ValidSizeAtribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value is string && ProductSize.AllSizes.Contains(value))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult($"Size must be one of the following: {string.Join(", ", ProductSize.AllSizes)}");
        }
    }
}
