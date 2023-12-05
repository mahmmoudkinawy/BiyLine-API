namespace BiyLineApi.Entities;
public sealed class ContractOrderVariationEntity
{
    public int Id { get; set; }
    public int Quantity { get; set; }

    public int ProductVariationId { get; set; }
    public ProductVariationEntity ProductVariation { get; set; }

    public ICollection<ContractOrderProductEntity> ContractOrderProducts { get; set; } = new List<ContractOrderProductEntity>();
}
