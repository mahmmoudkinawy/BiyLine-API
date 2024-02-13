
using BiyLineApi.policy.Employee;

public class EmployeeWriteHandler  : AuthorizationHandler<EmployeeWriteRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployeeWriteRequirement requirement)
    {
        if (context.User.IsInRole(Constants.Roles.Employee))
        {
            if (context.User.HasClaim(c => c.Type == "Permission" && c.Value == Constants.Policies.EmployeeWrite))
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
