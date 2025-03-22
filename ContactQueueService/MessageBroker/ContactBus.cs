using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ContactQueueService.Configuration;
using ContactQueueService.Dto;
using MassTransit;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;

namespace ContactQueueService.MessageBroker
{
    public class ContactBus : IContactBus
    {
        private readonly IBus _bus;
        private readonly ILogger<ContactBus> _logger;
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly System.Diagnostics.ActivitySource _activitySource;
        private static readonly TextMapPropagator _propagator = Propagators.DefaultTextMapPropagator;


        public ContactBus(IBus bus, ILogger<ContactBus> logger, RabbitMQSettings rabbitMQSettings, ActivitySource activitySource)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _rabbitMQSettings = rabbitMQSettings ?? throw new ArgumentNullException(nameof(rabbitMQSettings));
            _activitySource = activitySource;
        }

        public async Task PublishContactCreated(ContactDto contact)
        {
            try
            {
                _logger.LogInformation("Publishing contact created");
                // Criar uma nova Activity para rastrear a operação de publicação
                using var activity = _activitySource.StartActivity(
                    $"PublishContactCreated",
                    ActivityKind.Producer);
                // Adicionar atributos importantes à activity atual
                activity?.SetTag("messaging.system", "rabbitmq");
                activity?.SetTag("messaging.destination_kind", "queue");
                activity?.SetTag("messaging.destination", _rabbitMQSettings.Queues.ContactCreate);
                activity?.SetTag("messaging.rabbitmq.routing_key", _rabbitMQSettings.Queues.ContactCreate);

                var queueUri = new Uri($"{_rabbitMQSettings.ConnectionString}/{_rabbitMQSettings.Queues.ContactCreate}");
                var endpoint = await _bus.GetSendEndpoint(queueUri);
                
                activity?.SetStatus(ActivityStatusCode.Ok);
                await endpoint.Send(contact);
                
                _logger.LogInformation("Contact published to create queue");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing contact created for contact");
                throw;
            }
        }

        public async Task PublishContactUpdated(ContactDto contact)
        {
            try
            {
                _logger.LogInformation("Publishing contact updated");
                
                var queueUri = new Uri($"{_rabbitMQSettings.ConnectionString}/{_rabbitMQSettings.Queues.ContactUpdate}");
                var endpoint = await _bus.GetSendEndpoint(queueUri);
                
                await endpoint.Send(contact);

                _logger.LogInformation("Contact published to update queue");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing contact updated for contact");
                throw;
            }
        }

        public async Task PublishContactDeleted(Guid contactId)
        {
            try
            {
                _logger.LogInformation("Publishing contact deleted");
                
                var queueUri = new Uri($"{_rabbitMQSettings.ConnectionString}/{_rabbitMQSettings.Queues.ContactDelete}");
                var endpoint = await _bus.GetSendEndpoint(queueUri);
                
                await endpoint.Send<ContactDeletedMessage>(new 
                { 
                    Id = contactId,
                    DeletedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing contact deleted for contact {ContactId}", contactId);
                throw;
            }
        }
    }
    
    // Simple message contract for the delete event
    public interface ContactDeletedMessage
    {
        Guid Id { get; }
        DateTime DeletedAt { get; }
    }
}