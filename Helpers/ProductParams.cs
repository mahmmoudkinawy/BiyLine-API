namespace BiyLineApi.Helpers;
public sealed class ProductParams : PaginationParams
{
    public string? Predicate { get; set; }
    public bool? IsInStock { get; set; } = true;
}
