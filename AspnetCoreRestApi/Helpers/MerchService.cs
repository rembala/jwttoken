using AspnetCoreRestApi.Helpers.Interfaces;

namespace AspnetCoreRestApi.Helpers
{
    public class MerchService : IMerchService
    {
        public void CreateMerch(Guid driverId)
        {
            Console.WriteLine($"This will create merch for driver with ID: {driverId}");
        }

        public void RemovedMerch(Guid driverId)
        {
            Console.WriteLine($"This will remove merch for driver with ID: {driverId}");
        }
    }
}
