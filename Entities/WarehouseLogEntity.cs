namespace BiyLineApi.Entities
{
    public class WarehouseLogEntity
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public WarehouseEntity Warehouse { get; set; }
        public double Quantity { get; set; }
        public int ProductId { get; set; }
        public ProductEntity Product { get; set; }
        public WarehouseLogType Type { get; set; }
        public int ProductVariationId { get; set; }
        public ProductVariationEntity ProductVariation { get; set; }
        public int? DocumentId { get; set; }
        public DocumentType DocumentType { get; set; }
        public Guid Code { get; set; }
        public DateTime OperationDate { get; set; }
        public decimal? SellingPrice { get; set; }
    }
}
