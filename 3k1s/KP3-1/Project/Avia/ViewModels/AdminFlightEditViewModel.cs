using Avia.Data.Entities;
using Avia.Infrastructure;
using Avia.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace Avia.ViewModels;

public partial class AdminFlightEditViewModel : ViewModelBase
{
    private readonly IFlightService _flightService;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private string departureCity = string.Empty;

    [ObservableProperty]
    private string arrivalCity = string.Empty;

    [ObservableProperty]
    private DateTime departureDate = DateTime.Now;

    [ObservableProperty]
    private TimeSpan departureTime = TimeSpan.FromHours(10);

    [ObservableProperty]
    private DateTime arrivalDate = DateTime.Now;

    [ObservableProperty]
    private TimeSpan arrivalTime = TimeSpan.FromHours(12);

    [ObservableProperty]
    private string airline = string.Empty;

    [ObservableProperty]
    private decimal economyPrice = 0;

    [ObservableProperty]
    private decimal businessPrice = 0;

    [ObservableProperty]
    private int economySeats = 0;

    [ObservableProperty]
    private int businessSeats = 0;

    [ObservableProperty]
    private decimal baggagePrice = 0;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool isEditMode = false;

    private int? _flightId;

    public AdminFlightEditViewModel(IFlightService flightService, NavigationService navigationService)
    {
        _flightService = flightService;
        _navigationService = navigationService;
    }

    public void SetFlight(Flight flight)
    {
        _flightId = flight.FlightId;
        DepartureCity = flight.DepartureCity;
        ArrivalCity = flight.ArrivalCity;
        DepartureDate = flight.DepartureDate;
        DepartureTime = flight.DepartureTime;
        ArrivalDate = flight.ArrivalDate;
        ArrivalTime = flight.ArrivalTime;
        Airline = flight.Airline;
        EconomyPrice = flight.EconomyPrice;
        BusinessPrice = flight.BusinessPrice;
        EconomySeats = flight.EconomySeats;
        BusinessSeats = flight.BusinessSeats;
        BaggagePrice = flight.BaggagePrice;
        IsEditMode = true;
    }

    [RelayCommand]
    private async Task Save()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(DepartureCity) ||
            string.IsNullOrWhiteSpace(ArrivalCity) ||
            string.IsNullOrWhiteSpace(Airline))
        {
            ErrorMessage = "Заполните все обязательные поля";
            return;
        }

        try
        {
            if (IsEditMode && _flightId.HasValue)
            {
                var flight = await _flightService.GetFlightByIdAsync(_flightId.Value);
                if (flight != null)
                {
                    flight.DepartureCity = DepartureCity;
                    flight.ArrivalCity = ArrivalCity;
                    flight.DepartureDate = DepartureDate;
                    flight.DepartureTime = DepartureTime;
                    flight.ArrivalDate = ArrivalDate;
                    flight.ArrivalTime = ArrivalTime;
                    flight.Airline = Airline;
                    flight.EconomyPrice = EconomyPrice;
                    flight.BusinessPrice = BusinessPrice;
                    flight.EconomySeats = EconomySeats;
                    flight.BusinessSeats = BusinessSeats;
                    flight.BaggagePrice = BaggagePrice;
                    await _flightService.UpdateFlightAsync(flight);
                }
            }
            else
            {
                await _flightService.CreateFlightAsync(
                    DepartureCity, ArrivalCity, DepartureDate, DepartureTime,
                    ArrivalDate, ArrivalTime, Airline, EconomyPrice, BusinessPrice,
                    EconomySeats, BusinessSeats, BaggagePrice);
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

