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
    }
}