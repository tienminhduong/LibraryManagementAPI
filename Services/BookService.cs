using System.Net.Quic;
using AutoMapper;
using CloudinaryDotNet.Actions;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Exceptions;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.BookCategory;
using LibraryManagementAPI.Models.Pagination;
using LibraryManagementAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LibraryManagementAPI.Services;

public class BookService(
    IBookCategoryRepository bookCategoryRepository,
    IBookRepository bookRepository,
    IBookImportRepository bookImportRepository,
    IBookCopyRepository bookCopyRepository,
    IAuthorRepository authorRepository,
    IMapper mapper,
    IMemoryCache _cache,
    ILogger _logger) : IBookService
{
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);
    public async Task<BookDto> AddBookAsync(CreateBookDto bookDto)
    {
        ArgumentNullException.ThrowIfNull(bookDto);

        var book = mapper.Map<Book>(bookDto);
        book.BookCategories = [.. await bookCategoryRepository.IdListToEntity(bookDto.CategoryIds)];
        book.Authors = [.. await authorRepository.IdListToEntity(bookDto.AuthorIds)];

        var result = await bookRepository.AddBookAsync(book);

        var createdBook = await bookRepository.GetBookByIdAsync(book.Id);

        if (!result)
            throw new DbUpdateException("Saving failed");

        return mapper.Map<BookDto>(createdBook);
    }

    public async Task<BookCategoryDto> CreateBookCategoryAsync(CreateBookCategoryDto categoryDto)
    {
        var isExisted = await bookCategoryRepository.IsCategoryExistsByName(categoryDto.Name);
        if (isExisted)
            throw new ExistedException(nameof(categoryDto.Name), categoryDto.Name);

        var bookCategory = mapper.Map<BookCategory>(categoryDto);
        await bookCategoryRepository.AddCategoryAsync(bookCategory);

        return mapper.Map<BookCategoryDto>(bookCategory);
    }

    public async Task DeleteBookCategoryAsync(Guid id)
    {
        var existingCategory = await bookCategoryRepository.GetCategoryByIdAsync(id)
            ?? throw new NotFoundException(nameof(BookCategory), id);

        int count = await bookCategoryRepository.CountBooksByCategory(id);
        if (count > 0)
            throw new InvalidOperationException("Cannot delete categories with books existed");

        await bookCategoryRepository.DeleteCategory(id);
    }

    public async Task<PagedResponse<BookCategoryDto>> GetAllBookCategoriesAsync(int pageNumber = 1, int pageSize = 20)
    {
        var categories = await bookCategoryRepository.GetAllCategories(pageNumber, pageSize);
        return PagedResponse<BookCategoryDto>.MapFrom(categories, mapper);
    }

    public async Task<PagedResponse<BookDto>> GetAllBooksAsync(Guid? categoryId, int pageNumber = 1, int pageSize = 20)
    {
        var books = await bookRepository.GetAllBooksAsync(categoryId, pageNumber, pageSize);
        var bookDtos = PagedResponse<BookDto>.MapFrom(books, mapper);
        await PopulateAvailableCopiesCountAsync(bookDtos.Data);
        return bookDtos;
    }

    public async Task<PagedResponse<BookDto>> GetAllBooksInCategoryAsync(Guid id, int pageNumber = 1, int pageSize = 20)
    {
        var books = await bookCategoryRepository.SearchBookByCategory(id, pageNumber, pageSize);
        var bookDtos = PagedResponse<BookDto>.MapFrom(books, mapper);
        await PopulateAvailableCopiesCountAsync(bookDtos.Data);
        return bookDtos;
    }

    public async Task<BookDto?> GetBookByIdAsync(Guid id)
    {
        var book = await bookRepository.GetBookByIdAsync(id);
        if (book == null)
            return null;

        var bookDto = mapper.Map<BookDto>(book);
        bookDto.AvailableCopiesCount = await bookCopyRepository.GetAvailableCopiesCountByBookId(id);
        return bookDto;
    }

    public async Task<BookCategoryDto> GetBookCategoryByIdAsync(Guid id)
    {
        var catogory = await bookCategoryRepository.GetCategoryByIdAsync(id);
        return mapper.Map<BookCategoryDto>(catogory);
    }

    public async Task UpdateAuthorOfBookAsync(Guid id, UpdateAuthorOfBookDto dto)
    {
        var book = await bookRepository.GetBookByIdAsync(id)
            ?? throw new NotFoundException(nameof(Book), id);

        var authors = await authorRepository.IdListToEntity(dto.AuthorIds);
        await bookRepository.UpdateAuthorOfBookAsync(book, authors);
    }

    public async Task UpdateBookCategoryAsync(Guid id, UpdateBookCategoryDto categoryDto)
    {
        var existingCategory = await bookCategoryRepository.GetCategoryByIdAsync(id)
            ?? throw new NotFoundException(nameof(BookCategory), id);

        existingCategory.Name = categoryDto.Name;
        await bookCategoryRepository.UpdateCategory(existingCategory);
    }

    public async Task UpdateCategoryOfBookAsync(Guid id, UpdateCategoryOfBookDto dto)
    {
        var book = await bookRepository.GetBookByIdAsync(id)
            ?? throw new NotFoundException(nameof(Book), id);

        var categories = await bookCategoryRepository.IdListToEntity(dto.CategoryIds);
        await bookRepository.UpdateCategoryOfBookAsync(book, categories);
    }

    public async Task<PagedResponse<BookDto>> SearchByTitleAsync(string title, int pageNumber = 1, int pageSize = 20)
    {
        var cacheKey = $"books_title_{title.ToLower()}_{pageNumber}_{pageSize}";

        if (_cache.TryGetValue(cacheKey, out PagedResponse<BookDto>? cachedResult))
        {
            _logger.LogInformation("Cache hit for title search: {Title}", title);
            return cachedResult!;
        }

        var books = await bookRepository.SearchByTitleAsync(title, pageNumber, pageSize);
        var result = PagedResponse<BookDto>.MapFrom(books, mapper);
        await PopulateAvailableCopiesCountAsync(result.Data);

        _cache.Set(cacheKey, result, _cacheDuration);

        return result;
    }

    public async Task<PagedResponse<BookDto>> SearchByAuthorAsync(string author, int pageNumber = 1, int pageSize = 20)
    {
        var cacheKey = $"books_author_{author.ToLower()}_{pageNumber}_{pageSize}";

        if (_cache.TryGetValue(cacheKey, out PagedResponse<BookDto>? cachedResult))
        {
            _logger.LogInformation("Cache hit for author search: {Author}", author);
            return cachedResult!;
        }

        var books = await bookRepository.SearchByAuthorAsync(author, pageNumber, pageSize);
        var result = PagedResponse<BookDto>.MapFrom(books, mapper);
        await PopulateAvailableCopiesCountAsync(result.Data);

        _cache.Set(cacheKey, result, _cacheDuration);

        return result;
    }

    public async Task<Guid> ImportBooks(CreateBookImportDto createBookImportDto, Guid staffId)
    {
        BookImport bookImport = new()
        {
            staffId = staffId,
            supplierId = createBookImportDto.SupplierId,
            importDate = DateTime.UtcNow,
            note = createBookImportDto.Notes,
            totalAmount = createBookImportDto.Details.Sum(detail => detail.Quantity)
        };

        await bookImportRepository.AddBookImportAsync(bookImport);

        foreach (var detail in createBookImportDto.Details)
        {
            var bookImportDetail = new BookImportDetail
            {
                bookId = detail.BookId,
                quantity = detail.Quantity,
                unitPrice = detail.UnitPrice,
                bookImportId = bookImport.id
            };
            await bookImportRepository.AddImportDetailAsync(bookImportDetail);

            for (int i = 0; i < detail.Quantity; ++i)
            {
                var bookCopy = new BookCopy
                {
                    bookId = detail.BookId,
                    bookImportDetailId = bookImportDetail.id,
                    status = Status.Available,
                };

                await bookCopyRepository.Add(bookCopy);
            }
        }
        return bookImport.id;
    }

    public async Task<PagedResponse<BookImportDto>> GetImportHistoryAsync(int pageNumber = 1, int pageSize = 20)
    {
        var history = await bookImportRepository.GetImportHistoryAsync(pageNumber, pageSize);
        return PagedResponse<BookImportDto>.MapFrom(history, mapper);
    }

    public async Task<DetailBookImportDto?> GetImportHistoryByIdAsync(Guid id)
    {
        var bookImport = await bookImportRepository.GetByIdAsync(id);
        return mapper.Map<DetailBookImportDto?>(bookImport);
    }
    
    public async Task<IEnumerable<BookCopy>> GetCopiesByBookIdAsync(Guid bookId)
    {
        return await bookCopyRepository.GetCopiesByBookId(bookId);
    }

    public async Task<PagedResponse<BookCategoryDto>> SearchBookCategories(string? query = null, int pageNumber = 1, int pageSize = 20)
    {
        var categories = await bookCategoryRepository.GetBookCategoriesByName(query, pageNumber, pageSize);
        return PagedResponse<BookCategoryDto>.MapFrom(categories, mapper);
    }

    public async Task<PagedResponse<BookDto>> SearchBooks(string? isbn = null, string? titleQuery = null, string? categoryName = null, string? authorName = null, string? publisherName = null, int? publishedYear = null, string? descriptionContains = null, int pageNumber = 1, int pageSize = 20)
    {
        var books = await bookRepository.SearchBooks(isbn, titleQuery, categoryName, authorName, publisherName, publishedYear, descriptionContains, pageNumber, pageSize);
        var result = PagedResponse<BookDto>.MapFrom(books, mapper);
        await PopulateAvailableCopiesCountAsync(result.Data);
        return result;
    }

    /// <summary>
    /// Helper method to populate AvailableCopiesCount for a collection of BookDtos
    /// </summary>
    private async Task PopulateAvailableCopiesCountAsync(IEnumerable<BookDto> bookDtos)
    {
        foreach (var bookDto in bookDtos)
        {
            bookDto.AvailableCopiesCount = await bookCopyRepository.GetAvailableCopiesCountByBookId(bookDto.Id);
        }
    }

    public async Task<Response<PagedResponse<CategoryBorrowStatDto>>> GetTopCategoryByTimeAsync(int pageNumber = 1, int pageSize = 20, DateTime? from = null, DateTime? to = null)
    {
        var categories = await bookCategoryRepository.GetTopCategoryByTime(pageNumber, pageSize, from, to);
        if (categories == null)
            return Response<PagedResponse<CategoryBorrowStatDto>>.Failure("Error");
        return Response<PagedResponse<CategoryBorrowStatDto>>.Success(categories);
    }

    public async Task<Response<PagedResponse<BookBorrowStatDto>>> GetTopBookByTimeAsync( DateTime? from = null, DateTime? to = null, int pageNumber = 1, int pageSize = 20)
    {
        var books = await bookRepository.GetTopBooks( from, to, pageNumber, pageSize);
        if (books == null)
            return Response<PagedResponse<BookBorrowStatDto>>.Failure("Error");
        return Response<PagedResponse<BookBorrowStatDto>>.Success(books);
    }
}