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

        public async Task Create(Car entity)
        {
            await _repository.Create(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task Delete(int? id)
        {
            await _repository.Delete(id);
            await _repository.SaveChangesAsync();
        }

        public bool EntityExists(int id)
        {
            return _repository.EntityExists(id);
        }

        public async Task<List<Car>> GetAllCars()
        {
            return await _repository.GetAll().ToListAsync();
        }

        public async Task<Car> GetById(int? id)
        {
            return await _repository.GetById(id);
        }

        public async Task<List<Car>> GetCars(CustomUser carOwner)
        {
            return await _repository.GetCars(carOwner);
        }

        public async Task<int> Update(Car entity)
        {
            _repository.Update(entity);
            try
            {
                return await _repository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return 0;
            
        }
    }
}