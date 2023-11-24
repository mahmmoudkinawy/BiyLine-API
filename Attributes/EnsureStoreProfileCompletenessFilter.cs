namespace BiyLineApi.Attributes;
public sealed class EnsureStoreProfileCompletenessFilter : IAsyncActionFilter
{
    private readonly BiyLineDbContext _context;
    private readonly UserManager<UserEntity> _userManager;

    public EnsureStoreProfileCompletenessFilter(BiyLineDbContext context, UserManager<UserEntity> userManager)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        var userId = user.GetUserById();

        var currentUser = await _userManager.FindByIdAsync(userId.ToString());

        var userInTraderRole = await _userManager.IsInRoleAsync(currentUser, Constants.Roles.Trader);

        if (userInTraderRole)
        {
            var storeProfileCompleteness = await _context.StoresProfilesCompleteness
                .FirstOrDefaultAsync(spc => spc.UserId == userId);

            if (storeProfileCompleteness != null &&
                typeof(StoreProfileCompletenessEntity)
                    .GetProperties()
                    .Where(p => p.PropertyType == typeof(bool))
                    .All(p => (bool)p.GetValue(storeProfileCompleteness, null)))
            {
                await next();
            }
            else
            {
                context.Result = new ObjectResult("Trader profile is not completed.")
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}