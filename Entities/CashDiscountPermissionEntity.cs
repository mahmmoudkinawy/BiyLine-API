namespace BiyLineApi.Entities;

public sealed class CashDiscountPermissionEntity
{
    public int Id { get; set; }
    public string? Reason { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public int StoreWalletId { get; set; }
    public StoreWalletEntity StoreWallet { get; set; }
}
