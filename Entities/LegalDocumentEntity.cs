namespace BiyLineApi.Entities;
public sealed class LegalDocumentEntity
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Type { get; set; }
    public string? Content { get; set; }
    public DateTime? UploadDate { get; set; }
    public DateTime? LastModified { get; set; }
}
