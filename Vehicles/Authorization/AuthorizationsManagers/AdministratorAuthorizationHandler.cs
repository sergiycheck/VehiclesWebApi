using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using vehicles.Models;

namespace vehicles.Authorization.AuthorizationsManagers
{
    public class AdministratorAuthorizationHandler:
        AuthorizationHandler<OperationAuthorizationRequirement, OwnerResource>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            OwnerResource resource)
        {
            if(context.User==null) 
                return Task.CompletedTask;

            if(context.User.IsInRole(AuthorizationConstants.ContactAdministratorsRole))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
