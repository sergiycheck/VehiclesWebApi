using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Vehicles.Data;
using Vehicles.Models;
using Vehicles.Options;
using Vehicles.Interfaces;
using Vehicles.AuthorizationsManagers;
using Vehicles.Contracts.V1.Requests;


namespace Vehicles.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly ICustomUserManager _userManager;

        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICustomRoleManager _roleManager;


        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly VehicleDbContext _context;
        //private readonly IFacebookAuthService _facebookAuthService;
        
        public IdentityService(
            ICustomUserManager userManager, 
            JwtSettings jwtSettings, 
            TokenValidationParameters tokenValidationParameters,
            VehicleDbContext context,
            //RoleManager<IdentityRole> roleManager
            ICustomRoleManager roleManager
            //IFacebookAuthService facebookAuthService
            )
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _context = context;
            _roleManager = roleManager;
            //_facebookAuthService = facebookAuthService;
        }

        
        
        public async Task<AuthenticationResult> RegisterAsync(UserRegistrationRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] {"User with this email address already exists"}
                };
            }

            var newUserId = Guid.NewGuid();
            var newUser = new CustomUser
            {
                Id = newUserId.ToString(),
                Email = request.Email,
                UserName = request.UserName,
                EmailConfirmed = true
            };

            var createdUser = await _userManager.CreateAsync(newUser, request.Password);

            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }
            
            return await GenerateAuthenticationResultForUserAsync(newUser);
        }
        
        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] {"User does not exist"}
                };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!userHasValidPassword)
            {
                return new AuthenticationResult
                {
                    Errors = new[] {"User/password combination is wrong"}
                };
            }
            
            return await GenerateAuthenticationResultForUserAsync(user);
        }
        
        
        private async Task<(List<Claim>,CustomUser)> AddRolesAndRoleClaimsAsync(List<Claim> claims,CustomUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));

                var role = await _roleManager.FindByNameAsync(userRole);
                if(role == null) continue;

                var roleClaims = await _roleManager.GetClaimsAsync(role);
                foreach (var roleClaim in roleClaims)
                {
                    if(claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }
            return (claims,user);
        }


        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(CustomUser user)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var jwtSecretBytesKey = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id)
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            (claims,user) = await AddRolesAndRoleClaimsAsync(claims,user);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),//differece here! now 30 seconds
                SigningCredentials =
                    new SigningCredentials(
                        new SymmetricSecurityKey(jwtSecretBytesKey), 
                        SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenFromTokenDescriptor = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);

            var refreshTokenForUser = new RefreshToken
            {
                JwtId = tokenFromTokenDescriptor.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)//6 months to expire
            };

            await _context.RefreshTokens.AddAsync(refreshTokenForUser);
            await _context.SaveChangesAsync();
            
            return new AuthenticationResult
            {
                Success = true,
                Token = jwtSecurityTokenHandler.WriteToken(tokenFromTokenDescriptor),
                RefreshToken = refreshTokenForUser.Token
            };
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                return new AuthenticationResult {Errors = new[] {"Invalid Token"}};
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult {Errors = new[] {"This token hasn't expired yet"}};
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult {Errors = new[] {"This refresh token does not exist"}};
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult {Errors = new[] {"This refresh token has expired"}};
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult {Errors = new[] {"This refresh token has been invalidated"}};
            }

            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult {Errors = new[] {"This refresh token has been used"}};
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult {Errors = new[] {"This refresh token does not match this JWT"}};
            }

            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<CustomUser> GetUserFromToken(string token)
        {
            var validatedToken = GetPrincipalFromToken(token);
            // var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            var id = validatedToken.Claims.Single(x => x.Type == "id").Value;
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u=>u.Id==id);
            return user;
        }

        

        public async Task<CustomIdentityResult> UpdateUser(CustomUser user)
        {
            // var updatedIdentityResult = await  _userManager.UpdateAsync(user);
            var userWithSameEmailExists = await _context.Users
                .AsNoTracking()
                .Where(u=>u.Id!=user.Id && u.Email==user.Email).FirstOrDefaultAsync();

            if(userWithSameEmailExists!=null){
                return new CustomIdentityResult(){
                    Succeeded = false,
                    Errors = new string [] {"User with the same email already exists"}
                };
            }

            var userWithSameUserNameExists = await _context.Users
                .AsNoTracking()
                .Where(u=>u.Id!=user.Id && u.UserName==user.UserName).FirstOrDefaultAsync();
            if(userWithSameEmailExists!=null){
                return new CustomIdentityResult(){
                    Succeeded = false,
                    Errors = new string [] {"User with the same UserName already exists"}
                };
            }
            _context.Users.Update(user);
            try
            {

                var updatedResult = await _context.SaveChangesAsync(); 
                if(updatedResult>0){
                    return new CustomIdentityResult(){
                        Succeeded = true
                    };
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return new CustomIdentityResult(){
                        Succeeded = false,
                        Errors = new string [] {"Updating user finished with errors"}
                    };
            
        }
        public async Task<CustomIdentityResult> DeleteUser(CustomUser user)
        {
            var deleteResult = await _userManager.DeleteAsync(user);
            if(deleteResult.Succeeded){
                return new CustomIdentityResult(){
                        Succeeded = true
                    };;
            }
            return new CustomIdentityResult(){
                        Succeeded = false,
                        Errors = new string [] {"Deleting user finished with errors"}
                    };;
        }
        public async Task<CustomUser> FindUser(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        // public async Task<AuthenticationResult> LoginWithFacebookAsync(string accessToken)
        // {
        //     var validatedTokenResult = await _facebookAuthService.ValidateAccessTokenAsync(accessToken);

        //     if (!validatedTokenResult.Data.IsValid)
        //     {
        //         return new AuthenticationResult
        //         {
        //             Errors = new[] {"Invalid Facebook token"}
        //         };
        //     }

        //     var userInfo = await _facebookAuthService.GetUserInfoAsync(accessToken);

        //     var user = await _userManager.FindByEmailAsync(userInfo.Email);

        //     if (user == null)
        //     {
        //         var identityUser = new CustomUser
        //         {
        //             Id = Guid.NewGuid().ToString(),
        //             Email = userInfo.Email,
        //             UserName = userInfo.Email
        //         };

        //         var createdResult = await _userManager.CreateAsync(identityUser);
        //         if (!createdResult.Succeeded)
        //         {
        //             return new AuthenticationResult
        //             {
        //                 Errors = new[] {"Something went wrong"}
        //             };
        //         }
                
        //         return await GenerateAuthenticationResultForUserAsync(identityUser);
        //     }

        //     return await GenerateAuthenticationResultForUserAsync(user);
        // }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }


        public async Task<int> RevokeToken(string token)
        {
            var user = await GetUserFromToken(token);
            if(user==null) return 0;
            var refreshTokens = await _context.RefreshTokens.Where(r=>r.UserId==user.Id).ToListAsync();
            
            if(refreshTokens!=null && refreshTokens.Count()>0)
            {
                var all = 0;
                foreach(var refreshToken in refreshTokens){
                    _context.RefreshTokens.Remove(refreshToken);
                    var res = await _context.SaveChangesAsync();
                    all++;
                }
                    
                return all;
            }
            return 0;

        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }


    }
}