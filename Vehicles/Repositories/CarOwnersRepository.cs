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
    public class CarOwnersRepository : GenericRepository<CarOwner>, ICarOwnersRepository
    {
        public CarOwnersRepository(VehicleDbContext context)
            : base(context)
        {
               
        }
        public async Task<List<CarOwner>> GetCarOwners(string uniqueNumber)
        {

            var res = from carOwner in _dbContext.CarOwners.AsNoTracking()
                      join mtmCarOwner in _dbContext.ManyToManyCarOwners.AsNoTracking()
                      on carOwner.Id equals mtmCarOwner.CarOwnerId
                      where mtmCarOwner.Car.UniqueNumber == uniqueNumber
                      select new CarOwner
                      {
                          Id = carOwner.Id,
                          Name = carOwner.Name,
                          SurName = carOwner.SurName,
                          Patronymic = carOwner.Patronymic,
                          CarOwnerPhone = carOwner.CarOwnerPhone,
                          Location = carOwner.Location,
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
