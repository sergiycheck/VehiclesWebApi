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
using Xunit.Abstractions;
using System.Linq;
using Vehicles.Contracts.V1.Requests;
using System.Net.Http.Headers;
using vehicles.Helpers;
using Vehicles.Models;
using Vehicles.Contracts.Requests;
using vehicles.Contracts.V1.Requests;
using System;

namespace VehiclesXUnitTests
{
    public class BasicHttpTests: IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _output;
        public BasicHttpTests(WebApplicationFactory<Startup> factory, 
                                ITestOutputHelper output) 
        {
            _factory = factory;
            _output = output;
        }
        private static readonly JsonSerializerOptions _jsonSerializerOptions = 
                                                        new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        [Fact]
        public async Task GetApiVehiclesDefaultEndpoint()
        {
            var url = ApiRoutes.Vehicles.Default;
            //Arrange
            var client = _factory.CreateClient();
            //Act
            var result = await client.GetAsync(url);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        }
        [Fact]
        public async Task GetVehicleImgs()
        {
            var url = ApiRoutes.Vehicles.GetImage;
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var dataJson = await response.Content.ReadAsStringAsync();
            _output.WriteLine(dataJson);

            Assert.NotEmpty(dataJson);
            Assert.NotEqual("Directory does not exists or file not found with this name {name}", dataJson.Trim());
        }


        [Theory]
        [InlineData(
            "Kia Mohave OFFICIAL FULL AWD",
            "HN-8080-OV", 
            "Kia_Mohave_OFFICIAL_FULL_AWD_HN-8080-OV")]
        [InlineData(
            "Porsche Panamera 2010", 
            "XX-7636-WX", 
            "Porsche_Panamera_2010_XX-7636-WX")]
        [InlineData(
            "Mercedes-Benz C 200 2010",
            "QY-0324-PK",
            "Mercedes-Benz_C_200_2010_QY-0324-PK")]
        public void ReplaceSpaceWithDash(string Brand, string UniqueNumber,string expected)
        {
            var imgRetriever = new VehicleImageRetriever();
            var nameResult = imgRetriever.ReplaceSpaceWithDash(Brand, UniqueNumber);
            _output.WriteLine($"nameResult {nameResult}");
            Assert.Equal(expected, nameResult);
        }




        private const string FILE_PATH = @"C:\Users\sergi\source\repos\Infotech\Vehicles\wwwroot\assets\vehicleImgs\";
        [Theory]
        [InlineData(
            "Kia Mohave OFFICIAL FULL AWD",
            "HN-8080-OV",
            FILE_PATH)]
        [InlineData(
            "Porsche Panamera 2010",
            "XX-7636-WX",
           FILE_PATH)]
        [InlineData(
            "Mercedes-Benz C 200 2010",
            "QY-0324-PK",
            FILE_PATH)]
        public async Task GetImageByBrandAndUniqueNumber(string Brand, string UniqueNumber, string imgDirectory)
        {
            var imgRetriever = new VehicleImageRetriever();
            var imgFileInfo = await imgRetriever
                .GetImageByBrandAndUniqueNumber(
                    Brand,
                    UniqueNumber,
                    imgDirectory);

            Assert.NotNull(imgFileInfo);
            Assert.IsAssignableFrom<FileImgInfo>(imgFileInfo);
            _output.WriteLine($"{imgFileInfo.FileType} \n" +
                $"{imgFileInfo.FileBytes.Length}");
        }


        //passed without authorization
        [Theory]
        [InlineData(1)]
        public async Task GetCarById(int id)
        {
            var url = ApiRoutes.Vehicles.Get;
            var getByIdUrl = url.Replace("{id:int}",$"{id}");
            //Arrange
            var client = _factory.CreateClient();
            var responseMessage = await client.GetAsync(getByIdUrl);
            var responseMessageJson = await responseMessage.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
            var carResponse = JsonConvert.DeserializeObject<Response<Car>>(responseMessageJson);
            Assert.IsAssignableFrom<Response<Car>>(carResponse);


        }


        [Fact]
        public async Task GetApiVehiclesAll()
        {
            var url = ApiRoutes.Vehicles.GetAll;
            //Arrange
            var client = _factory.CreateClient();
            //Act
            var result = await client.GetAsync(url);
            var carsJson = await client.GetStringAsync(url);
            var cars = JsonConvert.DeserializeObject<Response<List<Car>>>(carsJson);

            //Assert
            Assert.IsAssignableFrom<List<Car>>(cars.Data);
            //Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            cars.Data.ForEach(c => _output.WriteLine($"{c.Id}, {c.Brand}, {c.Date}, {c.Price}"));
        }
        [Fact]
        public async Task GetApiCarOwnersAll()
        {
            //Arrange
            var url = ApiRoutes.Vehicles.GetAll;     
            var client = _factory.CreateClient();
            //Act
            var result = await client.GetAsync(url);
            var Json = await client.GetStringAsync(url);
            var owners = JsonConvert.DeserializeObject<Response<List<OwnerResponce>>>(Json);
            
            //Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            //Assert
            Assert.IsAssignableFrom<List<OwnerResponce>>(owners.Data);


            
        }

        public async Task<AuthSuccessResponse> GetLoginResponse() 
        {
            var userLoginRequest = new UserLoginRequest()
            {
                Email = "name1Email@domain.com",
                Password = "!VeryStrPass1234_1"
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

            return authSuccessLoginResponce;
        }

        [Fact]
        public async Task PostGetCarsByOwner()
        {
            //Arrange
            var client = _factory.CreateClient();

            var userParams = new UsersParameters()
            {
                PageSize = 1
            };

            var getOwnersUrl = ApiRoutes.Owners.GetAll;

            var propertyList = userParams.GetType().GetProperties();
            var queryString = "?";
            foreach(var prop in propertyList)
            {
                queryString += $"{prop.Name}={prop.GetValue(userParams, null)}&";
            }
            queryString = queryString.Remove(queryString.Length - 1, 1);

            _output.WriteLine(queryString);
            var ownersUrlAndQuery = $"{getOwnersUrl}{queryString}";
            _output.WriteLine(ownersUrlAndQuery);

            var json = await client.GetStringAsync(ownersUrlAndQuery);
            Assert.NotNull(json);
            var ownerFromDbResponse = JsonConvert.DeserializeObject<Response<IEnumerable<OwnerResponce>>>(json);
            Assert.IsAssignableFrom<Response<IEnumerable<OwnerResponce>>>(ownerFromDbResponse);
            var owner = ownerFromDbResponse.Data.ToArray()[0];
            Assert.NotNull(owner);

            var jsonOwnerRequest = JsonConvert.SerializeObject(owner);

            var authSuccessLoginResponce = await GetLoginResponse();

            var url = ApiRoutes.Vehicles.GetCarsByOwner;

            var strContent = new StringContent(jsonOwnerRequest, 
                        Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", authSuccessLoginResponce.Token);

            var response = await client.PostAsync(url,strContent);
            
            var carsJson = await response.Content.ReadAsStringAsync();
            var cars = JsonConvert.DeserializeObject<Response<List<Car>>>(carsJson);
            //Assert
            Assert.IsAssignableFrom<List<Car>>(cars.Data);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task GetOwnersByCarUniqueNumber() 
        {
            var url = ApiRoutes.Vehicles.Get.Replace("{id:int}","2");
            //Arrange
            var client = _factory.CreateClient();
            //Act
            var JsonCar = await client.GetStringAsync(url);
            var car = JsonConvert.DeserializeObject<Response<Car>>(JsonCar);// we user Car instead of carResponse because we cannot deserialize file
            url = ApiRoutes.Owners
                    .GetOwnersByCarUniqueNumber
                    .Replace("{uniqueNumber}",car.Data.UniqueNumber);

            var response = await client.GetAsync(url);
            var ownersJson = await response.Content.ReadAsStringAsync();

            var owners = JsonConvert.DeserializeObject<Response<List<OwnerResponce>>>(ownersJson);
            //Assert
            Assert.IsAssignableFrom<List<OwnerResponce>>(owners.Data);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.True(owners.Data.Count > 0);
        }
    }
}
