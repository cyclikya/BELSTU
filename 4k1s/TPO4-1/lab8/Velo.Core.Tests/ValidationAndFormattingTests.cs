using Velo.Core;

namespace Velo.Core.Tests;

/// <summary>
/// Тесты чистых функций: проверка данных регистрации, хеширование паролей
/// и перевод рейтинга в звёзды. Внешних зависимостей у них нет, поэтому
/// заглушки здесь не нужны.
/// </summary>
public class ValidationAndFormattingTests
{
    // ─────────────── 11. проверка данных регистрации ───────────────

    [Theory]
    // корректные данные — ошибки нет
    [InlineData("violetta", "1234", "1234", "Виолетта", "Угоренко", "+375291112233", null)]
    // пустые обязательные поля
    [InlineData("", "1234", "1234", "Виолетта", "Угоренко", "+375291112233",
        "Все поля обязательны для заполнения")]
    [InlineData("violetta", "1234", "1234", "", "Угоренко", "+375291112233",
        "Все поля обязательны для заполнения")]
    [InlineData("violetta", "1234", "1234", "Виолетта", "Угоренко", "",
        "Все поля обязательны для заполнения")]
    // пароли не совпадают
    [InlineData("violetta", "1234", "4321", "Виолетта", "Угоренко", "+375291112233",
        "Пароли не совпадают")]
    // пароль короче минимального
    [InlineData("violetta", "123", "123", "Виолетта", "Угоренко", "+375291112233",
        "Пароль должен содержать минимум 4 символа")]
    // телефон не в белорусском формате
    [InlineData("violetta", "1234", "1234", "Виолетта", "Угоренко", "+79991234567",
        "Введите корректный белорусский номер в формате +375291234567")]
    public void Validate_ReturnsExpectedMessage(
        string login, string password, string confirm,
        string firstName, string lastName, string phone, string? expected)
    {
        // Act
        var actual = RegistrationValidator.Validate(
            login, password, confirm, firstName, lastName, phone);

        // Assert
        Assert.Equal(expected, actual);
    }

    // ─────────────── 12. формат телефона ───────────────

    [Theory]
    [InlineData("+375291112233", true)]   // код 29
    [InlineData("+375251112233", true)]   // код 25
    [InlineData("+375331112233", true)]   // код 33
    [InlineData("+375441112233", true)]   // код 44
    [InlineData("+375211112233", false)]  // несуществующий код оператора
    [InlineData("+79991234567", false)]   // другая страна
    [InlineData("375291112233", false)]   // без плюса
    [InlineData("+37529111223", false)]   // на цифру меньше
    [InlineData("+3752911122334", false)] // на цифру больше
    [InlineData("abc", false)]            // не цифры
    [InlineData(null, false)]             // значение не задано
    public void IsValidPhone_ChecksBelarusianFormat(string? phone, bool expected)
    {
        Assert.Equal(expected, RegistrationValidator.IsValidPhone(phone));
    }

    // ─────────────── 13. сверка пароля с сохранённым хешем ───────────────

    [Theory]
    [InlineData("1234", "1234", true)]    // верный пароль
    [InlineData("1234", "4321", false)]   // неверный пароль
    [InlineData("1234", "1235", false)]   // отличается одним символом
    [InlineData("1234", "", false)]       // пустой ввод
    [InlineData("1234", "1234 ", false)]  // лишний пробел
    public void VerifyPassword_ComparesWithStoredHash(
        string stored, string entered, bool expected)
    {
        // Arrange: в базе хранится не пароль, а его хеш
        var hash = PasswordHasher.HashPassword(stored);

        // Act
        var actual = PasswordHasher.VerifyPassword(entered, hash);

        // Assert
        Assert.Equal(expected, actual);
        // хеширование детерминировано: тот же пароль даёт тот же хеш
        Assert.Equal(hash, PasswordHasher.HashPassword(stored));
    }

    // ─────────────── 14. перевод рейтинга в звёзды ───────────────

    [Theory]
    [InlineData(null, "")]                 // оценок нет
    [InlineData(5.0, "★★★★★")]
    [InlineData(4.0, "★★★★✩")]
    [InlineData(4.5, "★★★★☆")]             // ровно половина
    [InlineData(4.7, "★★★★☆")]             // округляется вниз до половины
    [InlineData(3.2, "★★★✩✩")]             // меньше половины отбрасывается
    [InlineData(0.0, "✩✩✩✩✩")]
    public void ToStars_FormatsRating(double? rating, string expected)
    {
        Assert.Equal(expected, RatingFormatter.ToStars(rating));
    }

    // ─────────────── 15. рейтинг вне допустимого диапазона ───────────────

    [Theory]
    [InlineData(-1.0)]
    [InlineData(5.5)]
    [InlineData(100.0)]
    public void ToStars_RatingOutOfRange_ThrowsArgumentOutOfRange(double rating)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => RatingFormatter.ToStars(rating));

        Assert.Equal("rating", exception.ParamName);
    }
}
