namespace ItAssetCheckout.Models;

// Decides whether a checkout would be a second open loan for the same asset.
public static class CheckoutRules
{
    // True when this checkout has no return date and the asset already has another open checkout.
    public static bool IsSecondOpenCheckout(DateTime? returnAt, bool anotherOpenCheckoutExists)
    {
        return returnAt is null && anotherOpenCheckoutExists;
    }
}
