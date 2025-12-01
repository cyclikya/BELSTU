using Avia.Data.Entities;
using Avia.Infrastructure;
using Avia.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace Avia.ViewModels;

public partial class AdminUserEditViewModel : ViewModelBase
{
    private readonly IUserService _userService;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private string passportNumber = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string lastName = string.Empty;

    [ObservableProperty]
    private string firstName = string.Empty;

    [ObservableProperty]
    private string? middleName;

    [ObservableProperty]
    private RoleType role = RoleType.Client;

    [ObservableProperty]
    private DateTime birthDate = DateTime.Now.AddYears(-18);

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool isEditMode = false;

    private int? _userId;

    public AdminUserEditViewModel(IUserService userService, NavigationService navigationService)
    {
        _userService = userService;
        _navigationService = navigationService;
    }

    public void SetUser(User user)
    {
        _userId = user.UserId;
        PassportNumber = user.PassportNumber;
        LastName = user.LastName;
        FirstName = user.FirstName;
        MiddleName = user.MiddleName;
        Role = user.AccessRole;
        BirthDate = user.BirthDate;
        IsEditMode = true;
    }

    [RelayCommand]
    private async Task Save()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(PassportNumber) ||
            string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(FirstName))
        {
            ErrorMessage = "Заполните все обязательные поля";
            return;
        }

        if (!IsEditMode && string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Пароль обязателен для нового пользователя";
            return;
        }

        try
        {
            if (IsEditMode && _userId.HasValue)
            {
                var user = await _userService.GetUserByIdAsync(_userId.Value);
                if (user != null)
                {
                    user.PassportNumber = PassportNumber;
                    user.LastName = LastName;
                    user.FirstName = FirstName;
                    user.MiddleName = MiddleName;
                    user.AccessRole = Role;
                    user.BirthDate = BirthDate;
                    if (!string.IsNullOrWhiteSpace(Password))
                    {
                        user.Pass = Infrastructure.PasswordHasher.HashPassword(Password);
                    }
                    await _userService.UpdateUserAsync(user);
                }
            }
            else
            {
                await _userService.CreateUserAsync(
                    PassportNumber, Password, LastName, FirstName, MiddleName, Role, BirthDate);
            }

            CloseWindow();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseWindow();
    }

    private void CloseWindow()
    {
        Application.Current.Windows.OfType<Window>()
            .FirstOrDefault(w => w.DataContext == this)?.Close();
    }
}

