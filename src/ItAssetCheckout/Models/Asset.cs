using System.ComponentModel.DataAnnotations;

namespace ItAssetCheckout.Models;

// One tracked piece of equipment.
public class Asset
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tag number is required.")]
    [StringLength(32, MinimumLength = 2)]
    [RegularExpression(@"^[A-Za-z0-9-]+$", ErrorMessage = "Tag number can use letters, numbers, and hyphens only.")]
    [Display(Name = "Tag number")]
    public string TagNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required.")]
    [StringLength(50, MinimumLength = 1)]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status is required.")]
    [AssetStatus]
    public string Status { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Purchase date")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [ReasonablePurchaseDate]
    public DateTime PurchaseDate { get; set; }

    [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "Value must be between 0 and 9999999.99.")]
    [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
    public decimal Value { get; set; }
}
