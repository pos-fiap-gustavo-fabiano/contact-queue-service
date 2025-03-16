using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContactQueueService.MessageBroker;
using ContactQueueService.Dto;
using Microsoft.Extensions.Logging;

namespace ContactQueueService.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactBus _contactBus;
        private readonly ILogger<ContactService> _logger;

        public ContactService(IContactBus contactBus, ILogger<ContactService> logger)
        {
            _contactBus = contactBus ?? throw new ArgumentNullException(nameof(contactBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ContactDto> CreateContactAsync(ContactDto contactDto)
        {
            await _contactBus.PublishContactCreated(contactDto);
            return contactDto;
        }

        public async Task<ContactDto> UpdateContactAsync(Guid id, ContactDto contactDto)
        {
            await _contactBus.PublishContactUpdated(contactDto);
            return contactDto;
        }

        public async Task DeleteContactAsync(Guid id)
        {
            await _contactBus.PublishContactDeleted(id);
            _logger.LogInformation("Contact deleted: {ContactId}", id);
        }
    }
}