using API.Entities;
using API.Models;

namespace API.Interfaces;

public interface IBookCategoryRepository
{
    Task<IEnumerable<BookCategory>> GetAllCategories();
    Task<BookCategory?> GetCategoryByIdAsync(Guid id);
    Task<bool> AddCategoryAsync(BookCategory category);
    Task<bool> UpdateCategory(BookCategory category);
    Task<bool> DeleteCategory(Guid id);
    Task<bool> IsCategoryExistsByName(string name);
    Task<int> CountBooksByCategory(Guid categoryId);
}