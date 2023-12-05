namespace BiyLineApi.Helpers;
public sealed class StockParams : PaginationParams
{
    public DateTime? Date { get; set; }
    public int? SourceWarehouseId { get; set; }
    public int? DestinationWarehouseId { get; set; }
    public string? InvoiceNumber { get; set; }
}
