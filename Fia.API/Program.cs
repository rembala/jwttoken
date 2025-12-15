using Fia.API.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMassTransit(x =>
{
    // Register your consumers
    x.AddConsumer<TodoNotificationConsumer>();

    // Configure RabbitMQ
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMqConfig:Host"], h =>
        {
            h.Username(builder.Configuration["RabbitMqConfig:Username"]);
            h.Password(builder.Configuration["RabbitMqConfig:Password"]);
        });

        // Define the receive endpoint for this consumer
        cfg.ReceiveEndpoint("todo-notification-queue", e =>
        {
            e.ConfigureConsumer<TodoNotificationConsumer>(context);
        });
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
