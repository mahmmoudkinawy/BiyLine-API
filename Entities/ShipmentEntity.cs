namespace BiyLineApi.Entities
{
    public class ShipmentEntity
    {
        public int Id { get; set; }
        public string InvoiceNo { get; set; }
        public int StoreId { get; set; }
        public StoreEntity Store { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string PickUpAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public string TraderPhone { get; set; }
        public decimal ShipmentPrice { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal ObtainedFromClient { get; set; }
        public string ShipmentContent { get; set; }
        public ShipmentStatus Status { get; set; }

    }
}
