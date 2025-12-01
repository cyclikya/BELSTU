using Avia.Data;
using Avia.Data.Entities;
using Avia.Infrastructure;
using Avia.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Avia.Services;

public class AuthService : IAuthService
{
    private readonly AviaDbContext _context;
    private User? _currentUser;

    public AuthService(AviaDbContext context)
    {
        _context = context;
    }

    public User? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser != null;

    public async Task<User?> LoginAsync(string passportNumber, string password)
    {
        try
        {
            // Убеждаемся, что search_path установлен
            var connection = _context.Database.GetDbConnection();
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                await _context.Database.OpenConnectionAsync();
            }

            try
            {
                var setPathCommand = connection.CreateCommand();
                setPathCommand.CommandText = "SET search_path TO avia";
                await setPathCommand.ExecuteNonQueryAsync();
            }
            catch
            {
                // Игнорируем ошибку
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PassportNumber == passportNumber);

            if (user == null || !PasswordHasher.VerifyPassword(password, user.Pass))
            {
                return null;
            }

            // Для timestamp without time zone используем DateTime с Kind=Unspecified
            user.LastLogin = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            await _context.SaveChangesAsync();

            _currentUser = user;
            return user;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<bool> RegisterAsync(string passportNumber, string password, 
        string lastName, string firstName, string? middleName, DateTime birthDate)
    {
        try
        {
            // Убеждаемся, что search_path установлен
            var connection = _context.Database.GetDbConnection();
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                await _context.Database.OpenConnectionAsync();
            }

            try
            {
                var setPathCommand = connection.CreateCommand();
                setPathCommand.CommandText = "SET search_path TO avia";
                await setPathCommand.ExecuteNonQueryAsync();
            }
            catch
            {
                // Игнорируем ошибку
            }

            if (await _context.Users.AnyAsync(u => u.PassportNumber == passportNumber))
            {
                return false;
            }

            var age = DateTime.UtcNow.Year - birthDate.Year;
            if (DateTime.UtcNow.DayOfYear < birthDate.DayOfYear) age--;
            if (age < 18)
            {
                return false;
            }

            var hashedPassword = PasswordHasher.HashPassword(password);
            // Для timestamp without time zone используем DateTime с Kind=Unspecified
            var createdAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            var user = new User
            {
                PassportNumber = passportNumber,
                Pass = hashedPassword,
                LastName = lastName,
                FirstName = firstName,
                MiddleName = middleName,
                BirthDate = birthDate.Date, // Сохраняем только дату, без времени
                AccessRole = RoleType.Client,
                CreatedAt = createdAt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _currentUser = user;
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Register error: {ex.Message}");
            if (ex.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public void Logout()
    {
        _currentUser = null;
    }
}

