using System.ComponentModel.DataAnnotations;

namespace ItAssetCheckout.Models;

// Closed set of asset lifecycle states used by the Assets form.
public static class AssetStatus
{
    public const string Available = "Available";
    public const string InUse = "In Use";
    public const string Maintenance = "Maintenance";
    public const string Retired = "Retired";

    public static readonly IReadOnlyList<string> All =
    [
        Available,
        InUse,
        Maintenance,
        Retired
    ];
}

// Rejects any status that is not one of AssetStatus.All.
public sealed class AssetStatusAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string status && AssetStatus.All.Contains(status))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("Select a valid status.");
    }
}
