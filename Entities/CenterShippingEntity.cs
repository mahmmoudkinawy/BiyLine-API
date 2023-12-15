namespace BiyLineApi.Entities;

public sealed class CenterShippingEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal ShippingPrice { get; set; }
    public decimal PickupPrice { get; set; }
    public decimal ReturnCost { get; set; }
    public decimal WeightTo { get; set; }
    public decimal PricePerExtraKilo { get; set; }
    public string Status { get; set; }
    public int GovernorateShippingId { get; set; }
    public GovernorateShippingEntity GovernorateShipping { get; set; }
}