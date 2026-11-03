using ItAssetCheckout.Models;

namespace ItAssetCheckout.Tests;

public class CheckoutRulesTests
{
    [Fact(DisplayName = "UT-03 A second open checkout for the same asset is rejected")]
    public void UT03_SecondOpenCheckout_IsRejected()
    {
        Assert.True(CheckoutRules.IsSecondOpenCheckout(returnAt: null, anotherOpenCheckoutExists: true));
        Assert.False(CheckoutRules.IsSecondOpenCheckout(returnAt: new DateTime(2024, 6, 12), anotherOpenCheckoutExists: true));
        Assert.False(CheckoutRules.IsSecondOpenCheckout(returnAt: null, anotherOpenCheckoutExists: false));
    }
}
