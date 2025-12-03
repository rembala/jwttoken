using AspnetCoreRestApi.Helpers;

namespace AspnetCoreRestApi.Services
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCorrelationIdMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationIdMiddleware>();
        }
    }
}
