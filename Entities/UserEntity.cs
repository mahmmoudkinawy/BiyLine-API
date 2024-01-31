namespace BiyLineApi.Entities;
public sealed class UserEntity : IdentityUser<int>
{
    public string? Name { get; set; }
    public string? CountryCode { get; set; }
    public DateTime? LastActive { get; set; }

    public BasketEntity? Basket { get; set; }

    public int? StoreId { get; set; }
    public StoreEntity? Store { get; set; }

    public ICollection<UserRoleEntity> UserRoles { get; set; }
    public ICollection<EmployeeEntity> Employees { get; set; } = new List<EmployeeEntity>();
    public ICollection<ShippingCompanyEntity> shippingCompanyEntities { get; set; } = new List<ShippingCompanyEntity>();


    public ICollection<AddressEntity> Addresses { get; set; } = new List<AddressEntity>();
    public ICollection<CouponUsageEntity> CouponUsages { get; set; } = new List<CouponUsageEntity>();

}
