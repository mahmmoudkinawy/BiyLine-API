namespace BiyLineApi.Entities;
public sealed class SubSpecializationEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public int? SubSpecializationImageId { get; set; }
    public ImageEntity? SubSpecializationImage { get; set; }

    public int? SpecializationId { get; set; }
    public SpecializationEntity? Specialization { get; set; }
}