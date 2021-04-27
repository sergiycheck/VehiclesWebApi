using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Vehicles.Models;
using vehicles.Contracts.V1.Requests;
using vehicles.Contracts.V1.Requests.Queries;

namespace Vehicles.Interfaces.ServiceInterfaces
{
    public interface ICarOwnerService
    {
        Task<List<CustomUser>> GetCarOwners(string UniqueNumber);
        Task<PagedList<CustomUser>> GetAllCarOwners(UsersParameters usersParameters);
        Task<CustomUser> GetById(string id);
    }
}