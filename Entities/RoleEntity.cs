namespace BiyLineApi.Entities;
public sealed class RoleEntity : IdentityRole<int>
{
    public ICollection<UserRoleEntity> UserRoles { get; set; }
}
