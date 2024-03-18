using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BiyLineApi.Entities
{
    public class ReturnReceiptEntity
    {
        [Key]
        public int Id { get; set; }
        public int StoreId { get; set; }
        public virtual StoreEntity Store { get; set; }
        public double? ValueAddedTax { get; set; }
        public double? TotalDiscountPercentage { get; set; }
        public int ReceiptId { get; set; }
        public virtual ReceiptEntity Receipt { get; set; }


        public int VendorId { get; set; }
        public VendorEntity Vendor { get; set; }

        public int Number { get; set; }
        public DateTime CreatedDate { get; set; }
        public double ShippingCost { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public double PaidAmount { get; set; }
        public int StoreWalletId { get; set; }
        public virtual StoreWalletEntity StoreWallet { get; set; }
   

        [NotMapped]
        public double TotalCostBeforeDiscount
        {
            get
            {
                return ReturnReceiptDetails.Sum(sd => sd.UnitCost * sd.Quantity);
            }
        }
        [NotMapped]
        public double? TotalCostAfterDiscount
        {
            get
            {
                double? total = ReturnReceiptDetails.Sum(sd => sd.TotalItemCost);
                total -= total * (TotalDiscountPercentage ?? 1);
                total += ((ValueAddedTax ?? 1) * total);
                return total;
            }
        }
        [NotMapped]
        public double RemainingAmount
        {
            get
            {
                return TotalCostAfterDiscount ?? 0 - PaidAmount;
            }
        }
        public virtual ICollection<ReturnReceiptDetailsEntity> ReturnReceiptDetails { get; set; } = new List<ReturnReceiptDetailsEntity>();
    }
}
