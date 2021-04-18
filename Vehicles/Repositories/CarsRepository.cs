using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vehicles.Interfaces.RepositoryInterfaces;
using Vehicles.Models;
using Microsoft.EntityFrameworkCore;
using Vehicles.Data;
using Microsoft.Data.SqlClient;

namespace Vehicles.Repositories
{
    public class CarsRepository : GenericRepository<Car>, ICarsRepository
    {
        public CarsRepository(VehicleDbContext context)
            : base(context)
        {
               
        }
        public async Task<List<Car>> GetCars(CustomUser carOwner)
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
                          Drive = car.Drive,
                          ImgPath = car.ImgPath
                      };

            return await res.ToListAsync();
            //var res = await _dbContext.CarOwners.AsNoTracking()
            //.Include(c => c.ManyToManyCarOwner)
            //    .ThenInclude(mtm => mtm.Car)
            //.FirstOrDefaultAsync(c => c.Id == carOwner.Id);

            //var cars = res.ManyToManyCarOwner.Select(mtm=>mtm.Car).ToList();//inner manyToManyLoading explicit loading reference json serialization error

            //return cars;

        }

        public async Task<int> TotalCount()
        {
            return await _dbContext.Cars.AsNoTracking().CountAsync();
        }

        public IQueryable<Car> PaginationQuery(int pageNum, int PageSize)
        {
            return _dbContext.Cars.AsQueryable().AsNoTracking()
                .Skip(PageSize * (pageNum - 1))
                .Take(PageSize);

        }
        public IQueryable<Car> GetIQueryableCars()
        {
            return _dbContext.Cars.AsQueryable().AsNoTracking();
        }

        public IQueryable<Car> SearchQuery
            ( IQueryable<Car> carQueryable, string search)
        {


            return carQueryable
                .Where(
                c => c.UniqueNumber.Contains(search) ||
                c.Brand.Contains(search) ||

                c.Color.Contains(search) ||

                c.Description.Contains(search) ||
                c.Transmision.Contains(search) ||
                c.Drive.Contains(search)
                );

        }
        public IQueryable<Car> SearchQueryPrice
            (IQueryable<Car> carQueryable, decimal? max,decimal? min)
        {
            if(max!=null && min != null)
            {
                return carQueryable
                    .Where(c => c.Price >= min && c.Price <= max);
            }
            if (max != null)
            {
                return carQueryable.Where(c => c.Price <= max);
            }
            if(min != null)
            {
                return carQueryable.Where(c => c.Price >= min);
            }
            return carQueryable;
        }

        public IQueryable<Car> SearchQueryCarEngine
            (IQueryable<Car> carQueryable, int? max, int? min)
        {
            if(max!= null && min != null)
            {
                return carQueryable
                .Where(c => c.CarEngine >= min && c.CarEngine <= max);
            }
            if(max != null)
            {
                return carQueryable
                .Where(c => c.CarEngine <= max);
            }
            if (min != null)
            {
                return carQueryable
                .Where(c => c.CarEngine >= min);
            }

            return carQueryable;
           
        }

        public IQueryable<Car> SearchQueryDate
            (IQueryable<Car> carQueryable, DateTime? max, DateTime? min)
        {

            if(max!=null && min != null)
            {
                return carQueryable
                .Where(
                    c =>
                        DateTime.Compare(c.Date, (DateTime)min) >= 0 &&
                        DateTime.Compare(c.Date, (DateTime)max) <= 0);
            }
            if (max != null)
            {
                return carQueryable
                .Where(
                    c =>
                        DateTime.Compare(c.Date, (DateTime)max) <= 0);
            }
            if ( min != null)
            {
                return carQueryable
                .Where(
                    c =>
                        DateTime.Compare(c.Date, (DateTime)min) >= 0);
            }
            return carQueryable;

        }


    }
}
