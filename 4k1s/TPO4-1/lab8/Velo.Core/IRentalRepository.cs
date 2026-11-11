namespace Velo.Core;

/// <summary>
/// Доступ к данным проката.
///
/// Это и есть шов для изоляции: сервис аренды зависит не от базы данных,
/// а от этого интерфейса. В приложении подставляется реализация поверх
/// Entity Framework, в тестах — заглушка, работающая в памяти.
/// </summary>
public interface IRentalRepository
{
    /// <summary>Велосипед по идентификатору или null, если такого нет.</summary>
    Bicycle? GetBicycle(int bicycleId);

    /// <summary>Действующие заказы на велосипед (без завершённых и отменённых).</summary>
    IReadOnlyList<RentalOrder> GetActiveOrders(int bicycleId);

    /// <summary>Сохраняет заказ и возвращает его с присвоенным идентификатором.</summary>
    RentalOrder AddOrder(RentalOrder order);

    /// <summary>Меняет признак доступности велосипеда.</summary>
    void SetAvailability(int bicycleId, bool isAvailable);
}
