namespace ItAssetCheckout.Models;

// Names of the xUnit cases shown on the in-app Test page.
public static class UnitTestCatalog
{
    public static readonly IReadOnlyList<UnitTestCase> Cases =
    [
        new("UT-01", "Due date before the checkout date is rejected", "CheckoutValidationTests.UT01_DueBeforeCheckout_IsRejected"),
        new("UT-02", "Due date on the checkout date is accepted", "CheckoutValidationTests.UT02_DueOnCheckoutDate_IsAccepted"),
        new("UT-03", "A second open checkout for the same asset is rejected", "CheckoutRulesTests.UT03_SecondOpenCheckout_IsRejected"),
        new("UT-04", "Staff email must be a valid address", "StaffValidationTests.UT04_StaffEmail_MustBeValid"),
        new("UT-05", "Purchase date cannot be in the future", "AssetValidationTests.UT05_FuturePurchaseDate_IsRejected")
    ];
}

// One unit-test case: id, short name, and the xUnit method.
public sealed record UnitTestCase(string Id, string Name, string Method);
