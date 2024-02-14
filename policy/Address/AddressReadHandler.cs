

using BiyLineApi.policy.Address;

public class AddressReadHandler : AuthorizationHandler<AddressReadRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AddressReadRequirement requirement)
    {
        if (context.User.IsInRole(Constants.Roles.Employee))
        {
            if (context.User.HasClaim(c => c.Type == "Permission" && c.Value == Constants.Policies.AddressRead))
            {
                context.Succeed(requirement);
            }
        }
        else if (context.User.IsInRole(Constants.Roles.Trader) || context.User.IsInRole(Constants.Roles.Admin))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}


