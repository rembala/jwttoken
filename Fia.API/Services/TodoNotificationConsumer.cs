using Common.Contracts;
using MassTransit;

namespace Fia.API.Services
{
    public class TodoNotificationConsumer : IConsumer<TotoNotificationRecord>
    {
        public TodoNotificationConsumer()
        {
            
        }

        private readonly ILogger<TodoNotificationConsumer> _logger;

        public TodoNotificationConsumer(ILogger<TodoNotificationConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TotoNotificationRecord> context)
        {
            _logger.LogInformation("Received Todo Id: {Id}, Title: {Title}", context.Message.Id, context.Message.Title);

            await Task.CompletedTask;
        }
    }
}
