using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using Vehicles.Models;

namespace Vehicles.Interfaces
{
    public interface ICustomSignInManager
    {
        Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
        Task SignInAsync(CustomUser user, bool isPersistent, string authenticationMethod = null);
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);
        Task<SignInResult> PasswordSignInAsync(CustomUser user, string password, bool isPersistent, bool lockoutOnFailure);
        Task SignOutAsync();
        bool IsSignedIn(ClaimsPrincipal principal);
    }
}