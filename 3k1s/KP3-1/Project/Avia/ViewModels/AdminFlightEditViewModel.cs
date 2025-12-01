using Avia.Data.Entities;
using Avia.Infrastructure;
using Avia.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
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

    [ObservableProperty]
    private string windowTitle = "Добавление рейса";

    private int? _flightId;

    // Строковые свойства для валидации
    public string DepartureTimeString
    {
        get => DepartureTime.ToString(@"hh\:mm");
        set
        {
            if (TimeSpan.TryParseExact(value, @"hh\:mm", CultureInfo.InvariantCulture, out var time))
            {
                DepartureTime = time;
            }
        }
    }

    public string ArrivalTimeString
    {
        get => ArrivalTime.ToString(@"hh\:mm");
        set
        {
            if (TimeSpan.TryParseExact(value, @"hh\:mm", CultureInfo.InvariantCulture, out var time))
            {
                ArrivalTime = time;
            }
        }
    }

    public string EconomyPriceString
    {
        get => EconomyPrice.ToString("F2", CultureInfo.InvariantCulture);
        set
        {
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var price) && price >= 0)
            {
                EconomyPrice = price;
            }
        }
    }

    public string BusinessPriceString
    {
        get => BusinessPrice.ToString("F2", CultureInfo.InvariantCulture);
        set
        {
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var price) && price >= 0)
            {
                BusinessPrice = price;
            }
        }
    }

    public string EconomySeatsString
    {
        get => EconomySeats.ToString();
        set
        {
            if (int.TryParse(value, out var seats) && seats > 0)
            {
                EconomySeats = seats;
            }
        }
    }

    public string BusinessSeatsString
    {
        get => BusinessSeats.ToString();
        set
        {
            if (int.TryParse(value, out var seats) && seats > 0)
            {
                BusinessSeats = seats;
            }
        }
    }

    public string BaggagePriceString
    {
        get => BaggagePrice.ToString("F2", CultureInfo.InvariantCulture);
        set
        {
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var price) && price >= 0)
            {
                BaggagePrice = price;
            }
        }
    }

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
        WindowTitle = "Редактирование рейса";
        OnPropertyChanged(nameof(DepartureTimeString));
        OnPropertyChanged(nameof(ArrivalTimeString));
        OnPropertyChanged(nameof(EconomyPriceString));
        OnPropertyChanged(nameof(BusinessPriceString));
        OnPropertyChanged(nameof(EconomySeatsString));
        OnPropertyChanged(nameof(BusinessSeatsString));
        OnPropertyChanged(nameof(BaggagePriceString));
    }

    [RelayCommand]
    private async Task Save()
    {
        ErrorMessage = string.Empty;

        // Валидация обязательных полей
        if (string.IsNullOrWhiteSpace(DepartureCity))
        {
            ErrorMessage = "Город отправления обязателен";
            return;
        }

        if (string.IsNullOrWhiteSpace(ArrivalCity))
        {
            ErrorMessage = "Город прибытия обязателен";
            return;
        }

        if (string.IsNullOrWhiteSpace(Airline))
        {
            ErrorMessage = "Авиакомпания обязательна";
            return;
        }

        if (EconomyPrice <= 0)
        {
            ErrorMessage = "Цена эконом должна быть больше 0";
            return;
        }

        if (BusinessPrice <= 0)
        {
            ErrorMessage = "Цена бизнес должна быть больше 0";
            return;
        }

        if (EconomySeats <= 0)
        {
            ErrorMessage = "Количество мест эконом должно быть больше 0";
            return;
        }

        if (BusinessSeats <= 0)
        {
            ErrorMessage = "Количество мест бизнес должно быть больше 0";
            return;
        }

        if (BaggagePrice < 0)
        {
            ErrorMessage = "Цена багажа не может быть отрицательной";
            return;
        }

        var departureDateTime = DepartureDate.Date.Add(DepartureTime);
        var arrivalDateTime = ArrivalDate.Date.Add(ArrivalTime);

        if (arrivalDateTime <= departureDateTime)
        {
            ErrorMessage = "Дата и время прибытия должны быть позже даты и времени вылета";
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

