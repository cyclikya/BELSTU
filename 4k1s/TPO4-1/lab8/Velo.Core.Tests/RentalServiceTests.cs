using Velo.Core;

namespace Velo.Core.Tests;

/// <summary>
/// Тесты оформления аренды. Зависимость от базы данных изолирована
/// заглушкой FakeRentalRepository, поэтому тесты не требуют ни PostgreSQL,
/// ни запущенного приложения.
///
/// Каждый тест построен по схеме AAA: Arrange — подготовка данных,
/// Act — вызов проверяемого метода, Assert — сравнение с ожидаемым.
/// </summary>
public class RentalServiceTests
{
    private static readonly DateTime Now = new(2026, 11, 3, 12, 0, 0);

    private static Bicycle SampleBicycle(bool available = true) => new()
    {
        Id = 1,
        Name = "Городской велосипед CityRide",
        Type = "City",
        PricePerHour = 100m,
        IsAvailable = available,
    };

    // ─────────────── расчёт стоимости ───────────────

    [Theory]
    [InlineData(100, 4, 400)]      // ровные часы
    [InlineData(150, 2, 300)]
    [InlineData(100, 1, 100)]      // минимальная аренда
    [InlineData(99.99, 3, 299.97)] // дробная цена
    [InlineData(100, 0.5, 50)]     // неполный час округляется до копеек
    public void CalculateTotal_ReturnsPriceMultipliedByHours(
        decimal pricePerHour, double hours, decimal expected)
    {
        // Arrange
        var start = Now;
        var end = Now.AddHours(hours);

        // Act
        var actual = RentalService.CalculateTotal(pricePerHour, start, end);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void CalculateTotal_NonPositivePrice_ThrowsArgumentOutOfRange(decimal price)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => RentalService.CalculateTotal(price, Now, Now.AddHours(1)));

        Assert.Equal("pricePerHour", exception.ParamName);
    }

    [Fact]
    public void CalculateTotal_EndNotAfterStart_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => RentalService.CalculateTotal(100m, Now, Now));

        Assert.Contains("позже начала", exception.Message);
    }

    // ─────────────── оформление заказа ───────────────

    [Fact]
    public void CreateOrder_ValidPeriod_SavesOrderWithCreatedStatus()
    {
        // Arrange
        var repository = new FakeRentalRepository().WithBicycle(SampleBicycle());
        var service = new RentalService(repository);

        // Act
        var order = service.CreateOrder(
            userId: 7, bicycleId: 1,
            start: Now.AddDays(1), end: Now.AddDays(1).AddHours(4), now: Now);

        // Assert
        var saved = Assert.Single(repository.SavedOrders);
        Assert.Equal(OrderStatus.Created, saved.Status);
        Assert.Equal(7, saved.UserId);
        Assert.Equal(400m, saved.TotalPrice);
        Assert.Equal(saved.Id, order.Id);
    }

    [Fact]
    public void CreateOrder_ValidPeriod_MarksBicycleUnavailable()
    {
        // Этот случай в курсовом проекте работал неверно: признак снимался
        // у объекта, не отслеживаемого контекстом, и до базы не доходил.
        var repository = new FakeRentalRepository().WithBicycle(SampleBicycle());
        var service = new RentalService(repository);

        service.CreateOrder(1, 1, Now.AddDays(1), Now.AddDays(1).AddHours(2), Now);

        var change = Assert.Single(repository.AvailabilityChanges);
        Assert.Equal((1, false), change);
        Assert.False(repository.GetBicycle(1)!.IsAvailable);
    }

    [Fact]
    public void CreateOrder_StartInPast_ThrowsInvalidOperation()
    {
        var repository = new FakeRentalRepository().WithBicycle(SampleBicycle());
        var service = new RentalService(repository);

        var exception = Assert.Throws<InvalidOperationException>(
            () => service.CreateOrder(1, 1, Now.AddDays(-1), Now.AddHours(1), Now));

        Assert.Contains("прошедшее время", exception.Message);
        Assert.Empty(repository.SavedOrders);
    }

    [Fact]
    public void CreateOrder_UnknownBicycle_ThrowsInvalidOperation()
    {
        // заглушка не знает ни одного велосипеда
        var repository = new FakeRentalRepository();
        var service = new RentalService(repository);

        var exception = Assert.Throws<InvalidOperationException>(
            () => service.CreateOrder(1, 999, Now.AddDays(1), Now.AddDays(2), Now));

        Assert.Contains("999", exception.Message);
    }

    [Fact]
    public void CreateOrder_UnavailableBicycle_ThrowsInvalidOperation()
    {
        var repository = new FakeRentalRepository()
            .WithBicycle(SampleBicycle(available: false));
        var service = new RentalService(repository);

        var exception = Assert.Throws<InvalidOperationException>(
            () => service.CreateOrder(1, 1, Now.AddDays(1), Now.AddDays(2), Now));

        Assert.Contains("недоступен", exception.Message);
        Assert.Empty(repository.SavedOrders);
    }

    [Fact]
    public void CreateOrder_OverlappingPeriod_ThrowsInvalidOperation()
    {
        var repository = new FakeRentalRepository()
            .WithBicycle(SampleBicycle())
            .WithActiveOrder(new RentalOrder
            {
                BicycleId = 1,
                StartDate = Now.AddDays(1).AddHours(2),
                EndDate = Now.AddDays(1).AddHours(6),
                Status = OrderStatus.Created,
            });
        var service = new RentalService(repository);

        var exception = Assert.Throws<InvalidOperationException>(
            () => service.CreateOrder(1, 1,
                Now.AddDays(1).AddHours(4), Now.AddDays(1).AddHours(8), Now));

        Assert.Contains("уже арендован", exception.Message);
    }

    [Theory]
    // существующий заказ занимает промежуток 10:00–14:00 следующего дня
    [InlineData(8, 10, false)]   // заканчивается ровно когда начинается занятый
    [InlineData(14, 18, false)]  // начинается ровно когда занятый кончился
    [InlineData(9, 11, true)]    // захватывает начало
    [InlineData(13, 15, true)]   // захватывает конец
    [InlineData(11, 12, true)]   // целиком внутри
    [InlineData(8, 20, true)]    // полностью накрывает
    public void HasOverlap_DetectsIntersectionCorrectly(
        int startHour, int endHour, bool expected)
    {
        var day = Now.Date.AddDays(1);
        var repository = new FakeRentalRepository()
            .WithBicycle(SampleBicycle())
            .WithActiveOrder(new RentalOrder
            {
                BicycleId = 1,
                StartDate = day.AddHours(10),
                EndDate = day.AddHours(14),
                Status = OrderStatus.Created,
            });
        var service = new RentalService(repository);

        var actual = service.HasOverlap(1, day.AddHours(startHour), day.AddHours(endHour));

        Assert.Equal(expected, actual);
    }
}
