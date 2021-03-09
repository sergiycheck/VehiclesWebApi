using Vehicles.Interfaces;
using Vehicles.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Vehicles.Interfaces.RepositoryInterfaces
{
    public interface ICarsRepository:IGenericRepository<Car>
    {
        Task<List<Car>> GetCars(CustomUser carOwner);
    }
}