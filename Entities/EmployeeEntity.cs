
namespace BiyLineApi.Entities;
public sealed class EmployeeEntity
{
    public int Id { get; set; }
    public decimal? Salary { get; set; }
    public string? Address { get; set; }
    public DateTime? EmploymentDate { get; set; } = DateTime.UtcNow;

    public int StoreId { get; set; }
    public StoreEntity Store { get; set; }

    public int UserId { get; set; }
    public UserEntity User { get; set; }

    public ICollection<StoreWalletEntity> StoreWallets { get; set; } = new List<StoreWalletEntity>();

    public ICollection<SalaryPaymentEntity> SalaryPayments { get; set; } = new List<SalaryPaymentEntity>();
    public ICollection<PermissionEntity> Permissions { get; set; }  = new List<PermissionEntity>();

}
