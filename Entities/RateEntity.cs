namespace BiyLineApi.Entities;
public sealed class RateEntity
{
    public int Id { get; set; }
    public decimal? Rating { get; set; }
    public string? Review { get; set; }
    public DateTime? RatingDate { get; set; }

    public int? ProductId { get; set; }

    public int UserId { get; set; }
    public UserEntity User { get; set; }
    public ProductEntity? Product { get; set; }
}
