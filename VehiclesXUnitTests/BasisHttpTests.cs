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
        public async Task GetApiVehiclesAll()
        {
            var url = ApiRoutes.Vehicles.GetAll;
            //Arrange
            var client = _factory.CreateClient();
            //Act
            var result = await client.GetAsync(url);
            var carsJson = await client.GetStringAsync(url);
            var cars = JsonConvert.DeserializeObject<Response<List<CarResponse>>>(carsJson);

            //Assert
            Assert.IsAssignableFrom<List<CarResponse>>(cars.Data);
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
        [Fact]
        public async Task PostGetCarsByOwner()
        {
            var url = ApiRoutes.Owners.Get.Replace("{id:int}","3");
            //Arrange
            var client = _factory.CreateClient();
            //Act
            var result = await client.GetAsync(url);
            var Json = await client.GetStringAsync(url);

            url = ApiRoutes.Vehicles.GetCarsByOwner;
            var strContent = new StringContent(Json, 
                        Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, strContent);
            
            var carsJson = await response.Content.ReadAsStringAsync();
            var cars = JsonConvert.DeserializeObject<Response<List<CarResponse>>>(carsJson);
            //Assert
            Assert.IsAssignableFrom<List<CarResponse>>(cars.Data);
            //Assert
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
            var car = JsonConvert.DeserializeObject<Response<CarResponse>>(JsonCar);
            url = ApiRoutes.Owners
                    .GetOwnersByCarUniqueNumber
                    .Replace("{uniqueNumber}",car.Data.UniqueNumber);

            var response = await client.GetAsync(url);
            var ownersJson = await response.Content.ReadAsStringAsync();

            var owners = JsonConvert.DeserializeObject<Response<List<OwnerResponce>>>(ownersJson);
            //Assert
            Assert.IsAssignableFrom<List<OwnerResponce>>(owners.Data);
            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
