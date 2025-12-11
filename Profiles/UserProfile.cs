using AutoMapper;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Author;
using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.BookCategory;
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

        // Supplier mappings
        CreateMap<Supplier, SupplierDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.address))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.phoneNumber))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email));
        
        CreateMap<CreateSupplierDTO, Supplier>()
            .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.phoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.id, opt => opt.Ignore())
            .ForMember(dest => dest.bookImports, opt => opt.Ignore());
        
        CreateMap<UpdateSupplierDTO, Supplier>()
            .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.phoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.id, opt => opt.Ignore())
            .ForMember(dest => dest.bookImports, opt => opt.Ignore());
    }
}