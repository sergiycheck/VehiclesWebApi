using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using Vehicles.Contracts.V1.Requests;
using Vehicles.Models;


namespace Vehicles.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(UserRegistrationRequest request);
        
        Task<AuthenticationResult> LoginAsync(string email, string password);
        
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
        Task<CustomUser> GetUserFromToken(string token);
        Task<int> RevokeToken(string token);

        ClaimsPrincipal GetPrincipalFromToken(string token);
        //Task<AuthenticationResult> LoginWithFacebookAsync(string accessToken);
        Task<IdentityResult> UpdateUser(CustomUser user);
        Task<IdentityResult> DeleteUser(CustomUser user);
        Task<CustomUser> FindUser(string id);
    }
}