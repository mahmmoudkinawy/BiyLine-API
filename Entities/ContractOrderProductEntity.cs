namespace BiyLineApi.Entities;
public sealed class ContractOrderProductEntity
{
    public int Id { get; set; }

    public decimal ProductPrice { get; set; }

    public int ProductId { get; set; }
    public ProductEntity Product { get; set; }

    public int QuantityPricingTierId { get; set; }
    public QuantityPricingTierEntity? QuantityPricingTier { get; set; }


    public ICollection<ContractOrderVariationEntity> ContractOrderVariations { get; set; }  = new List<ContractOrderVariationEntity>();
    public ICollection<ContractOrderEntity> ContractOrders { get; set; } = new List<ContractOrderEntity>();
}
