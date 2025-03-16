using System;
using System.Threading.Tasks;
using ContactQueueService.Configuration;
using ContactQueueService.Dto;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ContactQueueService.MessageBroker
{
    public class ContactBus : IContactBus
    {
        private readonly IBus _bus;
        private readonly ILogger<ContactBus> _logger;
        private readonly RabbitMQSettings _rabbitMQSettings;

        public ContactBus(IBus bus, ILogger<ContactBus> logger, RabbitMQSettings rabbitMQSettings)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _rabbitMQSettings = rabbitMQSettings ?? throw new ArgumentNullException(nameof(rabbitMQSettings));
        }

        public async Task PublishContactCreated(ContactDto contact)
        {
            try
            {
                _logger.LogInformation("Publishing contact created");
                
                var queueUri = new Uri($"{_rabbitMQSettings.ConnectionString}/{_rabbitMQSettings.Queues.ContactCreate}");
                var endpoint = await _bus.GetSendEndpoint(queueUri);
                
                await endpoint.Send(contact);
                
                _logger.LogInformation("Contact published to create queue");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing contact created for contact");
                throw;
            }
        }

        public async Task PublishContactUpdated(UpdateContactDto contact)
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