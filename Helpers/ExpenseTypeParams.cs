namespace BiyLineApi.Helpers;
public sealed class ExpenseTypeParams : PaginationParams
{
    public DateTime? Date { get; set; }
    public string? Type { get; set; }
    public string? Wallet { get; set; }
}