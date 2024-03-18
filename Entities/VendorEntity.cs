using System.ComponentModel.DataAnnotations.Schema;

namespace BiyLineApi.Entities
{
    public class VendorEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int StoreId { get; set; }
        public StoreEntity Store { get; set; }
        [NotMapped]
        public int ReceiptsCount
        {
            get
            {
                return Receipts.Count;
            }
        }
        
        [NotMapped]
        public double ReceiptsCost
        {
            get
            {
                return Receipts.Sum(r => r.TotalCostAfterDiscount??0);
            }
        }
        
        
        
        
        [NotMapped]
        public int ReturnReceiptsCount
        {
            get
            {
                return ReturnReceipts.Count;
            }
        }
        
        [NotMapped]
        public double ReturnReceiptsCost
        {
            get
            {
                return ReturnReceipts.Sum(r => r.TotalCostAfterDiscount??0);
            }
        }
        public virtual ICollection<ReceiptEntity> Receipts { get; set; } = new List<ReceiptEntity>();
        public virtual ICollection<ReturnReceiptEntity> ReturnReceipts { get; set; } = new List<ReturnReceiptEntity>();
    }
}
