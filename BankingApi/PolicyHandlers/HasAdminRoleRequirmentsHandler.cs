namespace BankingApi.PolicyHandlers
{
    using Microsoft.AspNetCore.Authorization;
    using System.Security.Claims;

    public class HasAdminRoleRequirmentsHandler : AuthorizationHandler<HasAdminRoleRequirments>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasAdminRoleRequirments requirement)
        {
            var hasClaim = context.User.HasClaim(x => x.Type == ClaimTypes.Role);
            var temp = context.User.Claims.ToList();
            if (!hasClaim)
            {
                return Task.CompletedTask;
            }

            var role = context.User.FindFirst(x => x.Type == ClaimTypes.Role)?.Value;
            if (role == "Administrator")
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
