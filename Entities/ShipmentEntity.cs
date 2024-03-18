using System.ComponentModel.DataAnnotations.Schema;

namespace BiyLineApi.Entities
{
    public class ShipmentEntity
    {
        public int Id { get; set; }
        //public string Serial { get; set; }
        public int WarehouseId { get; set; }
        public WarehouseEntity Warehouse { get; set; }

        public int StoreId { get; set; }
        public StoreEntity Store { get; set; } 


        public string? ClientName { get; set; }
        public string? ClientPhone { get; set; }
        public double? TotalDiscountPercentage { get; set; }
        public double? ValueAddedTax { get; set; }
        public CashOutType CashOutType { get; set; }
        public ShipmentStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public CollectingShipmentCost CollectingShipmentCost { get; set; }
        public CollectingDeliveryCost CollectingDeliveryCost { get; set; }

        [NotMapped]
        public double TotalCostBeforeDiscount
        {
            get
            {
                return ShipmentDetails.Sum(sd => sd.UnitCost * sd.Quantity);
            }
        }
        public int GovernorateId { get; set; }
        public GovernorateEntity Governorate { get; set; }

        public string DetailedAddress { get; set; }
        public double ShippingCost { get; set; }
        public int? ShippingCompanyId { get; set; }
        public ShippingCompanyEntity? ShippingCompany { get; set; }
        public int? PickUpPointId { get; set; }
        public PickUpPointEntity? PickUpPoint { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }

        public int? StoreWalletId { get; set; }
        public StoreWalletEntity? StoreWallet { get; set; }

        public double PaidAmount { get; set; }


        [NotMapped]
        public double? TotalCostAfterDiscount
        {
            get
            {
                double? total = ShipmentDetails.Sum(sd => sd.TotalItemCost);
                total -= total * (TotalDiscountPercentage ?? 1);
                total += ((ValueAddedTax ?? 1) * total);
                return total;
            }
        }


        public ICollection<ShipmentDetailsEntity> ShipmentDetails { get; set; } = new List<ShipmentDetailsEntity>();
    }
}
