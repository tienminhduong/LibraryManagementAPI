using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Pagination;
using Microsoft.EntityFrameworkCore;

public class PublisherRepository(LibraryDbContext dbContext) : IPublisherRepository
{
  // Repository implementation
  public async Task AddPublisherAsync(Publisher publisher)
  {
    await dbContext.Publishers.AddAsync(publisher);
    await dbContext.SaveChangesAsync();
  }

  public async Task DeletePublisherAsync(Guid id)
  {
    // find publisher by id
    var publisher = await dbContext.Publishers.FindAsync(id);
    // if not found, throw exception
    if (publisher == null)
    {
      throw new ArgumentNullException(nameof(publisher), "Publisher not found");
    }
    // remove publisher
    dbContext.Publishers.Remove(publisher);
    await dbContext.SaveChangesAsync();
  }

  public async Task<PagedResponse<Publisher>> GetAllPublishersAsync(int pageNumber, int pageSize)
  {
    var publishers = dbContext.Publishers
                              .AsQueryable()
                              .AsNoTracking();
    return await PagedResponse<Publisher>.FromQueryable(publishers, pageNumber, pageSize);
  }

  public async Task<Publisher?> GetPublisherByIdAsync(Guid id)
  {
    // find publisher by id
    var publisher = await dbContext.Publishers.FindAsync(id);
    return publisher;
  }

  public async Task UpdatePublisherAsync(Publisher publisher)
  {
    // check if publisher exists
    var existingPublisher = await dbContext.Publishers.AsNoTracking()
                                                      .FirstOrDefaultAsync(p => p.Id == publisher.Id);
    // if not, throw exception else update
    if (existingPublisher == null)
    {
      throw new ArgumentNullException(nameof(publisher), "Publisher not found");
    }
    dbContext.Publishers.Update(publisher);
    await dbContext.SaveChangesAsync();
  }
}