namespace BiyLineApi.Entities;
public sealed class QuantityPricingTierEntity
{
    public int Id { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public decimal? Price { get; set; }

    public int ProductId { get; set; }
    public ProductEntity Product { get; set; }

    public ICollection<ContractOrderProductEntity> ContractOrderProducts { get; set; } = new List<ContractOrderProductEntity>();  

}
