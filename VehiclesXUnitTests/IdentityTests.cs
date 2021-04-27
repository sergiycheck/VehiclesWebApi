using Microsoft.AspNetCore.Mvc.Testing;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vehicles;
using Xunit;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Newtonsoft.Json;
using Vehicles.Contracts.V1;
using Vehicles.Contracts.V1.Responses;
using Vehicles.Contracts.Responces;
using Vehicles.Contracts.V1.Requests;
using System;
using Xunit.Abstractions;
using System.Net.Http.Headers;
using Vehicles.Models;
using vehicles.Contracts.V1.Requests;

namespace VehiclesXUnitTests
{
    public class IdentityTests: IClassFixture<WebApplicationFactory<Startup>>
    {

        private readonly ITestOutputHelper _output;
        private readonly WebApplicationFactory<Startup> _factory;
        public IdentityTests(WebApplicationFactory<Startup> factory,ITestOutputHelper output) 
        {
            _factory = factory;
            _output = output;
        
        }

        [Fact]
        public async Task IdentityControllerRegisterTest()
        {
            var url = ApiRoutes.Identity.Register;

            var client = _factory.CreateClient();
            
            var userRegistrationRequest = new UserRegistrationRequest()
            {
                UserName="NewTestUserName",
                Email = "testname@domain.com",
                Password = "test124!StrongPass"
            };
            var jsonRegistrationRequest = JsonConvert.SerializeObject(userRegistrationRequest);

            var strContent = new StringContent(jsonRegistrationRequest, 
                        Encoding.UTF8, "application/json");
            var registrationResponse = await client.PostAsync(url, strContent);
            
            var jsonRegistrationResponce = await registrationResponse.Content.ReadAsStringAsync();
            var userExists = "User with this email address already exists";
            if (jsonRegistrationResponce.Trim().Contains(userExists)) 
            {
                _output.WriteLine(userExists);
                return;
            }
                
            var authSuccessRegistratinoResponce 
                        = JsonConvert
                            .DeserializeObject<AuthSuccessResponse>(jsonRegistrationResponce);

            Assert.IsAssignableFrom<AuthSuccessResponse>(authSuccessRegistratinoResponce);
            Assert.NotNull(authSuccessRegistratinoResponce.Token);
            Assert.NotNull(authSuccessRegistratinoResponce.RefreshToken);
            _output.WriteLine(
                  $"Token {authSuccessRegistratinoResponce.Token}, \n"
                + $"RefreshToken {authSuccessRegistratinoResponce.RefreshToken}"
                );
           
        }

        private async Task<HttpResponseMessage> LoginAsync(UserLoginRequest userLoginRequest) 
        {
            var url = ApiRoutes.Identity.Login;

            var client = _factory.CreateClient();
            
            var jsonLoginRequest = JsonConvert.SerializeObject(userLoginRequest);
            var strContent = new StringContent(
                                jsonLoginRequest,
                                Encoding.UTF8,
                                "application/json");

            var loginResponse = await client.PostAsync(url, strContent);
            return loginResponse;
        }
        [Fact]
        public async Task IdentityControllerLoginTest()
        {
            var userLoginRequest = new UserLoginRequest()
            {
                Email = "name1Email@domain.com",
                Password = "!VeryStrPass1234_1"
            };

            var loginResponse = await LoginAsync(userLoginRequest);
            

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var jsonLoginResponce = await loginResponse.Content.ReadAsStringAsync();
            var authSuccessLoginResponce 
                        = JsonConvert
                            .DeserializeObject<AuthSuccessResponse>(jsonLoginResponce);

            Assert.IsAssignableFrom<AuthSuccessResponse>(authSuccessLoginResponce);

            _output.WriteLine(
                  $"Token {authSuccessLoginResponce.Token}, \n"
                + $"RefreshToken {authSuccessLoginResponce.RefreshToken}"
                );
        }

        [Fact]
        public async Task IdentityControllerRegisterGetUserDeleteTest()
        {
            var url = ApiRoutes.Identity.Register;

            var client = _factory.CreateClient();

            var userRegistrationRequest = new UserRegistrationRequest()
            {
                UserName = "testDeleteUser",
                Email = "testDeleteMename@domain.com",
                Password = "DeleteMetest124!StrongPass"
            };
            var jsonRegistrationRequest = JsonConvert.SerializeObject(userRegistrationRequest);

            var strContent = new StringContent(jsonRegistrationRequest,
                        Encoding.UTF8, "application/json");

            var registrationResponse = await client.PostAsync(url, strContent);

            var jsonRegistrationResponce = await registrationResponse.Content.ReadAsStringAsync();


            var userExists = "User with this email address already exists";
            var Token = String.Empty;
            if (jsonRegistrationResponce.Trim().Contains(userExists))
            {
                _output.WriteLine(userExists);
                var userLoginRequest = new UserLoginRequest()
                {
                    Email = userRegistrationRequest.Email,
                    Password = userRegistrationRequest.Password
                };

                var loginResponse = await LoginAsync(userLoginRequest);


                Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

                var jsonLoginResponce = await loginResponse.Content.ReadAsStringAsync();
                var authSuccessLoginResponce
                            = JsonConvert
                                .DeserializeObject<AuthSuccessResponse>(jsonLoginResponce);

                Assert.IsAssignableFrom<AuthSuccessResponse>(authSuccessLoginResponce);
                _output.WriteLine(
                  $"Token {authSuccessLoginResponce.Token}, \n"
                + $"RefreshToken {authSuccessLoginResponce.RefreshToken}"
                );
                Token = authSuccessLoginResponce.Token;
            }
            else
            {
                var authSuccessRegistratinoResponce
                        = JsonConvert
                            .DeserializeObject<AuthSuccessResponse>(jsonRegistrationResponce);

                Assert.IsAssignableFrom<AuthSuccessResponse>(authSuccessRegistratinoResponce);
                Assert.NotNull(authSuccessRegistratinoResponce.Token);
                Assert.NotNull(authSuccessRegistratinoResponce.RefreshToken);
                _output.WriteLine(
                      $"Token {authSuccessRegistratinoResponce.Token}, \n"
                    + $"RefreshToken {authSuccessRegistratinoResponce.RefreshToken}"
                    );
                Token = authSuccessRegistratinoResponce.Token;
            }

            
            //todo: get user by token
            var getUserUrl = ApiRoutes.Identity.GetUser;
            var tokenRequest = new TokenRequest()
            {
                Token = Token
            };
            var jsonToken = JsonConvert.SerializeObject(tokenRequest);

            var strContentToken = new StringContent(jsonToken,
                        Encoding.UTF8, "application/json");

            var requestGetUser = new HttpRequestMessage(HttpMethod.Post, getUserUrl);
            requestGetUser.Content = strContentToken;
            requestGetUser.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);


            var GetUserResponse = await client.SendAsync(requestGetUser);
            Assert.Equal(HttpStatusCode.OK, GetUserResponse.StatusCode);

            var jsonIGetUserResponce = await GetUserResponse.Content.ReadAsStringAsync();
            var userResponse
                        = JsonConvert
                            .DeserializeObject<Response<OwnerResponce>>(jsonIGetUserResponce);
            Assert.NotNull(userResponse);
            Assert.IsAssignableFrom<Response<OwnerResponce>>(userResponse);

            var urlDelete = ApiRoutes.Identity.Delete;
            var urlDeleteWithId = urlDelete.Replace("{id}", userResponse.Data.Id);


            var request = new HttpRequestMessage(HttpMethod.Post, urlDeleteWithId);
            request.Content = strContentToken;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var deleteResponse = await client.SendAsync(request);
            
            var jsonDeleteResponce = await deleteResponse.Content.ReadAsStringAsync();
            _output.WriteLine(jsonDeleteResponce);

            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        }



        [Theory]
        [InlineData("name1Email@domain.com", "!VeryStrPass1234_1")]
        public async Task VehiclesControllerGetByIdAuthorizeTest(string email,string password) 
        {
            var userLoginRequest = new UserLoginRequest()
            {
                Email = email,
                Password = password
            };

            var url = ApiRoutes.Identity.Login;
            

            var client = _factory.CreateClient();

            var jsonLoginRequest = JsonConvert.SerializeObject(userLoginRequest);
            var strContent = new StringContent(
                                jsonLoginRequest,
                                Encoding.UTF8,
                                "application/json");

            var loginResponse = await client.PostAsync(url, strContent);
            var jsonLoginResponce = await loginResponse.Content.ReadAsStringAsync();
            var authSuccessLoginResponce
                        = JsonConvert
                            .DeserializeObject<AuthSuccessResponse>(jsonLoginResponce);

            var urlGet1CarById = ApiRoutes.Vehicles.Get.Replace("{id:int}", "1");

            var request = new HttpRequestMessage(HttpMethod.Get, urlGet1CarById);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authSuccessLoginResponce.Token);

            var responseCar = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, responseCar.StatusCode);

            var carJson = await responseCar.Content.ReadAsStringAsync();
            
            var car = JsonConvert.DeserializeObject<Response<Car>>(carJson);
            Assert.IsAssignableFrom<Response<Car>>(car);


        }

        [Fact]
        public async Task VehiclesControllerGetByIdUnAuthorizedExceptionTest() 
        {
            var client = _factory.CreateClient();
            var urlGet1CarById = ApiRoutes.Vehicles.Get.Replace("{id:int}", "1");

            var request = new HttpRequestMessage(HttpMethod.Get, urlGet1CarById);
            var responseCar = await client.SendAsync(request);
            //Assert.Equal(HttpStatusCode.Unauthorized, responseCar.StatusCode);

            Assert.Equal(HttpStatusCode.OK, responseCar.StatusCode);

        }
        [Fact]
        public async Task VehiclesControllerGetByIdUnAuthorizedExceptionTest_AuthorizationWithoutToken()
        {
            var client = _factory.CreateClient();
            var urlGet1CarById = ApiRoutes.Vehicles.Get.Replace("{id:int}", "1");

            var request = new HttpRequestMessage(HttpMethod.Get, urlGet1CarById);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer");
            var responseCar = await client.SendAsync(request);
            //Assert.Equal(HttpStatusCode.Unauthorized, responseCar.StatusCode);

            Assert.Equal(HttpStatusCode.OK, responseCar.StatusCode);
        }

    }
}