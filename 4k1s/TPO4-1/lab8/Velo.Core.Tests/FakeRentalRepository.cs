using Velo.Core;

namespace Velo.Core.Tests;

/// <summary>
/// Тестовая заглушка вместо настоящего хранилища.
///
/// Совмещает две роли из классификации тестовых двойников:
///   • stub  — возвращает заранее заданные данные (каталог и заказы),
///             чтобы сервис не обращался к базе;
///   • spy   — запоминает, что у него вызвали, чтобы тест мог проверить
///             побочные эффекты: сохранён ли заказ и снят ли признак
///             доступности велосипеда.
///
/// Написана вручную: так на защите видно, что именно подменяется,
/// и не нужно объяснять синтаксис сторонней библиотеки.
/// </summary>
public class FakeRentalRepository : IRentalRepository
{
    private readonly Dictionary<int, Bicycle> _bicycles = new();
    private readonly Dictionary<int, List<RentalOrder>> _orders = new();
    private int _nextOrderId = 1;

    /// <summary>Заказы, сохранённые через этот репозиторий.</summary>
    public List<RentalOrder> SavedOrders { get; } = new();

    /// <summary>История вызовов SetAvailability: что и на какое значение меняли.</summary>
    public List<(int BicycleId, bool IsAvailable)> AvailabilityChanges { get; } = new();

    public FakeRentalRepository WithBicycle(Bicycle bicycle)
    {
        _bicycles[bicycle.Id] = bicycle;
        return this;
    }

    public FakeRentalRepository WithActiveOrder(RentalOrder order)
    {
        if (!_orders.TryGetValue(order.BicycleId, out var list))
        {
            list = new List<RentalOrder>();
            _orders[order.BicycleId] = list;
        }
        list.Add(order);
        return this;
    }

    public Bicycle? GetBicycle(int bicycleId)
        => _bicycles.TryGetValue(bicycleId, out var bicycle) ? bicycle : null;

    public IReadOnlyList<RentalOrder> GetActiveOrders(int bicycleId)
        => _orders.TryGetValue(bicycleId, out var list)
            ? list
            : Array.Empty<RentalOrder>();

    public RentalOrder AddOrder(RentalOrder order)
    {
        order.Id = _nextOrderId++;
        SavedOrders.Add(order);
        WithActiveOrder(order);
        return order;
    }

    public void SetAvailability(int bicycleId, bool isAvailable)
    {
        AvailabilityChanges.Add((bicycleId, isAvailable));
        if (_bicycles.TryGetValue(bicycleId, out var bicycle))
            bicycle.IsAvailable = isAvailable;
    }
}
