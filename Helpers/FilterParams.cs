namespace BiyLineApi.Helpers;
public sealed class FilterParams : PaginationParams
{
    public string? Predicate { get; set; }
    public bool? IsInStock { get; set; } = true;
}
