namespace Velo.Core;

/// <summary>Велосипед из каталога проката.</summary>
public class Bicycle
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal PricePerHour { get; set; }
    public bool IsAvailable { get; set; } = true;
}

/// <summary>Статусы заказа, допустимые в базе данных.</summary>
public static class OrderStatus
{
    public const string Created = "created";
    public const string Active = "active";
    public const string Completed = "completed";
    public const string Cancelled = "cancelled";
}

/// <summary>Заказ на аренду велосипеда.</summary>
public class RentalOrder
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BicycleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = OrderStatus.Created;
    public decimal TotalPrice { get; set; }
}
