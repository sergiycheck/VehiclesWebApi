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
using Vehicles.Contracts.Requests;
using System.Collections.Generic;
using vehicles.Contracts.V1.Requests;
using Microsoft.AspNetCore.Identity;
using System;

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
        
        [Authorize]
        [HttpPut(ApiRoutes.Identity.Update), DisableRequestSizeLimit]
        public async Task<IActionResult>
            Put(string id, [FromForm]OwnerRequest ownerRequest)
        {
            if (id != ownerRequest.Id)
                return BadRequest();

            if(ownerRequest.Token == null)
            {
                _logger.LogInformation($"bad request token is empty");

                return BadRequest("token is empty");
            }
            else
            {
                _logger.LogInformation($"getting user with token");

                var user = await _identityService.GetUserFromToken(ownerRequest.Token);
                if(user.Id == ownerRequest.Id)
                {
                    var customUser = _customMapper
                        .OwnerRequestToCarOwner(ownerRequest);
                    var result = await _identityService.UpdateUser(customUser);
                    if (result.Succeeded)
                    {
                        return Ok($"{customUser.Email} was successfylly  updated ");
                    }
                    else
                    {
                        var Errors = GetErrors(result);

                        return BadRequest(new AuthFailedResponse
                        {
                            Errors = Errors
                        });
                    }
                }
                else
                {
                    _logger.LogInformation($"bad request token is not yours");

                    return BadRequest("bad token");
                }

            }
        }

        private List<string> GetErrors(IdentityResult result)
        {
            var Errors = new List<string>();
            result.Errors.ToList().ForEach(e =>
            {
                Errors.Add($"{e.Description}");
            });
            return Errors;
        }

        [Authorize]
        [HttpPost(ApiRoutes.Identity.Delete)]
        public async Task<IActionResult>
            Delete(string id, [FromBody] TokenRequest tokenRequest)
        {
            if (id == null)
            {
                return BadRequest("Id is empty");
            }
            var user = await _identityService.FindUser(id);
            if(user == null)
            {
                return BadRequest("no user with such id");
            }
            var userFromToken = await _identityService
                .GetUserFromToken(tokenRequest.Token);

            if (user.Id != userFromToken.Id)
            {
                return BadRequest("The token is not yours");
            }
            try
            {
                //remove all user tokens before deleting
                var res = await _identityService.RevokeToken(tokenRequest.Token);
                if (res > 0)
                {
                    _logger.LogInformation("Token revoked successfully");
                    var deleteResult =
                        await _identityService.DeleteUser(user);
                    if (deleteResult.Succeeded)
                    {
                        return Ok($"{user.Email} was successfylly deleted ");
                    }
                    else
                    {
                        var Errors = GetErrors(deleteResult);

                        return BadRequest(new AuthFailedResponse
                        {
                            Errors = Errors
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;
                _logger.LogInformation(msg);
                return BadRequest(msg);
            }
            return NoContent();

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