namespace LibraryManagementAPI.Models.BookCategory;

public class BookCategoryDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}

public class CreateBookCategoryDto
{
    //public required string Name { get; set; }
}