using System.ComponentModel.DataAnnotations;

namespace ItAssetCheckout.Models;

// Requires a purchase date from 1990-01-01 through today.
public sealed class ReasonablePurchaseDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateTime date || date == default)
        {
            return new ValidationResult("Purchase date is required.");
        }

        var day = date.Date;
        if (day < new DateTime(1990, 1, 1))
        {
            return new ValidationResult("Purchase date cannot be before 1990-01-01.");
        }

        if (day > DateTime.Today)
        {
            return new ValidationResult("Purchase date cannot be in the future.");
        }

        return ValidationResult.Success;
    }
}
