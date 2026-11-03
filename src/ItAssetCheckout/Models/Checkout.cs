using System.ComponentModel.DataAnnotations;

namespace ItAssetCheckout.Models;

// One loan of an asset to a staff member. ReturnAt is empty while the loan is open.
public class Checkout : IValidatableObject
{
    public int Id { get; set; }

    [Display(Name = "Asset")]
    public int AssetId { get; set; }

    public Asset? Asset { get; set; }

    [Display(Name = "Staff member")]
    public int StaffId { get; set; }

    public Staff? Staff { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Checkout date")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime CheckoutAt { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Due date")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime DueAt { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Return date")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? ReturnAt { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }

    // Checks required dates and that due and return dates are not before checkout.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckoutAt == default)
        {
            yield return new ValidationResult("Checkout date is required.", new[] { nameof(CheckoutAt) });
            yield break;
        }

        var checkoutDay = CheckoutAt.Date;
        if (checkoutDay < new DateTime(1990, 1, 1))
        {
            yield return new ValidationResult("Checkout date cannot be before 1990-01-01.", new[] { nameof(CheckoutAt) });
        }

        if (checkoutDay > DateTime.Today)
        {
            yield return new ValidationResult("Checkout date cannot be in the future.", new[] { nameof(CheckoutAt) });
        }

        if (DueAt == default)
        {
            yield return new ValidationResult("Due date is required.", new[] { nameof(DueAt) });
        }
        else if (DueAt.Date < checkoutDay)
        {
            yield return new ValidationResult("Due date cannot be before the checkout date.", new[] { nameof(DueAt) });
        }

        if (ReturnAt is DateTime returned)
        {
            if (returned.Date < checkoutDay)
            {
                yield return new ValidationResult("Return date cannot be before the checkout date.", new[] { nameof(ReturnAt) });
            }

            if (returned.Date > DateTime.Today)
            {
                yield return new ValidationResult("Return date cannot be in the future.", new[] { nameof(ReturnAt) });
            }
        }
    }
}
