using Avia.Data.Entities;

namespace Avia.Services.Interfaces;

public interface ITicketService
{
    Task<List<Ticket>> GetAllTicketsAsync();
    Task<List<Ticket>> GetUserTicketsAsync(int userId);
    Task<Ticket?> GetTicketByIdAsync(int ticketId);
    Task<Ticket> BuyTicketAsync(int userId, int flightId, ClassType classType, bool baggage);
    Task CancelTicketAsync(int ticketId);
    Task<List<Ticket>> SearchTicketsAsync(int? userId, int? flightId);
    Task<List<Ticket>> FilterTicketsByStatusAsync(TicketStatus status);
}

