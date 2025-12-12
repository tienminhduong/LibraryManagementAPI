using AutoMapper;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Author;
using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.BookCategory;
using LibraryManagementAPI.Models.Info;
using LibraryManagementAPI.Models.Supplier;

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

        // Author mappings
        CreateMap<Author, AuthorDto>();
        CreateMap<CreateAuthorDto, Author>();
        CreateMap<UpdateAuthorDto, Author>();

        // Publisher mappings
        CreateMap<Publisher, PublisherDTO>();
        CreateMap<CreatePublisherDTO, Publisher>();
        CreateMap<UpdatePublisherDTO, Publisher>();
        
        CreateMap<Supplier, SupplierDto>();
        
        CreateMap<StaffInfo, StaffInfoDto>();
        
        CreateMap<BookImport, BookImportDto>();
        CreateMap<BookImport, DetailBookImportDto>();
        CreateMap<BookImportDetail, SimpleBookImportDetailsDto>()
            .ForMember(dest => dest.BookTitle,
                opt => opt.MapFrom(src => src.book != null ? src.book.Title : string.Empty))
            .ForMember(dest => dest.BookISBN,
                opt => opt.MapFrom(src => src.book != null ? src.book.ISBN : string.Empty));
        CreateMap<BookImportDetail, BookImportDetailsDto>();
    }
}