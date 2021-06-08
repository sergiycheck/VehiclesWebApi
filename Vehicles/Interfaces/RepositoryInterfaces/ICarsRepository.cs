using Vehicles.Interfaces;
using Vehicles.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Vehicles.Interfaces.RepositoryInterfaces
{
    public interface ICarsRepository:IGenericRepository<Car>
    {
        Task<List<Car>> GetCars(CustomUser carOwner);
        IQueryable<Car> PaginationQuery(int PageNum, int PageSize);
        IQueryable<Car> GetIQueryableCars();
        Task<int> AddVehicleRelationToUser(int carId, string userId);

        Task<int> TotalCount();
        IQueryable<Car> SearchQuery
            (IQueryable<Car> carQueryable, string search);
        IQueryable<Car> SearchQueryPrice
            (IQueryable<Car> carQueryable, decimal? max, decimal? min);
        IQueryable<Car> SearchQueryCarEngine
            (IQueryable<Car> carQueryable, float? max, float? min);
        IQueryable<Car> SearchQueryDate
            (IQueryable<Car> carQueryable, DateTime? max, DateTime? min);

    }
}