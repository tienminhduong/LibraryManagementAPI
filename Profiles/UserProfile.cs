using API.Entities;
using AutoMapper;
using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.BookCategory;

namespace API.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // BookCategory mappings
        CreateMap<BookCategory, BookCategoryDto>();
        CreateMap<CreateBookCategoryDto, BookCategory>();
        CreateMap<BookCategoryDto, BookCategory>();

        // Book mappings
        CreateMap<Book, BookDTO>();
        CreateMap<CreateBookDTO, Book>();
    }
}