using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Vehicles.Models;
using Vehicles.Interfaces.RepositoryInterfaces;
using Vehicles.Interfaces.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Vehicles.Services
{
    public class CarOwnerService:ICarOwnerService
    {
        private readonly ICarOwnersRepository _repository;
        public CarOwnerService(ICarOwnersRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CarOwner>> GetAllCarOwners()
        {
            return await _repository.GetAll().ToListAsync();
        }

        public async Task<CarOwner> GetById(int? id)
        {
            return await _repository.GetById(id);
        }

        public async Task<List<CarOwner>> GetCarOwners(string UniqueNumber)
        {
            return await _repository.GetCarOwners(UniqueNumber);
        }
    }
}