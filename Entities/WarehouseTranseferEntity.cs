namespace BiyLineApi.Entities
{
    public class WarehouseTranseferEntity
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public StoreEntity Store { get; set; }
        public int SourceWarehouseId { get; set; }
        public WarehouseEntity SourceWarehouse { get; set; }
        public int DestinationWarehouseId { get; set; }
        public WarehouseEntity DestinationWarehouse { get; set; }
        public decimal TranseferCost { get; set; }
        public DateTime OperationDate { get; set; }
        public decimal TotalCost 
        {
            get
            {
                return TranseferCost + TranseferDetails.Sum(td => td.TotalPrice);
            }
        }
        public ICollection<WarehouseTranseferDetails> TranseferDetails { get; set; } = new List<WarehouseTranseferDetails>();
    }
}
