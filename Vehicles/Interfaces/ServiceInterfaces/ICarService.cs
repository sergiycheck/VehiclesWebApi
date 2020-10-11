using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Vehicles.Models;

namespace Vehicles.Interfaces.ServiceInterfaces
{
    public interface ICarService
    {
        Task<List<Car>> GetCars(CarOwner carOwner);
        Task<List<Car>> GetAllCars();
        Task<Car> GetById(int? id);
        Task Create(Car entity);
        Task<int> Update(Car entity);
        Task Delete(int? id);
        bool EntityExists(int id);

    }
}