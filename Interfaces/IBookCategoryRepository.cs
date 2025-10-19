using API.Entities;

namespace API.Interfaces;

public interface IBookCategoryRepository
{
    Task<IEnumerable<BookCategory>> GetAllCategories();

    
}