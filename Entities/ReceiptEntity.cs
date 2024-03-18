using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static BiyLineApi.Features.Shipment.Commands.CreateShipment.CreateShipmentCommand;

namespace BiyLineApi.Entities
{
    public class ReceiptEntity
    {
        [Key]
        public int Id { get; set; }
        public int StoreId { get; set; }
        public virtual StoreEntity Store { get; set; }
        public double? ValueAddedTax { get; set; }
        public double? TotalDiscountPercentage { get; set; }
        public int VendorId { get; set; }
        public virtual VendorEntity Vendor { get; set; }
        public int Number { get; set; }
        public DateTime CreatedDate { get; set; }
        public double ShippingCost { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public double PaidAmount { get; set; }
        public int StoreWalletId { get; set; }
        public virtual StoreWalletEntity StoreWallet { get; set; }
        public bool Recieved { get; set; }
        public DateTime RecievedDate { get; set; }
        public int WarehouseId { get; set; }
        public virtual WarehouseEntity Warehouse { get; set; }

        [NotMapped]
        public double TotalCostBeforeDiscount
        {
            get
            {
                return ReceiptDetails.Sum(sd => sd.UnitCost * sd.Quantity);
            }
        }
        [NotMapped]
        public double? TotalCostAfterDiscount
        {
            get
            {
                double? total = ReceiptDetails.Sum(sd => sd.TotalItemCost);
                total -= total * (TotalDiscountPercentage ?? 1);
                total += ((ValueAddedTax ?? 1) * total);
                return total;
            }
        }
        [NotMapped]
        public double  RemainingAmount 
        {
            get
            {
                return TotalCostAfterDiscount??0 - PaidAmount;
            }    
        }
        public virtual ICollection<ReceiptDetailsEntity> ReceiptDetails { get; set; } = new List<ReceiptDetailsEntity>();
    }
}
