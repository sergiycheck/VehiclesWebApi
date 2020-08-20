using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vehicles;
using Vehicles.Models;
using Xunit;
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
        public async Task GetApiVehiclesAllEndpoint()
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
    }
}
