using System;

namespace ContactQueueService.MessageBroker.Messages
{
    public interface ContactCreatedEvent
    {
        Guid Id { get; }
        string FirstName { get; }
        string LastName { get; }
        string Email { get; }
        string PhoneNumber { get; }
        string Address { get; }
        DateTime CreatedAt { get; }
    }

    public interface ContactUpdatedEvent
    {
        Guid Id { get; }
        string FirstName { get; }
        string LastName { get; }
        string Email { get; }
        string PhoneNumber { get; }
        string Address { get; }
        DateTime UpdatedAt { get; }
    }

    public interface ContactDeletedEvent
    {
        Guid Id { get; }
        DateTime DeletedAt { get; }
    }
}