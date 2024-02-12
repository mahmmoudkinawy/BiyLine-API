namespace BiyLineApi.Entities;

public sealed class GovernorateShippingEntity
{
    public int Id { get; set; }
    public decimal ShippingPrice { get; set; }
    public decimal PickupPrice { get; set; }
    public decimal ReturnCost { get; set; }
    public decimal WeightTo { get; set; }
    public decimal PricePerExtraKilo { get; set; }
    public int GovernorateId { get; set; }
    public GovernorateEntity Governorate { get; set; }
    public int TraderShippingCompanyId { get; set; }
    public TraderShippingCompanyEntity TraderShippingCompany { get; set; }
}