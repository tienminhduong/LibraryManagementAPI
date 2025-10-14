using API.Entities;

namespace API.Interfaces;

public interface IBookCategoryRepository
{
    Task<IEnumerable<BookCategory>> GetAllCategories();
    Task<BookCategory?> GetCategoryById(Guid id);
    Task<bool> AddCategory(BookCategory category);
    Task<bool> UpdateCategory(BookCategory category);
    Task<bool> DeleteCategory(Guid id);
    Task<bool> IsCategoryExistsByName(string name);
}