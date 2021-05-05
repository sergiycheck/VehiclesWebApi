using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vehicles.Interfaces;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using vehicles.Models;

namespace vehicles.Authorization.AuthorizationsManagers
{
    public class OwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, OwnerResource>
    {

        private readonly ICustomUserManager _userManager;

        public OwnerAuthorizationHandler(ICustomUserManager
            userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(
                                   AuthorizationHandlerContext context,
                                   OperationAuthorizationRequirement requirement,
                                   OwnerResource resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for CRUD permission, return.

            if (requirement.Name != AuthorizationConstants.CreateOperationName &&
                requirement.Name != AuthorizationConstants.ReadOperationName &&
                requirement.Name != AuthorizationConstants.UpdateOperationName &&
                requirement.Name != AuthorizationConstants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }
            var userId = context.User.Claims.Single(x => x.Type == "id").Value;

            if (resource.OwnersId.Contains(userId))//if user is owner of selected item
            {
                context.Succeed(requirement);//contact OperationAuthorizationRequirement is Succeed
            }

            return Task.CompletedTask;// Task result affects on future event handlers
        }
    }
}
