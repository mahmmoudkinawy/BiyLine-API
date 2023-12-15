namespace BiyLineApi.Entities;

public sealed class TraderShippingCompanyEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int StoreId { get; set; }
    public StoreEntity Store { get; set; }
    public ICollection<GovernorateShippingEntity> GovernorateShippings { get; set; } = new List<GovernorateShippingEntity>();
}
