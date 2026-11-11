namespace Velo.Core;

/// <summary>
/// Оформление аренды. Вынесено из обработчика кнопки окна WinRentBicycle:
/// в курсовом проекте расчёт, проверки и обращение к базе данных лежали
/// вперемешку с вызовами MessageBox, поэтому протестировать их было нельзя.
/// </summary>
public class RentalService
{
    private readonly IRentalRepository _repository;

    public RentalService(IRentalRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// Стоимость аренды: цена за час, умноженная на длительность в часах.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">цена не положительна</exception>
    /// <exception cref="ArgumentException">окончание не позже начала</exception>
    public static decimal CalculateTotal(decimal pricePerHour, DateTime start, DateTime end)
    {
        if (pricePerHour <= 0)
            throw new ArgumentOutOfRangeException(nameof(pricePerHour),
                "Цена за час должна быть положительной");

        if (end <= start)
            throw new ArgumentException(
                "Дата и время окончания должны быть позже начала", nameof(end));

        var hours = (decimal)(end - start).TotalHours;
        return Math.Round(pricePerHour * hours, 2);
    }

    /// <summary>
    /// Оформляет заказ на аренду и помечает велосипед занятым.
    /// </summary>
    /// <exception cref="ArgumentException">окончание не позже начала</exception>
    /// <exception cref="InvalidOperationException">
    /// аренда в прошлом, велосипед не найден, недоступен либо уже занят
    /// на пересекающийся период
    /// </exception>
    public RentalOrder CreateOrder(int userId, int bicycleId,
                                   DateTime start, DateTime end, DateTime now)
    {
        if (end <= start)
            throw new ArgumentException(
                "Дата и время окончания должны быть позже начала", nameof(end));

        if (start < now)
            throw new InvalidOperationException(
                "Нельзя арендовать велосипед на прошедшее время");

        var bicycle = _repository.GetBicycle(bicycleId)
            ?? throw new InvalidOperationException(
                $"Велосипед с идентификатором {bicycleId} не найден");

        if (!bicycle.IsAvailable)
            throw new InvalidOperationException(
                "Выбранный велосипед недоступен для аренды");

        if (HasOverlap(bicycleId, start, end))
            throw new InvalidOperationException(
                "Велосипед уже арендован на выбранный период");

        var order = _repository.AddOrder(new RentalOrder
        {
            UserId = userId,
            BicycleId = bicycleId,
            StartDate = start,
            EndDate = end,
            Status = OrderStatus.Created,
            TotalPrice = CalculateTotal(bicycle.PricePerHour, start, end),
        });

        // В курсовом проекте этот шаг терялся: признак снимался у объекта,
        // который не отслеживался контекстом, и до базы не доходил.
        _repository.SetAvailability(bicycleId, false);

        return order;
    }

    /// <summary>Пересекается ли период с уже существующими заказами.</summary>
    public bool HasOverlap(int bicycleId, DateTime start, DateTime end)
    {
        return _repository.GetActiveOrders(bicycleId)
            .Any(o => o.StartDate < end && o.EndDate > start);
    }
}
