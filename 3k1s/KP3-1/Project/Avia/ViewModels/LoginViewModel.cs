using Avia.Data.Entities;
using Avia.Infrastructure;
using Avia.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace Avia.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private string passportNumber = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public LoginViewModel(IAuthService authService, NavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task Login()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(PassportNumber) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Заполните все поля";
            return;
        }

        try
        {
            var user = await _authService.LoginAsync(PassportNumber, Password);
            if (user == null)
            {
                ErrorMessage = "Неверный номер паспорта или пароль";
                return;
            }

            if (user.AccessRole == RoleType.Admin)
            {
                _navigationService.NavigateTo<AdminMainViewModel>();
            }
            else
            {
                _navigationService.NavigateTo<ClientMainViewModel>();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    private void NavigateToRegistration()
    {
        _navigationService.NavigateTo<RegistrationViewModel>();
    }
}

