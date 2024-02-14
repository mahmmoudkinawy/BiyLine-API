namespace BiyLineApi.Entities;

public sealed class PermissionEntity
{
    public int Id { get; set; }
    public string PermissionName { get; set; }
    
    public ICollection<EmployeeEntity> EmployeePermissions { get; set; } = new List<EmployeeEntity>();
}
