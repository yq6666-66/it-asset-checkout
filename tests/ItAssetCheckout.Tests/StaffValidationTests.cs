using System.ComponentModel.DataAnnotations;
using ItAssetCheckout.Models;

namespace ItAssetCheckout.Tests;

public class StaffValidationTests
{
    [Fact(DisplayName = "UT-04 Staff email must be a valid address")]
    public void UT04_StaffEmail_MustBeValid()
    {
        var invalid = Person("not-an-email");
        var invalidResults = Validate(invalid);
        Assert.Contains(invalidResults, result =>
            result.MemberNames.Contains(nameof(Staff.Email))
            && result.ErrorMessage == "Enter a valid email address.");

        var valid = Person("ada.okonkwo@example.com");
        Assert.Empty(Validate(valid));
    }

    private static Staff Person(string email)
    {
        return new Staff
        {
            StaffCode = "S-1001",
            FullName = "Ada Okonkwo",
            Email = email,
            Department = "Finance"
        };
    }

    private static List<ValidationResult> Validate(Staff person)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(person, new ValidationContext(person), results, validateAllProperties: true);
        return results;
    }
}
