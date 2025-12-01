using Avia.Infrastructure;
using Avia.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avia.ViewModels;

public partial class RegistrationViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private string passportNumber = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private string lastName = string.Empty;

    [ObservableProperty]
    private string firstName = string.Empty;

    [ObservableProperty]
    private string? middleName;

    [ObservableProperty]
    private DateTime birthDate = DateTime.Now.AddYears(-18);

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public RegistrationViewModel(IAuthService authService, NavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task Register()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(PassportNumber) || 
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(FirstName))
        {
            ErrorMessage = "Заполните все обязательные поля";
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Пароли не совпадают";
            return;
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "Пароль должен содержать минимум 6 символов";
            return;
        }

        // Проверка даты рождения
        if (BirthDate > DateTime.Now.AddYears(-18))
        {
            ErrorMessage = "Вы должны быть старше 18 лет для регистрации";
            return;
        }

        if (BirthDate < DateTime.Now.AddYears(-120))
        {
            ErrorMessage = "Указана некорректная дата рождения";
            return;
        }

        try
        {
            var success = await _authService.RegisterAsync(
                PassportNumber, Password, LastName, FirstName, MiddleName, BirthDate);

            if (!success)
            {
                ErrorMessage = "Ошибка регистрации. Возможно, пользователь с таким паспортом уже существует или возраст меньше 18 лет";
                return;
            }

            _navigationService.NavigateTo<ClientMainViewModel>();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    private void NavigateToLogin()
    {
        _navigationService.NavigateTo<LoginViewModel>();
    }
}

