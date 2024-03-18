using System.ComponentModel.DataAnnotations.Schema;

namespace BiyLineApi.Entities
{
    public class ReceiptDetailsEntity
    {
        public int Id { get; set; }
        public int ReceiptId { get; set; }
        public ReceiptEntity Receipt { get; set; }
        public int ProductId { get; set; }
        public ProductEntity Product { get; set; }
        public int ProductVariationId { get; set; }
        public ProductVariationEntity ProductVariation { get; set; }
        public double UnitCost { get; set; }
        public double DiscountPercentage { get; set; }
        public double Quantity { get; set; }
        [NotMapped]
        public double TotalItemCost
        {
            get
            {
                return (UnitCost - (UnitCost * (DiscountPercentage / 100.00))) * Quantity;
            }
        }
    }
}
