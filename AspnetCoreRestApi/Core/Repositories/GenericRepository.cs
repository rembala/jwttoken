using AspnetCoreRestApi.Core.IRepositories;
using AspnetCoreRestApi.Data;
using Microsoft.EntityFrameworkCore;

namespace AspnetCoreRestApi.Core.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected ApiDbContext _context;

        protected DbSet<T> dbSet;

        protected readonly ILogger<GenericRepository<T>> _logger;

        protected GenericRepository(
            ApiDbContext apiDbContext,
            ILogger<GenericRepository<T>> logger)
        {
            _context = apiDbContext;
            _logger = logger;
            dbSet = _context.Set<T>();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            return (await dbSet.AddAsync(entity)).Entity;
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await dbSet.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            dbSet.Remove(entity);
            return true;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            return dbSet.Update(entity).Entity;
        }
    }
}
