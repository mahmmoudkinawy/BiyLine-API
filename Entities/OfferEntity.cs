namespace BiyLineApi.Entities;
public sealed class OfferEntity
{
    public int Id { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public ICollection<ProductEntity> Products { get; set; }
}
