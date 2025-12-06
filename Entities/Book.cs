using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace LibraryManagementAPI.Entities;

[Index(nameof(Title), Name = "IX_Book_Title")]
public class Book
{
    public Guid Id { get; set; }
    public required string ISBN { get; set; }
    public required string Title { get; set; }
    public string? ImgUrl { get; set; }
    public Guid? PublisherId { get; set; }
    public Publisher? Publisher { get; set; }
    public int PublicationYear { get; set; }
    public string? Description { get; set; }
    public ICollection<BookCategory> BookCategories { get; set; } = [];
    public ICollection<Author> Authors { get; set; } = [];
}