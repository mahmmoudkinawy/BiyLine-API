namespace BiyLineApi.Entities;
public sealed class ShippingCompanyEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? CountryCode { get; set; }

    public int StoreId { get; set; }
    public StoreEntity Store { get; set; }

    public ICollection<ShippingCompanyGovernorateEntity> ShippingCompanyGovernorates { get; set; }
        = new List<ShippingCompanyGovernorateEntity>();
}
