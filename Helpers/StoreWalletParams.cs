namespace BiyLineApi.Helpers;

public class StoreWalletParams : PaginationParams
{
    public string? StoreWalletName { get; set; }
    public DateTime? Date { get; set; }
}
