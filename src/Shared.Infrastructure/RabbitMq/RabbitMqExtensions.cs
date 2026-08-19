using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Infrastructure.RabbitMq;

public static class RabbitMqExtensions
{
    public static IServiceCollection AddRabbitMqMessaging(
        this IServiceCollection services, 
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureBus = null)
    {
        services.AddMassTransit(x =>
        {
            configureBus?.Invoke(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = configuration.GetConnectionString("RabbitMq") ?? "amqp://guest:guest@rabbitmq:5672";
                cfg.Host(connectionString);
                
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
