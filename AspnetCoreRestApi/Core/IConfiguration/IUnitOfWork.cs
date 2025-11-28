using AspnetCoreRestApi.Core.IRepositories;

namespace AspnetCoreRestApi.Core.IConfiguration
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IGenericRepository<T> GenericRepository<T>() where T : class;
        Task SaveAsync();
    }
}
