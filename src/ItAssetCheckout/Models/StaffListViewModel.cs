namespace ItAssetCheckout.Models;

// Staff rows plus the search text and sort column currently applied.
public class StaffListViewModel
{
    public IReadOnlyList<Staff> Staff { get; init; } = Array.Empty<Staff>();

    public string Query { get; init; } = string.Empty;

    public string Sort { get; init; } = "code";
}
