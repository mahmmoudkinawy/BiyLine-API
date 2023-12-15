namespace BiyLineApi.Entities;
public sealed class ExpenseEntity
{
    public int Id { get; set; }
    public DateTime? Date { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }

    public int ExpenseTypeId { get; set; }
    public ExpenseTypeEntity ExpenseType { get; set; }

    public int StoreId { get; set; }
    public StoreEntity Store { get; set; }

    public int StoreWalletId { get; set; }
    public StoreWalletEntity StoreWallet { get; set; }
    
    public int? ReceiptImageId { get; set; }
    public ImageEntity? ReceiptImage { get; set; }
}
