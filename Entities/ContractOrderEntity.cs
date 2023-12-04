using Bogus.DataSets;

namespace BiyLineApi.Entities;
public sealed class ContractOrderEntity
{ 
    public int Id { get; set; }
    public string Status { get; set; }

    public int FromStoreId { get; set; }
    public StoreEntity FromStore { get; set; }
    
    public int ToStoreId { get; set; }
    public StoreEntity ToStore { get; set; }

    public decimal TotalPrice { get; set; }

    public string? Note { get; set; }

    public DateTime Date { get; set; }

    public int? SupplierInvoiceId { get; set; }
    public SupplierInvoiceEntity SupplierInvoice { get; set; }

    public ICollection<ContractOrderProductEntity> ContractOrderProducts { get; set; } = new List<ContractOrderProductEntity>();
}
    