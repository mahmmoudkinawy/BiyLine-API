namespace BiyLineApi.Entities;
public sealed class InventoryEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ShippingAddress { get; set; }
    public DateTime? Created { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsMain { get; set; }
}