namespace API.Models;

public class BookCategoryDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}

public class CreateBookCategoryDto
{
    public required string Name { get; set; }
}

public class UpdateBookCategoryDto
{
    public required string Name { get; set; }
}