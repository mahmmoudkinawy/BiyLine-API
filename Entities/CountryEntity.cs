namespace BiyLineApi.Entities;
public sealed class CountryEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? CountryCode { get; set; }
    public string? CurrencyCode { get; set; }
    public string? CurrencySymbol { get; set; }

    public ICollection<GovernorateEntity> Governorates { get; set; }
    public ICollection<StoreEntity> Stores { get; set; }
}
