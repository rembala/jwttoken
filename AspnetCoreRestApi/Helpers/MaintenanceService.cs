using AspnetCoreRestApi.Helpers.Interfaces;

namespace AspnetCoreRestApi.Helpers
{
    public class MaintenanceService : IMaintenanceService
    {
        public void SyncRecords()
        {
            Console.WriteLine("This will sync records in the system.");
        }
    }
}
