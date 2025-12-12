using LibraryManagementAPI.Models.Pagination;

public interface IPublisherService
{
    Task<PagedResponse<PublisherDTO>> GetAllPublishersAsync(int pageNumber, int pageSize);
    Task<PublisherDTO> GetPublisherByIdAsync(Guid id);
    Task<PublisherDTO> AddPublisherAsync(CreatePublisherDTO publisherDto);
    Task UpdatePublisherAsync(Guid id, UpdatePublisherDTO publisherDto);
    Task DeletePublisherAsync(Guid id);
    Task<PagedResponse<PublisherDTO>> SearchPublishersAsync(string searchTerm, int pageNumber, int pageSize);
}