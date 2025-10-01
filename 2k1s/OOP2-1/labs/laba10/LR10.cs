using System;
using System.Collections.Generic;
using System.Linq;

public partial class Airline
{
    private string fPoint;
    private readonly int id;
    private string typeOfPlane;
    private string sTime;
    private string dayOfWeek;

    public static string AirlineCompanyName;
    private static int PlaneCount;
    public const string AirportName = "International Airport";

    public string Destination => fPoint;
    public string AircraftType => typeOfPlane;
    public string DayOfWeek => dayOfWeek;
    public DateTime DepartureTime => DateTime.Parse(sTime);
    public Airline(string fPoint, string typeOfPlane, string sTime, string dayOfWeek)
    {
        this.fPoint = fPoint;
        this.typeOfPlane = typeOfPlane;
        this.sTime = sTime;
        this.dayOfWeek = dayOfWeek;

        PlaneCount++;

        id = GetHashCode();
    }

    public override string ToString()
    {
        return $" ID: {id}, Пункт назначения: {fPoint}, Самолет: {typeOfPlane}, Время: {sTime}, День: {dayOfWeek}, Аэропорт: {AirportName}, Авиакомпания: {AirlineCompanyName}.";
    }
}

class LR10
{
    static void Main()
    {
        string[] months = { "June", "July", "May", "December", "January", "February", "March", "April", "August", "September", "October", "November" };

        int n = 5;
        var monthsWithLengthN = months.Where(m => m.Length == n);
        Console.WriteLine("Месяцы с длиной строки {0}: {1}", n, string.Join(", ", monthsWithLengthN));

        var summerAndWinterMonths = months.Where(m => new[] { "June", "July", "August", "December", "January", "February" }.Contains(m));
        Console.WriteLine("Летние и зимние месяцы: {0}", string.Join(", ", summerAndWinterMonths));

        var monthsAlphabetical = months.OrderBy(m => m);
        Console.WriteLine("Месяцы в алфавитном порядке: {0}", string.Join(", ", monthsAlphabetical));

        var monthsWithU = months.Where(m => m.Contains('u') && m.Length >= 4);
        Console.WriteLine("Месяцы с буквой 'u' и длиной не менее 4: {0}", string.Join(", ", monthsWithU));



        List<Airline> flights = new List<Airline>
        {
            new Airline("Москва", "Boeing 747", "08:30", "Понедельник"),
            new Airline("Париж", "Airbus A320", "12:00", "Вторник"),
            new Airline("Лондон", "Boeing 737", "18:45", "Среда"),
            new Airline("Москва", "Airbus A320", "21:15", "Понедельник"),
            new Airline("Берлин", "Boeing 747", "06:00", "Четверг"),
            new Airline("Москва", "Airbus A320", "23:00", "Пятница"),
            new Airline("Париж", "Boeing 737", "17:00", "Понедельник"),
            new Airline("Лондон", "Boeing 747", "20:30", "Среда"),
            new Airline("Берлин", "Airbus A320", "07:45", "Вторник"),
            new Airline("Москва", "Boeing 737", "22:30", "Понедельник")
        };

        string mosc = "Москва";
        var flightsToDestination = flights.Where(f => f.Destination == mosc);
        Console.WriteLine($"Рейсы в пункт назначения '{mosc}':");
        foreach (var flight in flightsToDestination)
        {
            Console.WriteLine(flight);
        }

        string mon = "Понедельник";
        var flightsOnDay = flights.Where(f => f.DayOfWeek == mon);
        Console.WriteLine($"\nРейсы на '{mon}':");
        foreach (var flight in flightsOnDay)
        {
            Console.WriteLine(flight);
        }

        var maxFlightOnDay = flights.Where(f => f.DayOfWeek == mon)
                                    .OrderByDescending(f => f.DepartureTime)
                                    .FirstOrDefault();
        Console.WriteLine($"\nМаксимальный по времени вылета рейс на '{mon}':");
        Console.WriteLine(maxFlightOnDay);

        var latestFlightsOnDay = flights.Where(f => f.DayOfWeek == mon)
                                        .GroupBy(f => f.DayOfWeek)
                                        .Select(group => group.OrderByDescending(f => f.DepartureTime).First());
        Console.WriteLine($"\nСамый поздний рейс на '{mon}':");
        foreach (var flight in latestFlightsOnDay)
        {
            Console.WriteLine(flight);
        }
        var orderedFlights = flights.OrderBy(f => GetDayOfWeekOrder(f.DayOfWeek))
                                    .ThenBy(f => f.DepartureTime);

        Console.WriteLine("\nРейсы, упорядоченные по дню недели и времени вылета:");
        foreach (var flight in orderedFlights)
        {
            Console.WriteLine(flight);
        }
        string aircraftType = "Airbus A320";
        int countByAircraftType = flights.Count(f => f.AircraftType == aircraftType);
        Console.WriteLine($"\nКоличество рейсов для типа самолета '{aircraftType}': {countByAircraftType}");



        var query = flights
            .Where(f => f.Destination == "Москва")
            .GroupBy(f => f.DayOfWeek)
            .Select(g => new
            {
                Day = g.Key,
                FlightsCount = g.Count(),
                LatestFlight = g.OrderByDescending(f => f.DepartureTime).FirstOrDefault() 
            })
            .OrderBy(q => GetDayOfWeekOrder(q.Day))
            .ToList();
        foreach (var result in query)
        {
            Console.WriteLine($"День: {result.Day}, Количество рейсов: {result.FlightsCount}, Последний рейс: {result.LatestFlight}");
        }
    }

    static int GetDayOfWeekOrder(string day)
    {
        switch (day)
        {
            case "Понедельник": return 1;
            case "Вторник": return 2;
            case "Среда": return 3;
            case "Четверг": return 4;
            case "Пятница": return 5;
            case "Суббота": return 6;
            case "Воскресенье": return 7;
            default: return 8;
        }
    }
}
