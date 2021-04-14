using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Vehicles.Models;
using System.Security.Claims;

namespace Vehicles.Interfaces.ServiceInterfaces
{
    public interface ICarService
    {
        Task<List<Car>> GetCars(CustomUser carOwner);
        Task<List<Car>> GetAllCars();
        Task<Car> GetById(int? id);
        Task Create(Car entity);
        Task<int> Update(Car entity);
        Task Delete(int? id);
        bool EntityExists(int id);
        Task<CustomUser> GetOwnerById(string id);
        ClaimsPrincipal GetClaimsPrincipal(string token);

        Task<List<CustomUser>> GetOwnersByCar(int id);
    }
}