using AutoMapper;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Pagination;

public class PublisherService(IPublisherRepository publisherRepository,
                              IMapper mapper) : IPublisherService
{
  public async Task<PublisherDTO> AddPublisherAsync(CreatePublisherDTO publisherDto)
  {
    // validate input
    if (publisherDto == null)
    {
      throw new ArgumentNullException(nameof(publisherDto), "Publisher data is null");
    }
    // map dto to entity and save it to repository
    var publisher = mapper.Map<Publisher>(publisherDto);
    await publisherRepository.AddPublisherAsync(publisher);
    return mapper.Map<PublisherDTO>(publisher);
  }

  public async Task DeletePublisherAsync(Guid id)
  {
    if (id == Guid.Empty)
    {
      throw new ArgumentException("Invalid publisher ID", nameof(id));
    }
    await publisherRepository.DeletePublisherAsync(id);
  }

  public async Task<PagedResponse<PublisherDTO>> GetAllPublishersAsync(int pageNumber, int pageSize)
  {
    var pagedPublishers = await publisherRepository.GetAllPublishersAsync(pageNumber, pageSize);
    var publisherDtos = PagedResponse<PublisherDTO>.MapFrom(pagedPublishers, mapper);
    return publisherDtos;
  }

  public async Task<PublisherDTO> GetPublisherByIdAsync(Guid id)
  {
    if (id == Guid.Empty)
    {
      throw new ArgumentException("Invalid publisher ID", nameof(id));
    }
    var publisher = await publisherRepository.GetPublisherByIdAsync(id);
    return mapper.Map<PublisherDTO>(publisher);
  }

  public Task UpdatePublisherAsync(Guid id, UpdatePublisherDTO publisherDto)
  {
    // validate id
    if (id == Guid.Empty)
    {
      throw new ArgumentException("Invalid publisher ID", nameof(id));
    }
    // validate input
    if (publisherDto == null)
    {
      throw new ArgumentNullException(nameof(publisherDto), "Publisher data is null");
    }
    // map dto to entity and update it in repository
    var publisher = mapper.Map<Publisher>(publisherDto);
    publisher.Id = id;
    return publisherRepository.UpdatePublisherAsync(publisher);
  }
}