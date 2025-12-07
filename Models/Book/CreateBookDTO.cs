using LibraryManagementAPI.Models.Utility;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Models.Book;

public class CreateBookDto
{
    public required string ISBN { get; set; }
    public required string Title { get; set; }
    public string? ImgUrl { get; set; }
    public int PublicationYear { get; set; }
    public string? Description { get; set; }
    [FromForm]
    [ModelBinder(typeof(GuidListModelBinder))]
    public IEnumerable<Guid> CategoryIds { get; set; } = [];
    [FromForm]
    [ModelBinder(typeof(GuidListModelBinder))]
    public IEnumerable<Guid> AuthorIds { get; set; } = [];
    public IFormFile? Image { get; set; }
}
