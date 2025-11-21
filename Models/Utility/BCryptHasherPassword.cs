using LibraryManagementAPI.Interfaces.IUtility;

namespace LibraryManagementAPI.Models.Utility
{
    public class BCryptHasherPassword : IHasherPassword
    {
        const int WORK_FACTOR = 12; // You can adjust the work factor as needed
        public string HashPassword(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, WORK_FACTOR);
            return hashedPassword;
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            bool isValid = BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
            return isValid;
        }
    }
}
