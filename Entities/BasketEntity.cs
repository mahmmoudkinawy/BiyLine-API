namespace BiyLineApi.Entities;
public sealed class BasketEntity
{
    public int Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public decimal TotalPrice { get; set; }

    public int? UserId { get; set; }
    public UserEntity? User { get; set; }
    
    public int? AddressId { get; set; }
    public AddressEntity? Address { get; set; }

    public int? StoreId { get; set; }
    public StoreEntity? Store { get; set; }

    public ICollection<BasketItemEntity> BasketItems { get; set; } = new List<BasketItemEntity>();


}
