using AspnetCoreRestApi.Configurations.Interfaces;

namespace AspnetCoreRestApi.Helpers
{
    public class CorrelationIdMiddleware
    {
        private const string _correlaitonIdHeader = "X-Correlation-ID";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICorellationIdGenerator corellationIdGenerator)
        {
            var correlationId = GetCorrelation(context, corellationIdGenerator);            

            AddCorrelationResponse(context, correlationId);

            // context.Request.Headers.Add(_correlaitonIdHeader, correlationId);

            await _next(context);
        }

        private static void AddCorrelationResponse(HttpContext context, string correlationId)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add(_correlaitonIdHeader, correlationId);
                return Task.CompletedTask;
            });
        }

        private string GetCorrelation(HttpContext context, ICorellationIdGenerator corellationIdGenerator)
        {
            if (context.Request.Headers.TryGetValue(_correlaitonIdHeader, out var corellationId))
            {
                corellationIdGenerator.Set(corellationId);

                return corellationId;
            }

            return corellationIdGenerator.Get();
        }
    }
}
