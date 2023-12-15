namespace BiyLineApi.Entities;
public sealed class ExpenseTypeEntity
{
    public int Id { get; set; }
    public string? Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime Created { get; set; }

    public int StoreId { get; set; }
    public StoreEntity Store { get; set; }

    public int StoreWalletId { get; set; }
    public StoreWalletEntity StoreWallet { get; set; }
}
