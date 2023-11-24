namespace BiyLineApi.Entities;
public sealed class SpecializationEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public int StoreId { get; set; }
    public StoreEntity Store { get; set; }

    public ICollection<SubSpecializationEntity> SubSpecializations { get; set; } = new List<SubSpecializationEntity>();
}