using AspnetCoreRestApi.Configurations.Interfaces;

namespace AspnetCoreRestApi.Configurations
{
    public class CorrelationIdGenerator : ICorellationIdGenerator
    {
        private string _corellationId = Guid.NewGuid().ToString();

        public string Get() => _corellationId;

        public void Set(string corellationId) => _corellationId = corellationId;
    }
}
