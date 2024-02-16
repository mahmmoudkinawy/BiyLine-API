
using BiyLineApi.policy.Product;

    public class ProductWriteHandler : AuthorizationHandler<ProductWriteRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProductWriteRequirement requirement)
        {
            if (context.User.IsInRole(Constants.Roles.Employee))
            {
                if (context.User.HasClaim(c => c.Type == "Permission" && c.Value == Constants.Policies.AddressWrite))
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
