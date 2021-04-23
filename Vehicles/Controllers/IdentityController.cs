using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vehicles.Contracts.V1;
using Vehicles.Contracts.V1.Requests;
using Vehicles.Contracts.V1.Responses;
using Vehicles.Services;
using Microsoft.AspNetCore.Authorization;
using Vehicles.Models;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Vehicles.MyCustomMapper;
using Vehicles.Contracts.Responces;

namespace Vehicles.Controllers
{
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;
        protected readonly ILogger<IdentityController> _logger;
        private readonly ICustomMapper _customMapper;
        public IdentityController(
            IIdentityService identityService,
            ILogger<IdentityController> logger,
            ICustomMapper customMapper)
        {
            _identityService = identityService;
            _logger = logger;
            _customMapper = customMapper;
        }

        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if(request.Email == null ||
                request.Password == null||
                request.UserName == null)
            {
                return BadRequest("data cann't be empty for registration");
            }

            _logger.LogInformation($"New registration request {request.Email} {request.Password}");
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            
            var authResponse = await _identityService.RegisterAsync(request);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }
            
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
        
        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            _logger.LogInformation($"New login request {request.Email} {request.Password}");
            var authResponse = await _identityService.LoginAsync(request.Email, request.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }
            
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
        
        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            if(request!=null && request.Token!=null && request.RefreshToken!=null){
                _logger.LogInformation($"refreshing token {request.Token} {request.RefreshToken}");
            }
            
            var authResponse = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);
            

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }
            
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }


        public class TokenRequest
        {
            public string Token{get;set;}
        }

        [Authorize]//send token in auth header
        [HttpPost(ApiRoutes.Identity.GetUser)]
        public async Task<IActionResult> GetUser([FromBody] TokenRequest tokenRequest)
        {
            
            //if(!User.Identity.IsAuthenticated)
            //{
            //    return Challenge();
            //}
            //var name=User.Identity.Name;//Name is null

            if (tokenRequest==null || tokenRequest.Token==null)
            {
                _logger.LogInformation($"bad request token is empty");

                return BadRequest("token is empty");
            }else
            {
                _logger.LogInformation($"getting user with token");

                var user = await _identityService.GetUserFromToken(tokenRequest.Token);
                var ownerResponse = _customMapper.OwnerToOwnerResponse(user);

                if(user!=null)
                    return Ok(new Response<OwnerResponce>(ownerResponse));
            }
            return(Ok("Empty")); 
                
            
        }

        [Authorize]
        [HttpGet(ApiRoutes.Identity.RevokeToken)]
        public async Task<IActionResult> RevokeToken([FromBody] TokenRequest tokenRequest)
        {
            if(!User.Identity.IsAuthenticated)
            {
                return Challenge();
            }
            if(tokenRequest!=null)
            {
                if(tokenRequest.Token!=null)
                {
                    var res = await _identityService.RevokeToken(tokenRequest.Token);
                    if(res>0)
                    {
                        return Ok("Token revoked successfully");
                    }
                }
            }
            return BadRequest("Empty token request");
           
        }

        
        // [HttpPost(ApiRoutes.Identity.FacebookAuth)]
        // public async Task<IActionResult> FacebookAuth([FromBody] UserFacebookAuthRequest request)
        // {
        //     var authResponse = await _identityService.LoginWithFacebookAsync(request.AccessToken);

        //     if (!authResponse.Success)
        //     {
        //         return BadRequest(new AuthFailedResponse
        //         {
        //             Errors = authResponse.Errors
        //         });
        //     }
            
        //     return Ok(new AuthSuccessResponse
        //     {
        //         Token = authResponse.Token,
        //         RefreshToken = authResponse.RefreshToken
        //     });
        // }
    }
}