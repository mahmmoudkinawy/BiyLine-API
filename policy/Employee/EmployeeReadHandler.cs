
using BiyLineApi.policy.Employee;


public class EmployeeReadHandler : AuthorizationHandler<EmployeeReadRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployeeReadRequirement requirement)
    {
        if (context.User.IsInRole(Constants.Roles.Employee))
        {
            if (context.User.HasClaim(c => c.Type == "Permission" && c.Value == Constants.Policies.EmployeeRead))
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
