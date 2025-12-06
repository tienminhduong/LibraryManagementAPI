using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace LibraryManagementAPI.Entities;

[Index(nameof(Name), Name = "IX_Author_Name")]
public class Author
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int YearOfBirth { get; set; }
    public string? BriefDescription { get; set; }
    public ICollection<Book> Books { get; set; } = [];
}