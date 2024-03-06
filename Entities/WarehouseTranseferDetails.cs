namespace BiyLineApi.Entities
{
    public class WarehouseTranseferDetails
    {
        public int Id { get; set; }
        public int WarehouseTranseferId { get; set; }
        public WarehouseTranseferEntity warehouseTransefer { get; set; }
        public int ProductVariationId { get; set; }
        public ProductVariationEntity ProductVariation { get; set; }
        
        
        public int ProductId { get; set; }
        public ProductEntity Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice 
        {
            get
            {
                return Quantity * UnitPrice;
            } 
        }
    }
}
