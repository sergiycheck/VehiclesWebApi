using Vehicles.Interfaces;
using Vehicles.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace Vehicles.Interfaces.RepositoryInterfaces
{
    public interface ICarOwnersRepository
    {
        Task<List<CustomUser>> GetAll();
        Task<CustomUser> GetById(string id);
        Task<List<CustomUser>> GetCarOwners(string UniqueNumber);
        public IQueryable<CustomUser> GetIQueryableUsers();
    }
}