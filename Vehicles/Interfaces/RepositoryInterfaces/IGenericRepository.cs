using System.Linq;
using System.Threading.Tasks;
using Vehicles.Models;

namespace Vehicles.Interfaces.RepositoryInterfaces
{
    public interface IGenericRepository<TEntity> where TEntity:BaseModel
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity> GetById(int? id);
        Task Create(TEntity entity);
        void Update(TEntity entity);
        Task Delete(int? id);
        Task<int> SaveChangesAsync();
        bool EntityExists(int id);
    }
}
