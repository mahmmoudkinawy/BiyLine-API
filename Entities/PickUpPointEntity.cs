namespace BiyLineApi.Entities
{
    public class PickUpPointEntity
    {
        public int Id { get; set; }
        public int GovernorateId { get; set; }
        public GovernorateEntity Governorate { get; set; }
        public string Address { get; set; }
        public int StoreId { get; set; }
        public StoreEntity Store { get; set; }
    }
}
