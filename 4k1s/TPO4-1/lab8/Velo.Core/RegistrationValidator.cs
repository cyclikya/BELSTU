using System.Text.RegularExpressions;

namespace Velo.Core;

/// <summary>
/// Проверка данных при регистрации. Правила перенесены из обработчика
/// BtnRegister_Click страницы PgRegister курсового проекта.
/// </summary>
public static class RegistrationValidator
{
    public const int MinPasswordLength = 4;

    private static readonly Regex PhonePattern =
        new(@"^\+375(25|29|33|44)\d{7}$", RegexOptions.Compiled);

    /// <summary>
    /// Возвращает текст ошибки либо null, если данные корректны.
    /// Порядок проверок повторяет порядок в приложении.
    /// </summary>
    public static string? Validate(string? login, string? password, string? confirmPassword,
                                   string? firstName, string? lastName, string? phone)
    {
        if (string.IsNullOrWhiteSpace(login)
            || string.IsNullOrWhiteSpace(password)
            || string.IsNullOrWhiteSpace(firstName)
            || string.IsNullOrWhiteSpace(lastName)
            || string.IsNullOrWhiteSpace(phone))
        {
            return "Все поля обязательны для заполнения";
        }

        if (password != confirmPassword)
            return "Пароли не совпадают";

        if (password.Length < MinPasswordLength)
            return $"Пароль должен содержать минимум {MinPasswordLength} символа";

        if (!IsValidPhone(phone))
            return "Введите корректный белорусский номер в формате +375291234567";

        return null;
    }

    /// <summary>Белорусский номер вида +375 с кодом оператора 25, 29, 33 или 44.</summary>
    public static bool IsValidPhone(string? phone)
    {
        return !string.IsNullOrWhiteSpace(phone) && PhonePattern.IsMatch(phone);
    }
}
