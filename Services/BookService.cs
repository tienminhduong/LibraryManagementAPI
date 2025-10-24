using API.Entities;
using API.Interfaces;
using API.Models;
using AutoMapper;
using LibraryManagementAPI.Exceptions;
using LibraryManagementAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace API.Services;

public class BookService(
    IBookCategoryRepository bookCategoryRepository,
    IBookRepository bookRepository,
    IMapper mapper) : IBookService
{
    public async Task<BookDTO> AddBookAsync(CreateBookDTO bookDto)
    {
        ArgumentNullException.ThrowIfNull(bookDto);

        var book = mapper.Map<Book>(bookDto);
        var result = await bookRepository.AddBookAsync(book);

        var createdBook = await bookRepository.GetBookByIdAsync(book.Id);

        if (!result)
            throw new DbUpdateException("Saving failed");

        return mapper.Map<BookDTO>(createdBook);
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

    public async Task<PagedResponse<BookDTO>> GetAllBooksAsync(int pageNumber = 1, int pageSize = 20)
    {
        var books = await bookRepository.GetAllBooksAsync(pageNumber, pageSize);
        var bookDtos = new PagedResponse<BookDTO>(
            pageNumber,
            pageSize,
            mapper.Map<IEnumerable<BookDTO>>(books.Data),
            books.TotalItems);

        return bookDtos;
    }

    public Task<PagedResponse<BookDTO>> GetAllBooksInCategoryAsync(string categoryName, int pageNumber = 1, int pageSize = 20)
    {
        throw new NotImplementedException();
    }

    public async Task<BookDTO?> GetBookByIdAsync(Guid id)
    {
        var book = await bookRepository.GetBookByIdAsync(id);
        return mapper.Map<BookDTO>(book);
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
}