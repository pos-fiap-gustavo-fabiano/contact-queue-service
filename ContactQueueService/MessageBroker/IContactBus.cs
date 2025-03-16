using System;
using System.Threading.Tasks;
using ContactQueueService.Dto;

namespace ContactQueueService.MessageBroker
{
    public interface IContactBus
    {
        Task PublishContactCreated(ContactDto contact);
        Task PublishContactUpdated(ContactDto contact);
        Task PublishContactDeleted(Guid contactId);
    }
}