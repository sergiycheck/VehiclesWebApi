﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vehicles.Interfaces;
using Vehicles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Vehicles.AuthorizationsManagers
{
    public class CustomUserManager : UserManager<CustomUser>, ICustomUserManager
    {
        public CustomUserManager(IUserStore<CustomUser> store, 
                                IOptions<IdentityOptions> optionsAccessor,
                                IPasswordHasher<CustomUser> passwordHasher, 
                                IEnumerable<IUserValidator<CustomUser>> userValidators,
                                IEnumerable<IPasswordValidator<CustomUser>> passwordValidators,    
                                ILookupNormalizer keyNormalizer,
                                IdentityErrorDescriber errors, 
                                IServiceProvider services, 
                                ILogger<UserManager<CustomUser>> logger)
                                                            : base(store, 
                                                                    optionsAccessor, 
                                                                    passwordHasher, 
                                                                    userValidators, 
                                                                    passwordValidators, 
                                                                    keyNormalizer, 
                                                                    errors, 
                                                                    services, 
                                                                    logger)
        {
        }
        public new IdentityOptions Options{get=> base.Options;set=>base.Options=value;}//not sure here
        public override IQueryable<CustomUser> Users{get =>base.Users;}
        public override Task<IdentityResult> CreateAsync(CustomUser user, string password)
        {
            return base.CreateAsync(user, password);
        }

        public override Task<IdentityResult> CreateAsync(CustomUser user)
        {
            return base.CreateAsync(user);
        }

        public override Task<CustomUser> FindByEmailAsync(string email)
        {
            return base.FindByEmailAsync(email);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(CustomUser user)
        {
            return base.GenerateEmailConfirmationTokenAsync(user);
        }

        public override string GetUserId(ClaimsPrincipal principal)
        {
            return base.GetUserId(principal);
        }

        public override Task<string> GetUserIdAsync(CustomUser user)
        {
            return base.GetUserIdAsync(user);
        }
        public override Task<CustomUser> FindByNameAsync(string userName)
        {
            return base.FindByNameAsync(userName);
        }
        public override Task<CustomUser> FindByIdAsync(string userId)
        {
            return base.FindByIdAsync(userId);
        }
        public override Task<bool> IsInRoleAsync(CustomUser user,string role)
        {
            return base.IsInRoleAsync(user,role);
        }
        public override Task<IdentityResult> AddToRoleAsync(CustomUser user, string role)
        {
            return base.AddToRoleAsync(user,role);
        }
        public override Task<IdentityResult> RemoveFromRoleAsync(CustomUser user, string role){
            return base.RemoveFromRoleAsync(user,role);
        }
        
        public override Task<IList<Claim>> GetClaimsAsync(CustomUser user)
        {
           return base.GetClaimsAsync(user); 
        }
        public override Task<IList<string>> GetRolesAsync(CustomUser user)
        {
            return base.GetRolesAsync(user);
        }
        public override Task<bool> CheckPasswordAsync(CustomUser user, string password)
        {
            return base.CheckPasswordAsync(user,password);
        }
        public override Task<IdentityResult> DeleteAsync(CustomUser user)
        {
            return base.DeleteAsync(user);
        }
        public override Task<IdentityResult> UpdateAsync(CustomUser user){
            return base.UpdateAsync(user);
        }
    }
}
