namespace BiyLineApi.Entities;
public sealed class ShippingCompanyGovernorateEntity
{
    public decimal ShippingPrice { get; set; }

    public int ShippingCompanyId { get; set; }
    public ShippingCompanyEntity ShippingCompany { get; set; }

    public int GovernorateId { get; set; }
    public GovernorateEntity Governorate { get; set; }

}
