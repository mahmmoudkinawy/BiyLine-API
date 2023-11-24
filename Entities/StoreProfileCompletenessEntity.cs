namespace BiyLineApi.Entities;
public sealed class StoreProfileCompletenessEntity
{
    public int Id { get; set; }
    public bool IsDetailsComplete { get; set; }
    public bool IsSpecializationsComplete { get; set; }
    public bool IsCoverImageComplete { get; set; }
    public bool IsProfileImageComplete { get; set; }
    public bool IsTaxImageComplete { get; set; }
    public bool IsNationalIdImageComplete { get; set; }
    public bool IsAddressComplete { get; set; }

    public int UserId { get; set; }
    public UserEntity User { get; set; }

    public int StoreId { get; set; }
    public StoreEntity Store { get; set; }
}
