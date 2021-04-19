using Vehicles.Interfaces;
using Vehicles.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Vehicles.Interfaces.RepositoryInterfaces
{
    public interface ICarsRepository:IGenericRepository<Car>
    {
        Task<List<Car>> GetCars(CustomUser carOwner);
        IQueryable<Car> PaginationQuery(int PageNum, int PageSize);
        IQueryable<Car> GetIQueryableCars();
    }
}