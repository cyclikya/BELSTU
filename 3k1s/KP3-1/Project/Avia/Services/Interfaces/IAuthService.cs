using Avia.Data.Entities;

namespace Avia.Services.Interfaces;

public interface IAuthService
{
    Task<User?> LoginAsync(string passportNumber, string password);
    Task<bool> RegisterAsync(string passportNumber, string password, string lastName, 
        string firstName, string? middleName, DateTime birthDate);
    void Logout();
    User? CurrentUser { get; }
    bool IsAuthenticated { get; }
}

