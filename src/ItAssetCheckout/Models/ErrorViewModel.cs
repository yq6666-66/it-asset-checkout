namespace ItAssetCheckout.Models;

// Request id shown when the production error page is used.
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
