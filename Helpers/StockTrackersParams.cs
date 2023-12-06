namespace BiyLineApi.Helpers;

public sealed class StockTrackersParams : PaginationParams
{
    /// <summary>
    /// Optional. The order by parameter. Available options: priceAsc, priceDesc, name, default will be Id.
    /// </summary>
    public string? OrderBy { get; set; } = "priceAsc";
}