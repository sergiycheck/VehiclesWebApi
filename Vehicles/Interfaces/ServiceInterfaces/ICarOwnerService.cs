using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Vehicles.Models;

namespace Vehicles.Interfaces.ServiceInterfaces
{
    public interface ICarOwnerService
    {
        Task<List<CustomUser>> GetCarOwners(string UniqueNumber);
        Task<List<CustomUser>> GetAllCarOwners();
        Task<CustomUser> GetById(string id);
    }
}