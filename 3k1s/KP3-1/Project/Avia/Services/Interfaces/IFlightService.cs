using Avia.Data.Entities;

namespace Avia.Services.Interfaces;

public interface IFlightService
{
    Task<List<Flight>> GetAllFlightsAsync();
    Task<Flight?> GetFlightByIdAsync(int flightId);
    Task<Flight> CreateFlightAsync(string departureCity, string arrivalCity, 
        DateTime departureDate, TimeSpan departureTime, DateTime arrivalDate, 
        TimeSpan arrivalTime, string airline, decimal economyPrice, decimal businessPrice, 
        int economySeats, int businessSeats, decimal baggagePrice);
    Task UpdateFlightAsync(Flight flight);
    Task DeleteFlightAsync(int flightId);
    Task<List<Flight>> SearchFlightsAsync(string departureCity, string arrivalCity);
    Task<List<Flight>> FilterFlightsByDateAsync(DateTime? fromDate, DateTime? toDate);
    Task<int> GetAvailableSeatsAsync(int flightId, ClassType classType);
    Task<decimal> GetFlightPriceAsync(int flightId, ClassType classType, bool baggage);
    Task<(int EconomyAvailable, int EconomyTotal, int BusinessAvailable, int BusinessTotal)> GetAvailableSeatsInfoAsync(int flightId);
}

