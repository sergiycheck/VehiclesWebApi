using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vehicles;
using Vehicles.Models;
using Xunit;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Newtonsoft.Json;

/// <summary>
/// modified program startup context seedData
/// another way is to start the app and start tests to test all methods
/// </summary>
namespace VehiclesXUnitTests
{
    public class BasicHttpTests: IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        public BasicHttpTests(WebApplicationFactory<Startup> factory) 
        {
            _factory = factory;
        }
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        [Fact]
        public async Task GetApiVehiclesDefaultEndpoint()
        {
            var url = "api/vehicles";
            //Arrange
            var client = _factory.CreateClient();
            //Act
            var result = await client.GetAsync(url);

            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);

        }
        
        [Fact]
        public async Task GetApiVehiclesAll()
        {
            var url = "api/vehicles/all";
            //Arrange
            var client = _factory.CreateClient();
            //Act
            var result = await client.GetAsync(url);
            var carsJson = await client.GetStringAsync(url);
            var cars = JsonConvert.DeserializeObject<List<Car>>(carsJson);

            //Assert
            Assert.IsAssignableFrom<List<Car>>(cars);
            //Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        }
        [Fact]
        public async Task GetApiCarOwnersAll()
        {
            var url = "api/CarOwners/all";
            //Arrange
            var client = _factory.CreateClient();
            //Act
            var result = await client.GetAsync(url);
            var Json = await client.GetStringAsync(url);
            var owners = JsonConvert.DeserializeObject<List<CarOwner>>(Json);

            //Assert
            Assert.IsAssignableFrom<List<CarOwner>>(owners);
            //Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        }
        [Fact]
        public async Task PostGetCarsByOwner()
        {
            var url = "api/CarOwners/1";
            //Arrange
            var client = _factory.CreateClient();
            //Act
            var result = await client.GetAsync(url);
            var Json = await client.GetStringAsync(url);

            url = "api/vehicles/get_cars_by_owner";
            var strContent = new StringContent(Json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, strContent);
            
            var carsJson = await response.Content.ReadAsStringAsync();
            var cars = JsonConvert.DeserializeObject<List<Car>>(carsJson);
            //Assert
            Assert.IsAssignableFrom<List<Car>>(cars);
            //Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task GetOwnersByCarUniqueNumber() 
        {
            var url = "api/vehicles/2";
            //Arrange
            var client = _factory.CreateClient();
            //Act
            var JsonCar = await client.GetStringAsync(url);
            var car = JsonConvert.DeserializeObject<Car>(JsonCar);
            url = $"api/CarOwners/{car.UniqueNumber}";

            var response = await client.GetAsync(url);
            var ownersJson = await response.Content.ReadAsStringAsync();

            var owners = JsonConvert.DeserializeObject<List<CarOwner>>(ownersJson);
            //Assert
            Assert.IsAssignableFrom<List<CarOwner>>(owners);
            //Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
