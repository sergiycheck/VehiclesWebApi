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

namespace VehiclesXUnitTests
{
    public class VehiclesTests : IClassFixture<SqlServerTests>
    {
        public VehiclesTests(SqlServerTests fixture) => Fixture = fixture;
        public SqlServerTests Fixture { get; }


        [Fact]
        public void TestPair()
        {
            var excludePair = new HashSet<CustomPair>();
            var car_id = 1;
            var car_owner_id = 1;
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
                    await SeedData.Initialize(context);

                    Assert.NotNull(context.CarOwners);
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
                    await SeedData.Initialize(context);
                    var carOwnerRepo = new CarOwnersRepository(context);
                    Random rnd = new Random();
                    var index = rnd.Next(0, context.Cars.AsNoTracking().ToHashSet().Count);
                    var car =  context.Cars.AsNoTracking().ToList()[index];
                    var res = await carOwnerRepo.GetCarOwners(car.UniqueNumber);

                    Assert.NotNull(res);
                    Assert.IsAssignableFrom<List<CarOwner>>(res);

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
                    await SeedData.Initialize(context);
                    var carsRepo = new CarsRepository(context);
                    Random rnd = new Random();
                    var index = rnd.Next(0, context.CarOwners.AsNoTracking().ToHashSet().Count);
                    var owner =  context.CarOwners.AsNoTracking().ToList()[index];
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
                    await SeedData.Initialize(context);
                    var carsRepo = new CarsRepository(context);
                    var carOwnerRepo = new CarOwnersRepository(context);

                    var carService = new CarService(carsRepo);
                    var carOwnerService = new CarOwnerService(carOwnerRepo);

                    var mockLogger = new Mock<ILogger<VehiclesController>>();
                    ILogger<VehiclesController> logger = mockLogger.Object;

                    var controller = new VehiclesController(carOwnerService, carService, logger);

                    Random rnd = new Random();
                    var index = rnd.Next(0, context.Cars.AsNoTracking().ToHashSet().Count);
                    var car = context.Cars.AsNoTracking().ToList()[index];

                    var res = await controller.GetOwnersByCarUniqueNumber(car.UniqueNumber);

                    Assert.NotNull(res);
                    Assert.IsAssignableFrom<ActionResult<IEnumerable<CarOwner>>>(res);
                    Assert.IsAssignableFrom<IEnumerable<CarOwner>>(res.Value);

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
                    await SeedData.Initialize(context);
                    var carsRepo = new CarsRepository(context);
                    var carOwnerRepo = new CarOwnersRepository(context);

                    var carService = new CarService(carsRepo);
                    var carOwnerService = new CarOwnerService(carOwnerRepo);

                    var mockLogger = new Mock<ILogger<VehiclesController>>();
                    ILogger<VehiclesController> logger = mockLogger.Object;

                    var controller = new VehiclesController(carOwnerService, carService, logger);

                    Random rnd = new Random();
                    var index = rnd.Next(0, context.CarOwners.AsNoTracking().ToHashSet().Count);
                    var owner = context.CarOwners.AsNoTracking().ToList()[index];
                    var resJson = JsonConvert.SerializeObject(owner);
                    var deserializedOwner = JsonConvert.DeserializeObject<CarOwner>(resJson);

                    var res = await controller.GetCarsByCarOwner(deserializedOwner);

                    Assert.NotNull(res);
                    Assert.IsAssignableFrom<ActionResult<IEnumerable<Car>>>(res);
                    Assert.IsAssignableFrom<IEnumerable<Car>>(res.Value);

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
                    await SeedData.Initialize(context);

                    Random rnd = new Random();
                    var index = rnd.Next(0, context.CarOwners.AsNoTracking().ToHashSet().Count);
                    var owner = context.CarOwners.AsNoTracking().ToList()[index];

                    var res = await context.CarOwners.AsNoTracking()
                    .Include(c => c.ManyToManyCarOwner)
                        .ThenInclude(mtm => mtm.Car)
                    .FirstOrDefaultAsync(c => c.Id == owner.Id);

                    var cars = res.ManyToManyCarOwner.Select(mtm => mtm.Car).ToList();//inner manyToManyLoading explicit loading reference json serialization error

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
                    await SeedData.Initialize(context);
                    var carOwnerRepo = new CarOwnersRepository(context);
                    Random rnd = new Random();
                    var index = rnd.Next(0, context.Cars.AsNoTracking().ToHashSet().Count);
                    var car = context.Cars.AsNoTracking().ToList()[index];

                    var res = await context.Cars.AsNoTracking()
                    .Include(c => c.ManyToManyCarOwner)
                        .ThenInclude(mtm => mtm.CarOwner)
                    .FirstOrDefaultAsync(c => c.UniqueNumber == car.UniqueNumber);

                    var owners = res.ManyToManyCarOwner.Select(mtm => mtm.CarOwner).ToList();

                    Assert.NotNull(owners);
                    Assert.IsAssignableFrom<List<CarOwner>>(owners);
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

    }
}
