namespace BiyLineApi.Entities
{
    public class ShippingCompanyGovernorateDetailsEntity
    {
        public int Id { get; set; }
        //public string Name { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal PickUpCost { get; set; }
        public decimal ReturnCost { get; set; }
        public double Weight { get; set; }
        public decimal OverweightFees { get; set; }
        public bool Status { get; set; }
        public int GovernorateId { get; set; }
        public GovernorateEntity Governorate { get; set; }
        public int ShippingCompanyId { get; set; }
        public ShippingCompanyEntity ShippingCompany { get; set; }
    }
}
