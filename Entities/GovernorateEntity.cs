namespace BiyLineApi.Entities;
public sealed class GovernorateEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? GovernorateCode { get; set; }

    public int CountryId { get; set; }
    public CountryEntity Country { get; set; }

    public ICollection<ShippingCompanyGovernorateDetailsEntity> ShippingCompanyGovernorates { get; set; }
        = new List<ShippingCompanyGovernorateDetailsEntity>();
    public ICollection<RegionEntity> Regions { get; set; }
}
