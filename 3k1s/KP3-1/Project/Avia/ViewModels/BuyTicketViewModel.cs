using Avia.Data.Entities;
using Avia.Infrastructure;
using Avia.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace Avia.ViewModels;

public partial class BuyTicketViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly IFlightService _flightService;
    private readonly ITicketService _ticketService;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private Flight? flight;

    [ObservableProperty]
    private ClassType selectedClass = ClassType.Economy;

    [ObservableProperty]
    private bool includeBaggage = false;

    [ObservableProperty]
    private int availableSeats = 0;

    [ObservableProperty]
    private int seatsCount = 1;

    [ObservableProperty]
    private decimal totalPrice = 0;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public BuyTicketViewModel(IAuthService authService, IFlightService flightService,
        ITicketService ticketService, NavigationService navigationService)
    {
        _authService = authService;
        _flightService = flightService;
        _ticketService = ticketService;
        _navigationService = navigationService;
    }

    public void SetFlight(Flight flight)
    {
        Flight = flight;
        LoadFlightData();
    }

    private async void LoadFlightData()
    {
        if (Flight == null) return;

        try
        {
            AvailableSeats = await _flightService.GetAvailableSeatsAsync(Flight.FlightId, SelectedClass);
            var singleTicketPrice = await _flightService.GetFlightPriceAsync(Flight.FlightId, SelectedClass, IncludeBaggage);
            TotalPrice = singleTicketPrice * SeatsCount;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка: {ex.Message}";
        }
    }

    partial void OnSelectedClassChanged(ClassType value)
    {
        LoadFlightData();
    }

    partial void OnIncludeBaggageChanged(bool value)
    {
        LoadFlightData();
    }

    partial void OnSeatsCountChanged(int value)
    {
        LoadFlightData();
    }

    [RelayCommand]
    private async Task Buy()
    {
        ErrorMessage = string.Empty;

        if (Flight == null || _authService.CurrentUser == null)
        {
            ErrorMessage = "Ошибка: данные не загружены";
            return;
        }

        if (AvailableSeats <= 0)
        {
            ErrorMessage = "Нет доступных мест";
            return;
        }

        if (SeatsCount > AvailableSeats)
        {
            ErrorMessage = $"Недостаточно мест. Доступно: {AvailableSeats}";
            return;
        }

        if (SeatsCount <= 0)
        {
            ErrorMessage = "Количество мест должно быть больше 0";
            return;
        }

        try
        {
            // Покупаем билеты в цикле
            for (int i = 0; i < SeatsCount; i++)
            {
                await _ticketService.BuyTicketAsync(
                    _authService.CurrentUser.UserId,
                    Flight.FlightId,
                    SelectedClass,
                    IncludeBaggage);
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

