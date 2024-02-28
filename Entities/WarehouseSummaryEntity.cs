namespace BiyLineApi.Entities
{
    public class WarehouseSummaryEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual ProductEntity Product { get; set; }
        public int WarehouseId { get; set; }
        public WarehouseEntity Warehouse { get; set; }
        public int Quantity { get; set; }
    }
}
