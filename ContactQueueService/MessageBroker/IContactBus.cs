using ContactQueueService.Dto;

namespace ContactQueueService.MessageBroker
{
    public interface IContactBus
    {
        Task PublishContactCreated(ContactDto contact);
        Task PublishContactUpdated(UpdateContactDto contact);
        Task PublishContactDeleted(Guid contactId);
    }
}