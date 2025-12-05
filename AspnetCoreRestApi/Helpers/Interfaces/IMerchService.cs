namespace AspnetCoreRestApi.Helpers.Interfaces
{
    public interface IMerchService
    {
        void CreateMerch(Guid driverId);

        void RemovedMerch(Guid driverId);
    }
}
