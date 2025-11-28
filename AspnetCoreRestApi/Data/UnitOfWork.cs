using AspnetCoreRestApi.Core.IConfiguration;
using AspnetCoreRestApi.Core.IRepositories;
using AspnetCoreRestApi.Core.Repositories;
using System.Threading.Tasks;

namespace AspnetCoreRestApi.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApiDbContext _context;

        private readonly ILogger _logger;

        public UnitOfWork(ApiDbContext context, ILoggerFactory logger)
        {
            _context = context;
            _logger = logger.CreateLogger("logs");
            UserRepository = new UserRepository(_context, logger.CreateLogger<GenericRepository<Models.DbSetModels.User>>());
        }

        public IUserRepository UserRepository { get; private set; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IGenericRepository<T> GenericRepository<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
