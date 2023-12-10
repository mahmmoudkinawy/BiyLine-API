using AutoMapper.Configuration.Conventions;

namespace BiyLineApi.Entities;

public sealed class CashDepositePermissionEntity
{
    public int Id { get; set; }
    public string? Reason { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public int StoreWalletId { get; set; }
    public StoreWalletEntity StoreWallet { get; set; }
}

