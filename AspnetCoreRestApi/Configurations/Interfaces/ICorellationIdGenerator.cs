namespace AspnetCoreRestApi.Configurations.Interfaces
{
    public interface ICorellationIdGenerator
    {
        string Get();

        void Set(string corellationId);
    }
}
