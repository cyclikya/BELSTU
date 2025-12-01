using Avia.Data;
using Avia.Data.Entities;
using Avia.Infrastructure;
using Avia.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Avia.Services;

public class UserService : IUserService
{
    private readonly AviaDbContext _context;

    public UserService(AviaDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<User> CreateUserAsync(string passportNumber, string password, 
        string lastName, string firstName, string? middleName, RoleType role, DateTime birthDate)
    {
        if (await _context.Users.AnyAsync(u => u.PassportNumber == passportNumber))
        {
            throw new InvalidOperationException("User with this passport number already exists");
        }

        var age = DateTime.UtcNow.Year - birthDate.Year;
        if (DateTime.UtcNow.DayOfYear < birthDate.DayOfYear) age--;
        if (age < 18)
        {
            throw new InvalidOperationException("User must be at least 18 years old");
        }

        var hashedPassword = PasswordHasher.HashPassword(password);
        var user = new User
        {
            PassportNumber = passportNumber,
            Pass = hashedPassword,
            LastName = lastName,
            FirstName = firstName,
            MiddleName = middleName,
            AccessRole = role,
            BirthDate = birthDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        if (user.AccessRole == RoleType.Admin)
            throw new InvalidOperationException("Cannot delete administrator");

        // Удаляем все билеты, купленные этим пользователем
        var tickets = await _context.Tickets
            .Where(t => t.UserId == userId)
            .ToListAsync();
        _context.Tickets.RemoveRange(tickets);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<List<User>> SearchUsersAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _context.Users
            .Where(u => u.PassportNumber.ToLower().Contains(term) ||
                       u.LastName.ToLower().Contains(term) ||
                       u.FirstName.ToLower().Contains(term) ||
                       (u.MiddleName != null && u.MiddleName.ToLower().Contains(term)))
            .ToListAsync();
    }

    public async Task<List<User>> FilterUsersByRoleAsync(RoleType role)
    {
        return await _context.Users
            .Where(u => u.AccessRole == role)
            .ToListAsync();
    }
}

