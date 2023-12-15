namespace BiyLineApi.Helpers;
public sealed class ExpenseParams : PaginationParams
{
    public DateTime? Date { get; set; }
    public string? Wallet { get; set; }
}