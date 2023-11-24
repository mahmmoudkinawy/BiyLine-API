namespace BiyLineApi.Entities;
public sealed class CouponEntity
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public StoreEntity? Store { get; set; }
    public int? StoreId { get; set; }
}