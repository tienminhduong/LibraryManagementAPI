using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IRepositories;

public interface IBookCategoryRepository
{
    Task<IEnumerable<BookCategory>> GetAllCategories();
    Task<BookCategory?> GetCategoryByIdAsync(Guid id);
    Task<bool> AddCategoryAsync(BookCategory category);
    Task<bool> UpdateCategory(BookCategory category);
    Task<bool> DeleteCategory(Guid id);
    Task<bool> IsCategoryExistsByName(string name);
    Task<int> CountBooksByCategory(Guid categoryId);
    Task<PagedResponse<Book>> SearchBookByCategory(Guid id, int pageNumber = 1, int pageSize = 20);
    Task<IEnumerable<BookCategory>> IdListToEntity(IEnumerable<Guid> ids);
}