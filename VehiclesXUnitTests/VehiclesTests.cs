using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vehicles.Data;
using Xunit;
using Vehicles.Helpers;
using Vehicles.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Vehicles.Models;
using Newtonsoft.Json;
using Vehicles.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using Vehicles.Services;
using Microsoft.AspNetCore.Mvc;
using Vehicles.MyCustomMapper;
using Vehicles.Contracts.Requests;
using Vehicles.Contracts.Responces;
using Vehicles.Contracts.V1.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using Vehicles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vehicles.Interfaces;
using Microsoft.AspNetCore.Hosting;
using vehicles.Helpers;
using Vehicles.AuthorizationsManagers;
using vehicles.Authorization.AuthorizationsManagers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace VehiclesXUnitTests
{
    //https://docs.microsoft.com/ru-ru/aspnet/core/test/integration-tests?view=aspnetcore-5.0

    public class VehiclesTests : IClassFixture<SqlServerTests>,IClassFixture<WebApplicationFactory<Startup>>
    {
        public SqlServerTests Fixture { get; }
        private IServiceProvider _serviceProvider {get;}
        public VehiclesTests(SqlServerTests fixture,WebApplicationFactory<Startup> factory)
        {
            Fixture = fixture;
            _serviceProvider = factory.Services;

        }
        [Fact]
        public void TestPair()
        {
            var excludePair = new HashSet<CustomPair>();
            var car_id = 1;
            string car_owner_id = "4985dd49-f06c-4bd8-8900-baeff6126b55";
            var pair = new CustomPair(car_id, car_owner_id);
            excludePair.Add(pair);
            var pair2 = new CustomPair(car_id, car_owner_id);
            excludePair.Add(pair2);
            var pair3 = new CustomPair(car_id+1, car_owner_id+1);

            Assert.Contains(pair2, excludePair);
            Assert.DoesNotContain(pair3, excludePair);
        }
        [Fact]
        public void TryParse() 
        {
            var data = DateTime.Parse("08.06.2018");
            var data2 = DateTime.Parse("1988-02-06");
            var data3 = DateTime.Parse("1978-07-26");

            Assert.IsAssignableFrom<DateTime>(data);
            Assert.IsAssignableFrom<DateTime>(data2);
            Assert.IsAssignableFrom<DateTime>(data3);
        }
        [Fact]
        public async Task TestSeedData()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    
                    var seed = new SeedData(new VehicleImageRetriever());
                    using var serviceScope = _serviceProvider.CreateScope();
                    var userManager = serviceScope.ServiceProvider.GetRequiredService<ICustomUserManager>();
                    var userRoleManager = serviceScope.ServiceProvider.GetRequiredService<ICustomRoleManager>();
                    await seed.Initialize(userManager, userRoleManager, context);

                    Assert.NotNull(context.Users);
                    Assert.NotNull(context.Cars);
                    Assert.NotNull(context.ManyToManyCarOwners);
                }
            }

        }
        [Fact]
        public async Task TestGetCarOwnersByUniqueNumber()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    var seed = new SeedData(new VehicleImageRetriever());
                    using var serviceScope = _serviceProvider.CreateScope();
                    var userManager = serviceScope.ServiceProvider.GetRequiredService<ICustomUserManager>();
                    var userRoleManager = serviceScope.ServiceProvider.GetRequiredService<ICustomRoleManager>();
                    await seed.Initialize(userManager, userRoleManager, context);

                    var carOwnerRepo = new CarOwnersRepository(context);
                    Random rnd = new Random();
                    var index = rnd.Next(0, context.Cars.AsNoTracking().ToHashSet().Count);
                    var car =  context.Cars.AsNoTracking().ToList()[index];
                    var res = await carOwnerRepo.GetCarOwners(car.UniqueNumber);

                    Assert.NotNull(res);
                    Assert.IsAssignableFrom<List<CustomUser>>(res);

                    var resJson = JsonConvert.SerializeObject(res);
                    Assert.NotNull(resJson);
                    Assert.IsAssignableFrom<string>(resJson);
                }
            }

        }
        [Fact]
        public async Task TestGetCarsbyOwner()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    var seed = new SeedData(new VehicleImageRetriever());
                    using var serviceScope = _serviceProvider.CreateScope();
                    var userManager = serviceScope.ServiceProvider.GetRequiredService<ICustomUserManager>();
                    var userRoleManager = serviceScope.ServiceProvider.GetRequiredService<ICustomRoleManager>();
                    await seed.Initialize(userManager, userRoleManager, context);

                    var carsRepo = new CarsRepository(context);
                    Random rnd = new Random();
                    var index = rnd.Next(0, context.Users.AsNoTracking().ToHashSet().Count);
                    var owner =  context.Users.AsNoTracking().ToList()[index];
                    var res = await carsRepo.GetCars(owner);

                    Assert.NotNull(res);
                    Assert.IsAssignableFrom<List<Car>>(res);
                    var resJson = JsonConvert.SerializeObject(res);
                    Assert.NotNull(resJson);
                    Assert.IsAssignableFrom<string>(resJson);
                }
            }

        }        
        [Fact]
        public async Task TestVehicleController_GetOwnersByCarUniqueNumber()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    var seed = new SeedData(new VehicleImageRetriever());
                    using var serviceScope = _serviceProvider.CreateScope();
                    var userManager = serviceScope.ServiceProvider.GetRequiredService<ICustomUserManager>();
                    var userRoleManager = serviceScope.ServiceProvider.GetRequiredService<ICustomRoleManager>();
                    await seed.Initialize(userManager, userRoleManager, context);


                    var carsRepo = new CarsRepository(context);
                    var carOwnerRepo = new CarOwnersRepository(context);
                    var identitySevice = new Mock<IIdentityService>();

                    var carService = new CarService(carsRepo,carOwnerRepo,identitySevice.Object);

                    
                    var carOwnerService = new CarOwnerService(carOwnerRepo);

                    var mockLogger = new Mock<ILogger<VehiclesController>>();
                    ILogger<VehiclesController> logger = mockLogger.Object;

                    var controller = new CarOwnersController(
                        carOwnerService, 
                        logger,
                        new UriService("https://localhost:5010"),
                        new CustomMapper(
                            new VehicleImageRetriever())
                        );

                    Random rnd = new Random();
                    var index = rnd.Next(0, context.Cars.AsNoTracking().ToHashSet().Count);
                    var car = context.Cars.AsNoTracking().ToList()[index];

                    var res = await controller.GetOwnersByCarUniqueNumber(car.UniqueNumber);

                    Assert.NotNull(res);
                    Assert.IsAssignableFrom<OkObjectResult>(res);

                }
            }

        }
        [Fact]
        public async Task TestVehicleController_GetCarsByCarOwner()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    var seed = new SeedData(new VehicleImageRetriever());
                    using var serviceScope = _serviceProvider.CreateScope();
                    var userManager = serviceScope.ServiceProvider.GetRequiredService<ICustomUserManager>();
                    var userRoleManager = serviceScope.ServiceProvider.GetRequiredService<ICustomRoleManager>();
                    await seed.Initialize(userManager, userRoleManager, context);

                    var carsRepo = new CarsRepository(context);
                    var carOwnerRepo = new CarOwnersRepository(context);
                    var identitySevice = new Mock<IIdentityService>();

                    var carService = new CarService(carsRepo,carOwnerRepo,identitySevice.Object);
                    var carOwnerService = new CarOwnerService(carOwnerRepo);

                    var mockLogger = new Mock<ILogger<VehiclesController>>();
                    ILogger<VehiclesController> logger = mockLogger.Object;

                    var mockWebHostingEnvironment = new Mock<IWebHostEnvironment>();

                    var mockCustomAuthorizationService = new Mock<ICustomAuthorizationService>();
                    mockCustomAuthorizationService
                        .Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IAuthorizationRequirement>())).ReturnsAsync(AuthorizationResult.Success);

                    var controller = new VehiclesController( 
                        carService, 
                        logger,
                        new UriService("https://localhost:5010"),
                        new CustomMapper(
                            new VehicleImageRetriever()),
                        mockWebHostingEnvironment.Object,
                        new VehicleImageRetriever(),
                        mockCustomAuthorizationService.Object
                        );

                    Random rnd = new Random();
                    var index = rnd.Next(0, context.Users.AsNoTracking().ToHashSet().Count);
                    var owner = context.Users.AsNoTracking().ToList()[index];
                    var resJson = JsonConvert.SerializeObject(owner);
                    var deserializedOwner = JsonConvert.DeserializeObject<OwnerRequest>(resJson);

                    var res = await controller.GetCarsByCarOwner(deserializedOwner);

                    Assert.NotNull(res);
                    Assert.IsAssignableFrom<OkObjectResult>(res);
                }
            }

        }

        [Fact]
        public async Task TestGetCarsbyOwnerUsingIncudeThrowsExeption()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    var seed = new SeedData(new VehicleImageRetriever());
                    using var serviceScope = _serviceProvider.CreateScope();
                    var userManager = serviceScope.ServiceProvider.GetRequiredService<ICustomUserManager>();
                    var userRoleManager = serviceScope.ServiceProvider.GetRequiredService<ICustomRoleManager>();
                    await seed.Initialize(userManager, userRoleManager, context);

                    Random rnd = new Random();
                    var index = rnd.Next(0, context.Users.AsNoTracking().ToHashSet().Count);
                    var owner = context.Users.AsNoTracking().ToList()[index];

                    var res = await context.Users.AsNoTracking()
                    .Include(c => c.ManyToManyCustomUserToVehicle)
                        .ThenInclude(mtm => mtm.Car)
                    .FirstOrDefaultAsync(c => c.Id == owner.Id);

                    var cars = res.ManyToManyCustomUserToVehicle
                        .Select(mtm => mtm.Car).ToList();//inner manyToManyLoading explicit loading reference json serialization error

                    Assert.NotNull(cars);
                    Assert.IsAssignableFrom<List<Car>>(cars);
                    try 
                    {
                        var resJson = JsonConvert.SerializeObject(cars);
                    }
                    catch(Exception ex) 
                    {
                        Assert.NotNull(ex);
                        Assert.IsAssignableFrom<JsonSerializationException>(ex);
                    }
                    
                    
                }
            }
        }
        [Fact]
        public async Task TestGetCarOwnersby_UsingUniqueNumber_UsingIncudeThrowsExeption()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    var seed = new SeedData(new VehicleImageRetriever());
                    using var serviceScope = _serviceProvider.CreateScope();
                    var userManager = serviceScope.ServiceProvider.GetRequiredService<ICustomUserManager>();
                    var userRoleManager = serviceScope.ServiceProvider.GetRequiredService<ICustomRoleManager>();
                    await seed.Initialize(userManager, userRoleManager, context);


                    var carOwnerRepo = new CarOwnersRepository(context);
                    Random rnd = new Random();
                    var index = rnd.Next(0, context.Cars.AsNoTracking().ToHashSet().Count);
                    var car = context.Cars.AsNoTracking().ToList()[index];

                    var res = await context.Cars.AsNoTracking()
                    .Include(c => c.ManyToManyCustomUserToVehicle)
                        .ThenInclude(mtm => mtm.CarOwner)
                    .FirstOrDefaultAsync(c => c.UniqueNumber == car.UniqueNumber);

                    var owners = res.ManyToManyCustomUserToVehicle.Select(mtm => mtm.CarOwner).ToList();

                    Assert.NotNull(owners);
                    Assert.IsAssignableFrom<List<CustomUser>>(owners);
                    try
                    {
                        var resJson = JsonConvert.SerializeObject(owners);
                    }
                    catch (Exception ex)
                    {
                        Assert.NotNull(ex);
                        Assert.IsAssignableFrom<JsonSerializationException>(ex);
                    }


                }
            }
        }
        [Fact]
        public async Task TestVehicleController_PostVehicle()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    var seed = new SeedData(new VehicleImageRetriever());
                    using var serviceScope = _serviceProvider.CreateScope();
                    var userManager = serviceScope.ServiceProvider.GetRequiredService<ICustomUserManager>();
                    var userRoleManager = serviceScope.ServiceProvider.GetRequiredService<ICustomRoleManager>();
                    await seed.Initialize(userManager, userRoleManager, context);
                    
                    var carsRepo = new CarsRepository(context);
                    var carOwnerRepo = new CarOwnersRepository(context);
                    var identitySevice = new Mock<IIdentityService>();

                    var carService = new CarService(carsRepo,carOwnerRepo,identitySevice.Object);

                    var mockLogger = new Mock<ILogger<VehiclesController>>();
                    ILogger<VehiclesController> logger = mockLogger.Object;

                    var mockWebHostingEnvironment = new Mock<IWebHostEnvironment>();

                    var mockCustomAuthorizationService = new Mock<ICustomAuthorizationService>();
                    mockCustomAuthorizationService
                        .Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IAuthorizationRequirement>())).ReturnsAsync(AuthorizationResult.Success);

                    var controller = new VehiclesController(
                        carService, 
                        logger,
                        new UriService("https://localhost:5010/"),
                        new CustomMapper(new VehicleImageRetriever()),
                        mockWebHostingEnvironment.Object,
                        new VehicleImageRetriever(),
                        mockCustomAuthorizationService.Object
                        );

                    var testCar = new CarRequest
                    {
                        Brand = "Test",
                        Price = 23.4m,
                        Date = DateTime.Now,
                        CarEngine = 3,
                        Color = "Grey",
                        Description = "test",
                        Drive = "Mixed",
                        Transmision = "Auto",
                        UniqueNumber = SeedData.GenerateRandomRegistrationPlateNumber()
                    };

                    var res = await controller.PostCarItem(testCar);

                    Assert.NotNull(res);
                    var car = await context.Cars.AsNoTracking().SingleOrDefaultAsync(c=>c.UniqueNumber==testCar.UniqueNumber);

                    Assert.NotNull(car);
                }
            }
        }




    }
}
