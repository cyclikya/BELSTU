using Avia.Data;
using Avia.Data.Entities;
using Avia.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Avia.Services;

public class FlightService : IFlightService
{
    private readonly AviaDbContext _context;

    public FlightService(AviaDbContext context)
    {
        _context = context;
    }

    public async Task<List<Flight>> GetAllFlightsAsync()
    {
        return await _context.Flights.ToListAsync();
    }

    public async Task<Flight?> GetFlightByIdAsync(int flightId)
    {
        return await _context.Flights.FindAsync(flightId);
    }

    public async Task<Flight> CreateFlightAsync(string departureCity, string arrivalCity,
        DateTime departureDate, TimeSpan departureTime, DateTime arrivalDate,
        TimeSpan arrivalTime, string airline, decimal economyPrice, decimal businessPrice,
        int economySeats, int businessSeats, decimal baggagePrice)
    {
        var departureDateTime = departureDate.Date.Add(departureTime);
        var arrivalDateTime = arrivalDate.Date.Add(arrivalTime);

        if (arrivalDateTime <= departureDateTime)
        {
            throw new InvalidOperationException("Arrival date must be later than departure date");
        }

        var flight = new Flight
        {
            DepartureCity = departureCity,
            ArrivalCity = arrivalCity,
            DepartureDate = departureDate,
            DepartureTime = departureTime,
            ArrivalDate = arrivalDate,
            ArrivalTime = arrivalTime,
            Airline = airline,
            EconomyPrice = economyPrice,
            BusinessPrice = businessPrice,
            EconomySeats = economySeats,
            BusinessSeats = businessSeats,
            BaggagePrice = baggagePrice
        };

        _context.Flights.Add(flight);
        await _context.SaveChangesAsync();
        return flight;
    }

    public async Task UpdateFlightAsync(Flight flight)
    {
        var departureDateTime = flight.DepartureDate.Date.Add(flight.DepartureTime);
        var arrivalDateTime = flight.ArrivalDate.Date.Add(flight.ArrivalTime);

        if (arrivalDateTime <= departureDateTime)
        {
            throw new InvalidOperationException("Arrival date must be later than departure date");
        }

        _context.Flights.Update(flight);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteFlightAsync(int flightId)
    {
        var flight = await _context.Flights.FindAsync(flightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found");

        // Удаляем все билеты, связанные с этим рейсом
        var tickets = await _context.Tickets
            .Where(t => t.FlightId == flightId)
            .ToListAsync();
        _context.Tickets.RemoveRange(tickets);

        _context.Flights.Remove(flight);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Flight>> SearchFlightsAsync(string departureCity, string arrivalCity)
    {
        return await _context.Flights
            .Where(f => (string.IsNullOrEmpty(departureCity) || f.DepartureCity.ToLower().Contains(departureCity.ToLower())) &&
                       (string.IsNullOrEmpty(arrivalCity) || f.ArrivalCity.ToLower().Contains(arrivalCity.ToLower())))
            .ToListAsync();
    }

    public async Task<List<Flight>> FilterFlightsByDateAsync(DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.Flights.AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(f => f.DepartureDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(f => f.DepartureDate <= toDate.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<int> GetAvailableSeatsAsync(int flightId, ClassType classType)
    {
        var flight = await _context.Flights.FindAsync(flightId);
        if (flight == null)
            return 0;

        var totalSeats = classType == ClassType.Economy ? flight.EconomySeats : flight.BusinessSeats;
        var soldTickets = await _context.Tickets
            .CountAsync(t => t.FlightId == flightId && 
                           t.ClassType == classType && 
                           t.Status == TicketStatus.Active);

        return totalSeats - soldTickets;
    }

    public async Task<decimal> GetFlightPriceAsync(int flightId, ClassType classType, bool baggage)
    {
        var flight = await _context.Flights.FindAsync(flightId);
        if (flight == null)
            return 0;

        var basePrice = classType == ClassType.Economy ? flight.EconomyPrice : flight.BusinessPrice;
        return basePrice + (baggage ? flight.BaggagePrice : 0);
    }

    public async Task<(int EconomyAvailable, int EconomyTotal, int BusinessAvailable, int BusinessTotal)> GetAvailableSeatsInfoAsync(int flightId)
    {
        var flight = await _context.Flights.FindAsync(flightId);
        if (flight == null)
            return (0, 0, 0, 0);

        var economyTotal = flight.EconomySeats;
        var businessTotal = flight.BusinessSeats;

        var economySold = await _context.Tickets
            .CountAsync(t => t.FlightId == flightId && 
                           t.ClassType == ClassType.Economy && 
                           t.Status == TicketStatus.Active);

        var businessSold = await _context.Tickets
            .CountAsync(t => t.FlightId == flightId && 
                           t.ClassType == ClassType.Business && 
                           t.Status == TicketStatus.Active);

        var economyAvailable = economyTotal - economySold;
        var businessAvailable = businessTotal - businessSold;

        return (economyAvailable, economyTotal, businessAvailable, businessTotal);
    }
}

