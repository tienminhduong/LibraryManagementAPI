using AutoMapper;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.BookCategory;

namespace LibraryManagementAPI.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // BookCategory mappings
        CreateMap<BookCategory, BookCategoryDto>();
        CreateMap<CreateBookCategoryDto, BookCategory>();
        CreateMap<BookCategoryDto, BookCategory>();

        // Book mappings
        CreateMap<Book, BookDto>();
        CreateMap<CreateBookDto, Book>();
    }
}