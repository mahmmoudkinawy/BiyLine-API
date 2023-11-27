namespace BiyLineApi.Entities;

public sealed class SupplierEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? TradeName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TaxCard { get; set; }
    public string? CommercialRecord { get; set; }
    public string? AccountNumber { get; set; }
    public string? PaymentMethod { get; set; }
    public bool IsSuspended { get; set; }
    public string? SupplierType { get; set; }

    public int? UserId { get; set; }
    public UserEntity? User { get; set; }
    public int StoreId { get; set; }
    public StoreEntity? Store { get; set; }
}
