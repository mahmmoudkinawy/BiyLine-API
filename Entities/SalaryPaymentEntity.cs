namespace BiyLineApi.Entities;

public sealed class SalaryPaymentEntity
{
    public int Id { get; set; }
    public decimal? Increase { get; set; } 
    public decimal? Deduction { get; set; } 
    public decimal? PaidAmount { get; set; } 
    public string? Notes { get; set; } 
    public string Status { get; set; } 
    public DateTime? PaymentDate { get; set; }
    public int EmployeeId { get; set; }
    public EmployeeEntity Employee { get; set; }
    public int? StoreWalletId { get; set; }
    public StoreWalletEntity? StoreWallet { get; set; }
}
