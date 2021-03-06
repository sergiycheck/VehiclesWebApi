using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vehicles.Models;
using Vehicles.Interfaces.RepositoryInterfaces;
using Vehicles.Data;

namespace Vehicles.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseModel
    {
        public readonly VehicleDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;
        
        public GenericRepository(VehicleDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet=_dbContext.Set<TEntity>();
        }
        public IQueryable<TEntity> GetAll()
        {
            return _dbSet.AsNoTracking();
        }
        public async Task<TEntity> GetById(int? id)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(entity=>entity.Id==id);
        }
        public async Task Create(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }
        public async Task Delete(int? id)
        {
            _dbSet.Remove(await GetById(id));
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        public bool EntityExists(int id)
        {
            return _dbSet.Any(e => e.Id == id);
        }
    }
}