using LibraryManagementAPI.Entities;

public interface ITokenService
{
    string GenerateToken(Account account, BaseInfo info);
}