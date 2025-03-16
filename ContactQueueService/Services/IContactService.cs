
using ContactQueueService.Dto;

namespace ContactQueueService.Services
{
    public interface IContactService
    {
        Task<ContactDto> CreateContactAsync(ContactDto contactDto);
        Task<ContactDto> UpdateContactAsync(Guid id, ContactDto contactDto);
        Task DeleteContactAsync(Guid id);
    }
}