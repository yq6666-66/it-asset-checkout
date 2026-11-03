namespace ItAssetCheckout.Models;

// Checkout rows plus the search text and sort column currently applied.
public class CheckoutListViewModel
{
    public IReadOnlyList<Checkout> Checkouts { get; init; } = Array.Empty<Checkout>();

    public string Query { get; init; } = string.Empty;

    public string Sort { get; init; } = "checkout";
}
