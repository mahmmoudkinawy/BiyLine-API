namespace BiyLineApi.Helpers;
public sealed class WarehousesProductParams : PaginationParams
{
    /// <summary>
    /// Optional. The order by parameter. Available options: priceAsc, priceDesc, default will be name.
    /// </summary>
    public string? OrderBy { get; set; } = "priceAsc";
    public string? CodeNumber { get; set; }
    public string? Name { get; set; }
    public bool? Status { get; set; } = true;
}