using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Vehicles.Models;
using System.Collections.Generic;

namespace Vehicles.Interfaces
{
    public interface ICustomUserManager
    {
        string GetUserId(ClaimsPrincipal principal);
        Task<IdentityResult> CreateAsync(CustomUser user, string password);
        Task<IdentityResult> CreateAsync(CustomUser user);
        Task<string> GenerateEmailConfirmationTokenAsync(CustomUser user);
        //add property _userManager.Options.SignIn.RequireConfirmedAccount
        Task<CustomUser> FindByEmailAsync(string email);
        Task<string> GetUserIdAsync(CustomUser user);
        public IdentityOptions Options{get;set;}
        Task<CustomUser> FindByNameAsync(string userName);
        Task<CustomUser> FindByIdAsync(string userId);
        Task<IdentityResult> AddToRoleAsync(CustomUser user, string role);
        Task<bool> IsInRoleAsync(CustomUser user,string role);
        public Task<IList<Claim>> GetClaimsAsync(CustomUser user);
        public Task<IList<string>> GetRolesAsync(CustomUser user);
        public Task<bool> CheckPasswordAsync(CustomUser user, string password);
        public Task<IdentityResult> DeleteAsync(CustomUser user);
    }
}