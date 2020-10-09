using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Vehicles.Models;

namespace Vehicles.Interfaces.ServiceInterfaces
{
    public interface ICarOwnerService
    {
        Task<List<CarOwner>> GetCarOwners(string UniqueNumber);
        Task<List<CarOwner>> GetAllCarOwners();
        Task<CarOwner> GetById(int? id);
    }
}