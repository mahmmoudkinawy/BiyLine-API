using System.ComponentModel.DataAnnotations.Schema;

namespace BiyLineApi.Entities
{
    public class ShipmentDetailsEntity
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public ShipmentEntity Shipment { get; set; }
        public int ProductId { get; set; }
        public ProductEntity Product { get; set; }
        public int ProductVariationEntity { get; set; }
        public ProductVariationEntity ProductVariation { get; set; }
        public double UnitCost { get; set; }
        public double DiscountPercentage { get; set; }
        public double Quantity { get; set; }
        [NotMapped]
        public double TotalItemCost 
        {
            get
            {
                return (UnitCost - (UnitCost* (DiscountPercentage / 100.00))) * Quantity;
            } 
        }

    }
}
