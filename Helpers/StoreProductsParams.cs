namespace BiyLineApi.Helpers;
public sealed class StoreProductsParams : PaginationParams
{
    public bool? IsInStock { get; set; } = true;
    public string? Name { get; set; }
    public string? CodeNumber { get; set; }
}