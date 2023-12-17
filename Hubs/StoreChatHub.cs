namespace BiyLineApi.Hubs;

/// <summary>
/// Authorized Message Hub that's accessed via 'hubs/storechathub'.
/// </summary>///
[SignalRHub]
[Authorize]
public sealed class StoreChatHub : Hub
{
    private readonly BiyLineDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public StoreChatHub(
        BiyLineDbContext context,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    /// <summary>
    /// ReceiveMessageThread Hub method method that's used for receiving a message.
    /// </summary>
    [SignalRMethod("StoreReceiveMessage")]
    public async Task SendMessage(string storeOwner, string message)
    {
        var userId = Context.User.GetUserById();

        var receiverUser = await _context.Users
            .FirstOrDefaultAsync(sw => sw.UserName.Contains(storeOwner) || sw.Email.Contains(storeOwner));

        if (receiverUser != null)
        {
            var messageToCreate = new StoreChatMessageEntity
            {
                Content = message,
                SenderUserId = userId,
                ReceiverUserId = userId,
                StoreId = receiverUser.StoreId.Value,
                Timestamp = _dateTimeProvider.GetCurrentDateTimeUtc()
            };

            _context.StoreMessages.Add(messageToCreate);
            await _context.SaveChangesAsync();
        }

        await Clients.Group(storeOwner)
            .SendAsync("StoreReceiveMessage", Context.User.Identity.Name, message);
    }

    /// <summary>
    /// JoinStoreOwnerGroup Hub method method that's used jointing a customer with store owner.
    /// </summary>
    [SignalRMethod("JoinStoreOwnerGroup")]
    public async Task JoinStoreOwnerGroup(string storeOwnerId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, storeOwnerId);
    }

    /// <summary>
    /// LeaveStoreOwnerGroup Hub method method that's used disconnecting a customer with store owner.
    /// </summary>
    [SignalRMethod("LeaveStoreOwnerGroup")]
    public async Task LeaveStoreOwnerGroup(string storeOwnerId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, storeOwnerId);
    }
}
