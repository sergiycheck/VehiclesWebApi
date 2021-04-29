using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Vehicles.Models;
using Vehicles.Interfaces.RepositoryInterfaces;
using Vehicles.Interfaces.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using vehicles.Contracts.V1.Requests;
using vehicles.Contracts.V1.Requests.Queries;
using System.Linq;

namespace Vehicles.Services
{
    public class CarService:ICarService
    {
        private readonly ICarsRepository _repository;

        private readonly ICarOwnersRepository _ownersRepository;
        private readonly IIdentityService _identityService;
        public CarService(ICarsRepository repository, 
            ICarOwnersRepository carOwnersRepository, 
            IIdentityService identityService)
        {
            _repository = repository;
            _ownersRepository = carOwnersRepository;
            _identityService = identityService;
        }

        public ClaimsPrincipal GetClaimsPrincipal(string token)
        {
            return _identityService.GetPrincipalFromToken(token);
        }
        public async Task<CustomUser> GetOwnerById(string id)
        {
            return await _ownersRepository.GetById(id);
        }
        public async Task<List<CustomUser>> GetOwnersByCar(int id)
        {
            var car = await _repository.GetById(id);
            if(car == null)
            {
                return null;
            }
            return await _ownersRepository.GetCarOwners(car.UniqueNumber);
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

        public async Task<PagedList<Car>> GetAllCars(CarsParameters carsParameters)
        {
            var iQueryableCars = _repository.GetIQueryableCars().OrderBy(c=>c.Brand);
            return await PagedList<Car>
                .ToPagedList(
                iQueryableCars, 
                    carsParameters.PageNum, 
                    carsParameters.PageSize);
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