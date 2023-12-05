namespace BiyLineApi.Entities;
public sealed class EmployeeEntity
{
    public int Id { get; set; }
    public decimal? Salary { get; set; }
    public string? Address { get; set; }
    public DateTime? EmploymentDate { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public int? WorkingHours { get; set; }
    public string? PaymentPeriod { get; set; }
    public string? PaymentMethod { get; set; }
    public string? VisaNumber { get; set; }

    public int? ImageOwnerId { get; set; }
    public ImageEntity? ImageOwner { get; set; }

    public int? NationalIdImageId { get; set; }
    public ImageEntity? NationalIdImage { get; set; }

    public int StoreId { get; set; }
    public StoreEntity Store { get; set; }

    public int UserId { get; set; }
    public UserEntity User { get; set; }

    public ICollection<StoreWalletEntity> StoreWallets { get; set; } = new List<StoreWalletEntity>();    
}
