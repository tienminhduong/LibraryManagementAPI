using API.Entities;
using API.Models;
using AutoMapper;

namespace API.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<BookCategory, BookCategoryDto>();
        CreateMap<CreateBookCategoryDto, BookCategory>();
        CreateMap<BookCategoryDto, BookCategory>();
    }
}