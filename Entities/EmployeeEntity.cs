namespace BiyLineApi.Entities;
public sealed class EmployeeEntity
{
    public int Id { get; set; }
    public decimal? Salary { get; set; }
    public string? Address { get; set; }
    public DateTime? EmploymentDate { get; set; }
    public int? WorkingHours { get; set; }
    public PaymentPeriodEnum? PaymentPeriod { get; set; }
    public PaymentMethodEnum? PaymentMethod { get; set; }
    public string? VisaNumber { get; set; }

    public int? NationalIdImageId { get; set; }
    public ImageEntity? NationalIdImage { get; set; }

    public int StoreId { get; set; }
    public StoreEntity Store { get; set; }

    public int UserId { get; set; }
    public UserEntity User { get; set; }
}
