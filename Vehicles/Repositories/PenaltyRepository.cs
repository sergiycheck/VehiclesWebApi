using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vehicles.Models;
using Vehicles.Data;
using Vehicles.Interfaces.RepositoryInterfaces;
using Vehicles.Repositories;

namespace vehicles.Repositories
{
    public interface IPenaltyRepository : IGenericRepository<Penalty>
    {
         public Task<List<Penalty>>
            GetPenaltiesFromCarUniqueNumber(string uniqueNumber);
        public Task<Penalty> GetPenaltyWithCar(int id);
        public Task<int> PayPenalty(int id);

        public Task<List<Penalty>>
            GetPenaltiesByUserId(string userId);
        IQueryable<Penalty> GetAllPenaltiesWithCarIncluded();
    }

    public class PenaltyRepository:GenericRepository<Penalty>,IPenaltyRepository
    {
        public PenaltyRepository(VehicleDbContext context)
        : base(context)
        {

        }

        public  IQueryable<Penalty> GetAllPenaltiesWithCarIncluded()
        {
            return _dbContext.Penalties.AsNoTracking().Include(p=>p.Car).AsNoTracking(); 
        }

        //get penalties from carUnique number
        public async Task<List<Penalty>>
            GetPenaltiesFromCarUniqueNumber(string uniqueNumber)
        {
            var res = from car in _dbContext.Cars.AsNoTracking()
                      join penalty in _dbContext.Penalties.AsNoTracking()
                      on car.Id equals penalty.CarId
                      where car.UniqueNumber == uniqueNumber
                      select new Penalty
                      {
                          CarUniqueNumber = car.UniqueNumber,
                          CarId = car.Id,
                          Id = penalty.Id,
                          Description = penalty.Description,
                          Location = penalty.Location,
                          PayedStatus = penalty.PayedStatus,
                          Price = penalty.Price,
                          Date = penalty.Date
                      };
            return await res.ToListAsync();
        }

        public async Task<List<Penalty>>
            GetPenaltiesByUserId(string userId)
        {
            var res = from car in _dbContext.Cars.AsNoTracking()
                      join mtmCarOwner in _dbContext.ManyToManyCarOwners.AsNoTracking()
                      on car.Id equals mtmCarOwner.CarId
                      join penalty in _dbContext.Penalties.AsNoTracking()
                      on car.Id equals penalty.CarId

                      where mtmCarOwner.CarOwner.Id == userId
                      select new Penalty
                      {
                          CarUniqueNumber = car.UniqueNumber,
                          CarId = car.Id,
                          Id = penalty.Id,
                          Description = penalty.Description,
                          Location = penalty.Location,
                          PayedStatus = penalty.PayedStatus,
                          Price = penalty.Price,
                          Date = penalty.Date
                      };

            return await res.ToListAsync();
        }

        public async Task<Penalty> GetPenaltyWithCar(int id)
        {
            var penalty = await _dbContext.Penalties
                .AsNoTracking()
                .Include(p => p.Car)
                .FirstOrDefaultAsync(p => p.Id == id);
            return penalty;
        }

        public async Task<int> PayPenalty(int id)
        {
            var penalty = await _dbContext.Penalties
                .FirstOrDefaultAsync(penalty => penalty.Id == id);

            if (penalty == null)
                return 0;

            penalty.PayedStatus = true;

            var updatedResult = 0;
            try
            {
                updatedResult = await SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return updatedResult;
        }
    }
}
