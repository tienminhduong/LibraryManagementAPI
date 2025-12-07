using AutoMapper;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Exceptions;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Interfaces.IUtility;
using LibraryManagementAPI.Models.Author;
using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.BookCategory;
using LibraryManagementAPI.Models.Pagination;
using LibraryManagementAPI.Models.Utility;
using LibraryManagementAPI.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace LibraryManagementAPI.Services;

public class BookService(
    IBookCategoryRepository bookCategoryRepository,
    IBookRepository bookRepository,
    IAuthorRepository authorRepository,
    IBookCategoryRepository categoryRepository,
    IMapper mapper,
    IMemoryCache _cache,
    IUnitOfWork unitOfWork,
    ILogger _logger) : IBookService
{
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

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

    public async Task<IEnumerable<BookCategoryDto>> GetAllBookCategoriesAsync()
    {
        var categories = await bookCategoryRepository.GetAllCategories();
        return mapper.Map<IEnumerable<BookCategoryDto>>(categories);
    }

    public async Task<PagedResponse<BookDto>> GetAllBooksAsync(Guid? categoryId, int pageNumber = 1, int pageSize = 20)
    {
        var books = await bookRepository.GetAllBooksAsync(categoryId, pageNumber, pageSize);
        var bookDtos = PagedResponse<BookDto>.MapFrom(books, mapper);
        return bookDtos;
    }

    public async Task<PagedResponse<BookDto>> GetAllBooksInCategoryAsync(Guid id, int pageNumber = 1, int pageSize = 20)
    {
        var books = await bookCategoryRepository.SearchBookByCategory(id, pageNumber, pageSize);
        var bookDtos = PagedResponse<BookDto>.MapFrom(books, mapper);
        return bookDtos;
    }

    public async Task<BookDto?> GetBookByIdAsync(Guid id)
    {
        var book = await bookRepository.GetBookByIdAsync(id);
        return mapper.Map<BookDto>(book);
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
            return cachedResult;
        }

        var books = await bookRepository.SearchByTitleAsync(title, pageNumber, pageSize);
        var result = PagedResponse<BookDto>.MapFrom(books, mapper);

        _cache.Set(cacheKey, result, _cacheDuration);

        return result;
    }

    public async Task<PagedResponse<BookDto>> SearchByAuthorAsync(string author, int pageNumber = 1, int pageSize = 20)
    {
        var cacheKey = $"books_author_{author.ToLower()}_{pageNumber}_{pageSize}";

        if (_cache.TryGetValue(cacheKey, out PagedResponse<BookDto>? cachedResult))
        {
            _logger.LogInformation("Cache hit for author search: {Author}", author);
            return cachedResult;
        }

        var books = await bookRepository.SearchByAuthorAsync(author, pageNumber, pageSize);
        var result = PagedResponse<BookDto>.MapFrom(books, mapper);

        _cache.Set(cacheKey, result, _cacheDuration);

        return result;
    }

    public async Task<Response<BookDto>> AddBookAsync(CreateBookDto bookDto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(bookDto);

        if (string.IsNullOrWhiteSpace(bookDto.ISBN))
            throw new ArgumentException("ISBN is required");

        if (bookDto.AuthorIds == null || !bookDto.AuthorIds.Any())
            throw new ArgumentException("Book must have at least one author");

        if (bookDto.CategoryIds == null || !bookDto.CategoryIds.Any())
            throw new ArgumentException("Book must have at least one category");

        // STEP 1: Validate in parallel (lightweight - only check IDs exist)
        var isbnExistsTask = await bookRepository.IsbnExistsAsync(bookDto.ISBN, ct);
        var authorIdsTask = await authorRepository.GetExistingIdsAsync(bookDto.AuthorIds, ct);
        var categoryIdsTask = await categoryRepository.GetExistingIdsAsync(bookDto.CategoryIds, ct);

        //await Task.WhenAll(isbnExistsTask, authorIdsTask, categoryIdsTask);
        // Check validation results
        if ( isbnExistsTask)
            throw new InvalidOperationException($"ISBN '{bookDto.ISBN}' already exists");

        var existingAuthorIds =  authorIdsTask;
        var existingCategoryIds =  categoryIdsTask;

        var invalidAuthorIds = bookDto.AuthorIds.Except(existingAuthorIds).ToList();
        if (invalidAuthorIds.Any())
            throw new ArgumentException($"Invalid author IDs: {string.Join(", ", invalidAuthorIds)}");

        var invalidCategoryIds = bookDto.CategoryIds.Except(existingCategoryIds).ToList();
        if (invalidCategoryIds.Any())
            throw new ArgumentException($"Invalid category IDs: {string.Join(", ", invalidCategoryIds)}");

        try
        {
            await unitOfWork.BeginTransactionAsync();

            // Fetch and validate sequentially
            if (await bookRepository.IsbnExistsAsync(bookDto.ISBN, ct))
                throw new InvalidOperationException($"ISBN '{bookDto.ISBN}' already exists");

            var authors = await authorRepository.IdListToEntity(bookDto.AuthorIds);
            var categories = await categoryRepository.IdListToEntity(bookDto.CategoryIds);

            if (authors.Count() != bookDto.AuthorIds.Distinct().Count())
            {
                var foundIds = authors.Select(a => a.Id).ToHashSet();
                var invalid = bookDto.AuthorIds.Except(foundIds);
                throw new ArgumentException($"Invalid author IDs: {string.Join(", ", invalid)}");
            }

            if (categories.Count() != bookDto.CategoryIds.Distinct().Count())
            {
                var foundIds = categories.Select(c => c.Id).ToHashSet();
                var invalid = bookDto.CategoryIds.Except(foundIds);
                throw new ArgumentException($"Invalid category IDs: {string.Join(", ", invalid)}");
            }

            // Create and save
            var book = mapper.Map<Book>(bookDto);
            book.Authors = (ICollection<Author>)authors;
            book.BookCategories = (ICollection<BookCategory>)categories;

            await bookRepository.AddBookAsync(book);
            await unitOfWork.CommitAsync();

            // Build response
            var result = mapper.Map<BookDto>(book);
            result.Authors = mapper.Map<List<AuthorDto>>(authors);
            result.BookCategories = mapper.Map<List<BookCategoryDto>>(categories);

            return Response<BookDto>.Success(result);
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}