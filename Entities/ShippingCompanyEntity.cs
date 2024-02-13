namespace BiyLineApi.Entities;
public sealed class ShippingCompanyEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? CountryCode { get; set; }
    public string? Address { get; set; }
    public int? Collection { get; set; }
    public DeliveryCases? DeliveryCases{ get; set; }
    public PaymentMethodEnum? PaymentMethod { get; set; }
    public int? StoreId { get; set; }
    public StoreEntity? Store { get; set; }
    //public int? IDImageId { get; set; }
    //public ImageEntity? IDImage { get; set; }
    
    //public int? CommercialRegisterImageId { get; set; }
    //public ImageEntity? CommercialRegisterImage { get; set; }
    //public int? ProfileImageId { get; set; }
    //public ImageEntity? ProfileImage { get; set; }


    public int UserEntityId { get; set; }
    public UserEntity UserEntity { get; set; }
    public ICollection<ShippingCompanyGovernorateDetailsEntity> ShippingCompanyGovernorateDetails { get; set; }
        = new List<ShippingCompanyGovernorateDetailsEntity>();
    
    public ICollection<ImageEntity> Images { get; set; } = new List<ImageEntity>();
}
