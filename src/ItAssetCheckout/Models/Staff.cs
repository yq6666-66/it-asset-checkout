using System.ComponentModel.DataAnnotations;

namespace ItAssetCheckout.Models;

// A person who can borrow equipment.
public class Staff
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Staff code is required.")]
    [StringLength(32, MinimumLength = 2)]
    [RegularExpression(@"^[A-Za-z0-9-]+$", ErrorMessage = "Staff code can use letters, numbers, and hyphens only.")]
    [Display(Name = "Staff code")]
    public string StaffCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, MinimumLength = 1)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required.")]
    [StringLength(80, MinimumLength = 1)]
    public string Department { get; set; } = string.Empty;
}
