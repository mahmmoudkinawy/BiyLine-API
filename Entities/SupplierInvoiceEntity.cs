namespace BiyLineApi.Entities;

public sealed class SupplierInvoiceEntity
{
    public int Id { get; set; }
    public decimal ShippingPrice { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal Returned { get; set; } 
    public decimal TotalPrice { get; set; }
    public string? AccountNumber { get; set; }
    public string? PaymentMethod { get; set; }
    public ContractOrderEntity ContractOrder { get; set; }
}
