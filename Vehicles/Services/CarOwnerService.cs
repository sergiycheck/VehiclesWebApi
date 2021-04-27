using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Vehicles.Models;
using Vehicles.Interfaces.RepositoryInterfaces;
using Vehicles.Interfaces.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;
using vehicles.Contracts.V1.Requests;
using vehicles.Contracts.V1.Requests.Queries;

namespace Vehicles.Services
{
    public class CarOwnerService:ICarOwnerService
    {
        private readonly ICarOwnersRepository _repository;
        public CarOwnerService(ICarOwnersRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedList<CustomUser>> GetAllCarOwners(UsersParameters usersParameters)
        {
            var iQueryableUsers = _repository.GetIQueryableUsers();
            return await PagedList<CustomUser>
                .ToPagedList(
                    iQueryableUsers,
                    usersParameters.PageNum,
                    usersParameters.PageSize
                );
        }

        public async Task<CustomUser> GetById(string id)
        {
            return await _repository.GetById(id);
        }

        public async Task<List<CustomUser>> GetCarOwners(string UniqueNumber)
        {
            return await _repository.GetCarOwners(UniqueNumber);
        }
    }
}