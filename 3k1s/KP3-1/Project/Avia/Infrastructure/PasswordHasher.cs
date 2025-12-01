using System.Security.Cryptography;
using System.Text;

namespace Avia.Infrastructure;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    public static bool VerifyPassword(string enteredPassword, string hashedPassword)
    {
        string hashedEnteredPassword = HashPassword(enteredPassword);
        return string.Equals(hashedEnteredPassword, hashedPassword, StringComparison.Ordinal);
    }
}

