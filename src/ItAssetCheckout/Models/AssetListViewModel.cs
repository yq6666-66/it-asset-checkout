namespace ItAssetCheckout.Models;

// Asset rows plus the search text and sort column currently applied.
public class AssetListViewModel
{
    public IReadOnlyList<Asset> Assets { get; init; } = Array.Empty<Asset>();

    public string Query { get; init; } = string.Empty;

    public string Sort { get; init; } = "tag";
}
