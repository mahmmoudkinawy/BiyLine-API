namespace BiyLineApi.Entities;
public sealed class UserRoleEntity : IdentityUserRole<int>
{
    public UserEntity User { get; set; }
    public RoleEntity Role { get; set; }
}
