namespace BiyLineApi.Attributes;
public sealed class EnsureSingleStoreFilter : IAsyncActionFilter
{
    private readonly BiyLineDbContext _context;

    public EnsureSingleStoreFilter(BiyLineDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        var userId = user.GetUserById();

        var store = await _context.Stores.AnyAsync(s => s.OwnerId == userId);

        var storeProfileCompleteness = await _context.StoresProfilesCompleteness
                .FirstOrDefaultAsync(spc => spc.UserId == userId);

        var storeProfileCompletenessResult = typeof(StoreProfileCompletenessEntity)
               .GetProperties()
               .Where(p => p.PropertyType == typeof(bool) && p.Name != "IsSpecializationsComplete")
               .All(p => (bool)p.GetValue(new StoreProfileCompletenessEntity(), null) || (p.Name == "IsSpecializationsComplete" && !(bool)p.GetValue(new StoreProfileCompletenessEntity(), null)));

        if (storeProfileCompleteness != null &&
            storeProfileCompletenessResult &&
            store)
        {
            context.Result = new ObjectResult("User is not allowed to create a new store")
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
        else
        {
            await next();
        }
    }
}
