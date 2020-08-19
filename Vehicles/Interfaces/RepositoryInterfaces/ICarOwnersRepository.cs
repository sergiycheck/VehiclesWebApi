using Vehicles.Interfaces;
using Vehicles.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;


namespace Vehicles.Interfaces.RepositoryInterfaces
{
    public interface ICarOwnersRepository:IGenericRepository<CarOwner>
    {
        Task<List<CarOwner>> GetCarOwners(string UniqueNumber);
    }
}