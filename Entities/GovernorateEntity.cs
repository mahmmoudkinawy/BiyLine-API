namespace BiyLineApi.Entities;
public sealed class GovernorateEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? GovernorateCode { get; set; }

    public int CountryId { get; set; }
    public CountryEntity Country { get; set; }

    public ICollection<ShippingCompanyGovernorateEntity> ShippingCompanyGovernorates { get; set; }
        = new List<ShippingCompanyGovernorateEntity>();
    public ICollection<RegionEntity> Regions { get; set; }
    public ICollection<AddressEntity> Addresses { get; set; } = new List<AddressEntity>();
}
