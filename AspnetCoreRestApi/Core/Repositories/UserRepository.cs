using AspnetCoreRestApi.Core.IRepositories;
using AspnetCoreRestApi.Data;
using AspnetCoreRestApi.Models.DbSetModels;
using Microsoft.EntityFrameworkCore;

namespace AspnetCoreRestApi.Core.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApiDbContext apiDbContext, ILogger<GenericRepository<User>> logger) : base(apiDbContext, logger)
        {

        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await dbSet.ToListAsync();
            }
            catch (Exception)
            {
                _logger.LogError("An error occurred while retrieving all users.");

                return Enumerable.Empty<User>();
            }
        }

        public override async Task<User?> GetByIdAsync(Guid id)
        {
            try
            {
                return await dbSet.FindAsync(id);
            }
            catch (Exception)
            {
                _logger.LogError("An error occurred while retrieving the user with ID: {UserId}", id);
                return null;
            }
        }

        public override async Task<User> AddAsync(User entity)
        {
            try
            {
               var existingUser = await dbSet.FirstOrDefaultAsync(u => u.Id == entity.Id);

                if (existingUser == null)
                {
                    return await base.AddAsync(entity);
                }

                existingUser.Email = entity.Email;
                existingUser.FirstName = entity.FirstName;
                existingUser.LastName = entity.LastName;

                return existingUser;
            }
            catch (Exception)
            {
                _logger.LogError("An error occurred while adding a new user.");
                throw;
            }
        }

        public override async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var existingUser = await dbSet.FindAsync(id);
                if (existingUser == null)
                {
                    return false;
                }
                dbSet.Remove(existingUser);

                return true;
            }
            catch (Exception)
            {
                _logger.LogError("An error occurred while deleting the user with ID: {UserId}", id);
                return false;
            }
        }
    }
}
