namespace LibraryManagementAPI.Interfaces.IUtility
{
    public interface IHasherPassword
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }
}
