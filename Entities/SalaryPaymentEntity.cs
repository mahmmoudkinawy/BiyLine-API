namespace BiyLineApi.Entities;

public sealed class SalaryPaymentEntity
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public int EmployeeId { get; set; }
    public EmployeeEntity Employee { get; set; }

    public int StoreWalletId { get; set; }
    public StoreWalletEntity StoreWallet { get; set; }
}
