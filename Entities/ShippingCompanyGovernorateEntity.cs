namespace BiyLineApi.Entities;
public sealed class ShippingCompanyGovernorateEntity
{
    public int ShippingCompanyId { get; set; }
    public ShippingCompanyEntity ShippingCompany { get; set; }

    public int GovernorateId { get; set; }
    public GovernorateEntity Governorate { get; set; }
}
