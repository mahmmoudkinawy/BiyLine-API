namespace BiyLineApi.Entities;
public sealed class RegionEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public int GovernorateId { get; set; }
    public GovernorateEntity Governorate { get; set; }
}