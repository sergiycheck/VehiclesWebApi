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
    public class CarOwnersRepository :  ICarOwnersRepository
    {
        protected readonly VehicleDbContext _dbContext;
        protected readonly DbSet<CustomUser> _dbSet;
        public CarOwnersRepository(VehicleDbContext context)
        {
            _dbContext = context;
            _dbSet=_dbContext.Set<CustomUser>();
        }

        public async Task<List<CustomUser>> GetAll()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }
        public async Task<CustomUser> GetById(string id)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(user=>user.Id==id);
        }
        public async Task<List<CustomUser>> GetCarOwners(string uniqueNumber)
        {

            var res = from carOwner in _dbContext.Users.AsNoTracking()
                      join mtmCarOwner in _dbContext.ManyToManyCarOwners.AsNoTracking()
                      on carOwner.Id equals mtmCarOwner.CarOwnerId
                      where mtmCarOwner.Car.UniqueNumber == uniqueNumber
                      select new CustomUser
                      {
                          Id = carOwner.Id,
                          FirstName = carOwner.FirstName,
                          LastName = carOwner.LastName,
                          PhoneNumber = carOwner.PhoneNumber,
                          Address = carOwner.Address,
                          BirthDate = carOwner.BirthDate
                      };
            return await res.ToListAsync();
            //var res = await _dbContext.Cars.AsNoTracking()
            //.Include(c => c.ManyToManyCarOwner)
            //    .ThenInclude(mtm => mtm.CarOwner)
            //.FirstOrDefaultAsync(c => c.UniqueNumber == uniqueNumber);

            //var owners = res.ManyToManyCarOwner.Select(mtm=>mtm.CarOwner).ToList();

            //return owners;
        }
    }
}
