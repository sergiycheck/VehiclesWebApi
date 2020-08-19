using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Vehicles.Models;
using Vehicles.Interfaces.RepositoryInterfaces;
using Vehicles.Interfaces.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Vehicles.Services
{
    public class CarService:ICarService
    {
        private readonly ICarsRepository _repository;
        public CarService(ICarsRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Car>> GetAllCars()
        {
            return await _repository.GetAll().ToListAsync();
        }

        public async Task<List<Car>> GetCars(CarOwner carOwner)
        {
            return await _repository.GetCars(carOwner);
        }
    }
}