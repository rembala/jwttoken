using AspnetCoreRestApi.Services.Interfaces;
using Common.Contracts;
using MassTransit;
using MassTransit.Transports;

namespace AspnetCoreRestApi.Services
{
    public class TodoNotificationPublisherService : ITodoNotificationPublisherService
    {
        private readonly ILogger<TodoNotificationPublisherService> _logger;
        private readonly IPublishEndpoint _bus;

        public TodoNotificationPublisherService(ILogger<TodoNotificationPublisherService> logger, IPublishEndpoint bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public async Task SendNotification(int Id, string Title)
        {
            _logger.LogInformation("Publishing notification for Todo Item with Id: {Id} and Title: {Title}", Id, Title);
            await _bus.Publish(new TotoNotificationRecord(Id, Title)); 
        }
    }
}
