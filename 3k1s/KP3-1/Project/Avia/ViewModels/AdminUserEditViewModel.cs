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

    [ObservableProperty]
    private string windowTitle = "Добавление пользователя";

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
        WindowTitle = "Редактирование пользователя";
    }

    [RelayCommand]
    private async Task Save()
    {
        ErrorMessage = string.Empty;

        // Валидация обязательных полей
        if (string.IsNullOrWhiteSpace(PassportNumber))
        {
            ErrorMessage = "Номер паспорта обязателен";
            return;
        }

        if (PassportNumber.Length < 5 || PassportNumber.Length > 20)
        {
            ErrorMessage = "Номер паспорта должен содержать от 5 до 20 символов";
            return;
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            ErrorMessage = "Фамилия обязательна";
            return;
        }

        if (LastName.Length < 2 || LastName.Length > 50)
        {
            ErrorMessage = "Фамилия должна содержать от 2 до 50 символов";
            return;
        }

        if (string.IsNullOrWhiteSpace(FirstName))
        {
            ErrorMessage = "Имя обязательно";
            return;
        }

        if (FirstName.Length < 2 || FirstName.Length > 50)
        {
            ErrorMessage = "Имя должно содержать от 2 до 50 символов";
            return;
        }

        if (MiddleName != null && MiddleName.Length > 50)
        {
            ErrorMessage = "Отчество не должно превышать 50 символов";
            return;
        }

        var age = DateTime.UtcNow.Year - BirthDate.Year;
        if (DateTime.UtcNow.DayOfYear < BirthDate.DayOfYear) age--;
        if (age < 18)
        {
            ErrorMessage = "Пользователь должен быть старше 18 лет";
            return;
        }

        if (BirthDate > DateTime.UtcNow)
        {
            ErrorMessage = "Дата рождения не может быть в будущем";
            return;
        }

        if (!IsEditMode && string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Пароль обязателен для нового пользователя";
            return;
        }

        if (!IsEditMode && Password.Length < 6)
        {
            ErrorMessage = "Пароль должен содержать минимум 6 символов";
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

