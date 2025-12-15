namespace AspnetCoreRestApi.Services.Interfaces
{
    public interface ITodoNotificationPublisherService
    {
        Task SendNotification(int Id, string Title);
    }
}
