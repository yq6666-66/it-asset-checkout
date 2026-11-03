using System.ComponentModel.DataAnnotations;
using ItAssetCheckout.Models;

namespace ItAssetCheckout.Tests;

public class CheckoutValidationTests
{
    [Fact(DisplayName = "UT-01 Due date before the checkout date is rejected")]
    public void UT01_DueBeforeCheckout_IsRejected()
    {
        var checkout = Sample(new DateTime(2024, 6, 10), new DateTime(2024, 6, 9));

        var results = Validate(checkout);

        Assert.Contains(results, result =>
            result.MemberNames.Contains(nameof(Checkout.DueAt))
            && result.ErrorMessage == "Due date cannot be before the checkout date.");
    }

    [Fact(DisplayName = "UT-02 Due date on the checkout date is accepted")]
    public void UT02_DueOnCheckoutDate_IsAccepted()
    {
        var checkout = Sample(new DateTime(2024, 6, 10), new DateTime(2024, 6, 10));

        var results = Validate(checkout);

        Assert.Empty(results);
    }

    private static Checkout Sample(DateTime checkoutAt, DateTime dueAt)
    {
        return new Checkout
        {
            AssetId = 1,
            StaffId = 1,
            CheckoutAt = checkoutAt,
            DueAt = dueAt,
            ReturnAt = null
        };
    }

    private static List<ValidationResult> Validate(Checkout checkout)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(checkout, new ValidationContext(checkout), results, validateAllProperties: true);
        return results;
    }
}
