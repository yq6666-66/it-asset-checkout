using System.ComponentModel.DataAnnotations;
using ItAssetCheckout.Models;

namespace ItAssetCheckout.Tests;

public class AssetValidationTests
{
    [Fact(DisplayName = "UT-05 Purchase date cannot be in the future")]
    public void UT05_FuturePurchaseDate_IsRejected()
    {
        var future = Sample(DateTime.Today.AddDays(1));
        var futureResults = Validate(future);
        Assert.Contains(futureResults, result =>
            result.ErrorMessage == "Purchase date cannot be in the future.");

        var today = Sample(DateTime.Today);
        Assert.DoesNotContain(Validate(today), result =>
            result.ErrorMessage == "Purchase date cannot be in the future.");
    }

    private static Asset Sample(DateTime purchaseDate)
    {
        return new Asset
        {
            TagNo = "IT-10482",
            Name = "Dell Latitude 5440",
            Category = "Laptop",
            Status = AssetStatus.Available,
            PurchaseDate = purchaseDate,
            Value = 1280m
        };
    }

    private static List<ValidationResult> Validate(Asset asset)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(asset, new ValidationContext(asset), results, validateAllProperties: true);
        return results;
    }
}
