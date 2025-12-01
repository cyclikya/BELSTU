using Avia.Data.Entities;

namespace Avia.Services.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int userId);
    Task<User> CreateUserAsync(string passportNumber, string password, string lastName, 
        string firstName, string? middleName, RoleType role, DateTime birthDate);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int userId);
    Task<List<User>> SearchUsersAsync(string searchTerm);
    Task<List<User>> FilterUsersByRoleAsync(RoleType role);
}

