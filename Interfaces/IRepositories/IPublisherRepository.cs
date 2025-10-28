using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Pagination;

public interface IPublisherRepository
{
    Task<PagedResponse<Publisher>> GetAllPublishersAsync(int pageNumber, int pageSize);
    Task<Publisher?> GetPublisherByIdAsync(Guid id);
    Task UpdatePublisherAsync(Publisher publisher);
    Task DeletePublisherAsync(Guid id);
    Task AddPublisherAsync(Publisher publisher);
}