namespace LibraryManagementAPI.Models.Author;

public class AuthorDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int YearOfBirth { get; set; }
    public required string BriefDescription { get; set; }
}