using ContactQueueService.Configuration;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ContactQueueService.MessageBroker
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContactBus(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMQSettings = new RabbitMQSettings()
            {
                ConnectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING"),
                Queues = new RabbitMQSettings.QueueSettings()
            };
            
            services.AddSingleton(rabbitMQSettings);

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                
                x.UsingRabbitMq((context, cfg) =>
                {
                    Uri uri = new Uri(rabbitMQSettings.ConnectionString);
                    cfg.PublishTopology.BrokerTopologyOptions = PublishBrokerTopologyOptions.MaintainHierarchy;
                    
                    cfg.Host(uri);
                });
            });

            services.AddScoped<IContactBus, ContactBus>();
            return services;
        }
    }
}