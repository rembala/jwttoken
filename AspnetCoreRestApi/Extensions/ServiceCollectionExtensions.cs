using AspnetCoreRestApi.Configurations;
using AspnetCoreRestApi.Configurations.Interfaces;

namespace AspnetCoreRestApi.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCorrelationIdManager(this IServiceCollection services)
        {
            services.AddScoped<ICorellationIdGenerator, CorrelationIdGenerator>();

            return services;
        }
    }
}
