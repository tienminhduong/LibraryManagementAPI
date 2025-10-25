using AutoMapper;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Exceptions;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.BookCategory;
using LibraryManagementAPI.Models.Pagination;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services;

public class BookService(
    IBookCategoryRepository bookCategoryRepository,
    IBookRepository bookRepository,
    IMapper mapper) : IBookService
{
    public async Task<BookDto> AddBookAsync(CreateBookDto bookDto)
    {
        ArgumentNullException.ThrowIfNull(bookDto);

        var book = mapper.Map<Book>(bookDto);
        book.BookCategories = [.. await bookCategoryRepository.IdListToEntity(bookDto.CategoryIds)];

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

    public async Task<IEnumerable<BookCategoryDto>> GetAllBookCategoriesAsync()
    {
        var categories = await bookCategoryRepository.GetAllCategories();
        return mapper.Map<IEnumerable<BookCategoryDto>>(categories);
    }

    public async Task<PagedResponse<BookDto>> GetAllBooksAsync(int pageNumber = 1, int pageSize = 20)
    {
        var books = await bookRepository.GetAllBooksAsync(pageNumber, pageSize);
        var bookDtos = new PagedResponse<BookDto>(
            pageNumber,
            pageSize,
            mapper.Map<IEnumerable<BookDto>>(books.Data),
            books.TotalItems);

        return bookDtos;
    }

    public async Task<PagedResponse<BookDto>> GetAllBooksInCategoryAsync(Guid id, int pageNumber = 1, int pageSize = 20)
    {
        var books = await bookCategoryRepository.SearchBookByCategory(id, pageNumber, pageSize);
        var bookDtos = new PagedResponse<BookDto>(
            pageNumber,
            pageSize,
            mapper.Map<IEnumerable<BookDto>>(books.Data),
            books.TotalItems
        );
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
}