using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Vehicles.Models;
using System.Security.Claims;
using vehicles.Contracts.V1.Requests;
using vehicles.Contracts.V1.Requests.Queries;

namespace Vehicles.Interfaces.ServiceInterfaces
{
    public interface ICarService
    {
        Task<List<Car>> GetCars(CustomUser carOwner);
        Task<PagedList<Car>> GetAllCars(CarsParameters carsParameters);
        Task<Car> GetById(int? id);
        Task<bool> Create(Car entity, string userId);
        Task<int> Update(Car entity);
        Task Delete(int? id);
        bool EntityExists(int id);
        Task<CustomUser> GetOwnerById(string id);
        ClaimsPrincipal GetClaimsPrincipal(string token);

        Task<List<CustomUser>> GetOwnersByCar(int id);
    }
}