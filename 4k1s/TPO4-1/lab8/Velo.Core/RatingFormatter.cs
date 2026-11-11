namespace Velo.Core;

/// <summary>
/// Перевод среднего рейтинга в строку звёзд. Перенесено из
/// RatingToStarsConverter, чтобы логику можно было проверить без WPF.
/// </summary>
public static class RatingFormatter
{
    public const char Full = '★';
    public const char Half = '☆';
    public const char Empty = '✩';

    /// <summary>Пустая строка, если оценок нет.</summary>
    public static string ToStars(double? rating)
    {
        if (rating is null)
            return string.Empty;

        if (rating < 0 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating),
                "Рейтинг должен быть в диапазоне от 0 до 5");

        var full = (int)Math.Floor(rating.Value);
        var hasHalf = rating.Value - full >= 0.5;
        var empty = 5 - full - (hasHalf ? 1 : 0);

        return new string(Full, full)
             + (hasHalf ? Half.ToString() : string.Empty)
             + new string(Empty, empty);
    }
}
