using Avia.Data;
using Avia.Data.Entities;
using Avia.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Avia.Services;

public class TicketService : ITicketService
{
    private readonly AviaDbContext _context;
    private readonly IFlightService _flightService;

    public TicketService(AviaDbContext context, IFlightService flightService)
    {
        _context = context;
        _flightService = flightService;
    }

    public async Task<List<Ticket>> GetAllTicketsAsync()
    {
        return await _context.Tickets
            .Include(t => t.Flight)
            .Include(t => t.User)
            .ToListAsync();
    }

    public async Task<List<Ticket>> GetUserTicketsAsync(int userId)
    {
        return await _context.Tickets
            .Include(t => t.Flight)
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<Ticket?> GetTicketByIdAsync(int ticketId)
    {
        return await _context.Tickets
            .Include(t => t.Flight)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TicketId == ticketId);
    }

    public async Task<Ticket> BuyTicketAsync(int userId, int flightId, ClassType classType, bool baggage)
    {
        var availableSeats = await _flightService.GetAvailableSeatsAsync(flightId, classType);
        if (availableSeats <= 0)
        {
            throw new InvalidOperationException("No available seats for this flight");
        }

        var ticket = new Ticket
        {
            UserId = userId,
            FlightId = flightId,
            ClassType = classType,
            Baggage = baggage,
            PurchaseDate = DateTime.UtcNow,
            Status = TicketStatus.Active
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task CancelTicketAsync(int ticketId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null)
            throw new InvalidOperationException("Ticket not found");

        ticket.Status = TicketStatus.Cancelled;
        await _context.SaveChangesAsync();
    }

    public async Task<List<Ticket>> SearchTicketsAsync(int? userId, int? flightId)
    {
        var query = _context.Tickets
            .Include(t => t.Flight)
            .Include(t => t.User)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(t => t.UserId == userId.Value);
        }

        if (flightId.HasValue)
        {
            query = query.Where(t => t.FlightId == flightId.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<List<Ticket>> FilterTicketsByStatusAsync(TicketStatus status)
    {
        return await _context.Tickets
            .Include(t => t.Flight)
            .Include(t => t.User)
            .Where(t => t.Status == status)
            .ToListAsync();
    }
}

