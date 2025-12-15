using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IServices
{
    public interface IRecommendationService
    {
        Task<Response<List<BookDto>>> GetRecommendedBooksForUser(Guid? memberId, int pageNumber = 1, int pageSize = 20, float alpha = 0.6f);
    }
}
