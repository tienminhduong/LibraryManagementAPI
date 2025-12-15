namespace LibraryManagementAPI.Interfaces.IUtility
{
    public interface IHasherPassword
    {
        string HashPassword(string password);
        bool VerifyPassword(string providedPassword, string hashedPassword);
    }
}
