namespace BiyLineApi.Entities;
public sealed class StoreChatMessageEntity
{
    public int Id { get; set; }
    public string? Content { get; set; }
    public DateTime? Timestamp { get; set; }

    public int SenderUserId { get; set; }
    public UserEntity SenderUser { get; set; }

    public int ReceiverUserId { get; set; }
    public UserEntity ReceiverUser { get; set; }

    public int StoreId { get; set; }
    public StoreEntity Store { get; set; }
}