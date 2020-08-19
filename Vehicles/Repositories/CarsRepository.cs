using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vehicles.Interfaces.RepositoryInterfaces;
using Vehicles.Models;
using Microsoft.EntityFrameworkCore;
using Vehicles.Data;

namespace Vehicles.Repositories
{
    public class CarsRepository : GenericRepository<Car>, ICarsRepository
    {
        public CarsRepository(VehicleDbContext context)
            : base(context)
        {
               
        }
        public async Task<List<Car>> GetCars(CarOwner carOwner)
        {
            var res = from car in _dbContext.Cars.AsNoTracking()
                      join mtmCarOwner in _dbContext.ManyToManyCarOwners.AsNoTracking()
                      on car.Id equals mtmCarOwner.CarId
                      where mtmCarOwner.CarOwner.Id == carOwner.Id
                      select new Car
                      {
                          Id = car.Id,
                          UniqueNumber = car.UniqueNumber,
                          Brand = car.Brand,
                          Color = car.Color,
                          Date = car.Date,
                          Price = car.Price,
                          CarEngine = car.CarEngine,
                          Description = car.Description,
                          Transmision = car.Transmision,
                          Drive = car.Drive
                      };

            return await res.ToListAsync();
            //var res = await _dbContext.CarOwners.AsNoTracking()
            //.Include(c => c.ManyToManyCarOwner)
            //    .ThenInclude(mtm => mtm.Car)
            //.FirstOrDefaultAsync(c => c.Id == carOwner.Id);

            //var cars = res.ManyToManyCarOwner.Select(mtm=>mtm.Car).ToList();//inner manyToManyLoading explicit loading reference json serialization error

            //return cars;

        }

    }
}
