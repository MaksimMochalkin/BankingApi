namespace BankingApi.PolicyHandlers
{
    using Microsoft.AspNetCore.Authorization;

    public class HasAdminRoleRequirments : IAuthorizationRequirement
    {
        private string Role { get; set; }

        public HasAdminRoleRequirments(string role)
        {
            Role = role;
        }
    }
}
