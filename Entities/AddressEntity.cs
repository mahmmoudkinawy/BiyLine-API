namespace BiyLineApi.Entities;

public sealed class AddressEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string AddressDetails { get; set; }
    public int GovernorateId { get; set; }
    public GovernorateEntity Governorate { get; set; }
    public int UserId { get; set; }
    public UserEntity User { get; set; }

}
