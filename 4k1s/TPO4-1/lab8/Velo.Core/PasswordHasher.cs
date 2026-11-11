using System.Security.Cryptography;
using System.Text;

namespace Velo.Core;

/// <summary>
/// Хеширование паролей. Перенесено из Context.cs курсового проекта
/// без изменения алгоритма.
/// </summary>
public static class PasswordHasher
{
    /// <exception cref="ArgumentNullException">пароль не задан</exception>
    public static string HashPassword(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public static bool VerifyPassword(string enteredPassword, string hashedPassword)
    {
        if (enteredPassword is null || hashedPassword is null)
            return false;

        return string.Equals(HashPassword(enteredPassword), hashedPassword,
                             StringComparison.Ordinal);
    }
}
