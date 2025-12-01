using Avia.Data.Entities;
using Avia.Services.Interfaces;
using Avia.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Avia.Views;

public partial class FlightCard : UserControl
{
    public static readonly DependencyProperty FlightProperty =
        DependencyProperty.Register(nameof(Flight), typeof(Flight), typeof(FlightCard),
            new PropertyMetadata(null, OnFlightChanged));

    public static readonly DependencyProperty BuyTicketCommandProperty =
        DependencyProperty.Register(nameof(BuyTicketCommand), typeof(ICommand), typeof(FlightCard),
            new PropertyMetadata(null, OnBuyTicketCommandChanged));

    public static readonly DependencyProperty FlightServiceProperty =
        DependencyProperty.Register(nameof(FlightService), typeof(IFlightService), typeof(FlightCard),
            new PropertyMetadata(null));

    public Flight Flight
    {
        get => (Flight)GetValue(FlightProperty);
        set => SetValue(FlightProperty, value);
    }

    public ICommand BuyTicketCommand
    {
        get => (ICommand)GetValue(BuyTicketCommandProperty);
        set => SetValue(BuyTicketCommandProperty, value);
    }

    public IFlightService? FlightService
    {
        get => (IFlightService?)GetValue(FlightServiceProperty);
        set => SetValue(FlightServiceProperty, value);
    }

    public FlightCard()
    {
        InitializeComponent();
        Loaded += FlightCard_Loaded;
    }

    private void FlightCard_Loaded(object sender, RoutedEventArgs e)
    {
        // Если Flight уже установлен, обновляем привязки
        // Но только если DataContext еще не установлен (чтобы избежать конфликта)
        if (Flight != null && DataContext == null)
        {
            _ = UpdateBindingsAsync(Flight);
        }
    }

    private static void OnFlightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FlightCard card)
        {
            if (e.NewValue is Flight flight)
            {
                // Обновляем привязки только если Flight установлен
                // Это происходит когда свойство устанавливается через XAML привязку
                // Используем Dispatcher для обновления после завершения привязки
                _ = card.Dispatcher.BeginInvoke(new Action(async () => await card.UpdateBindingsAsync(flight)), 
                    DispatcherPriority.Loaded);
            }
            else if (e.OldValue != null && e.NewValue == null)
            {
                // Если Flight стал null, очищаем DataContext
                card.DataContext = null;
            }
        }
    }

    private async Task UpdateBindingsAsync(Flight flight)
    {
        if (flight == null)
        {
            System.Diagnostics.Debug.WriteLine("FlightCard: Flight is null");
            return;
        }

        System.Diagnostics.Debug.WriteLine($"FlightCard: Updating bindings for flight {flight.FlightId}");

        // Обновляем привязки для вычисляемых свойств
        var route = $"{flight.DepartureCity}-{flight.ArrivalCity}";
        var formattedDate = FormatDate(flight.DepartureDate);
        var duration = CalculateDuration(flight);
        var flightNumber = $"Nº{flight.FlightId}";

        // Вычисляем доступные места
        string availableSeats = $"Эконом: {flight.EconomySeats}, Бизнес: {flight.BusinessSeats}";
        
        if (FlightService != null)
        {
            try
            {
                var seatsInfo = await FlightService.GetAvailableSeatsInfoAsync(flight.FlightId);
                availableSeats = $"эконом: {seatsInfo.EconomyAvailable}/{seatsInfo.EconomyTotal} бизнес: {seatsInfo.BusinessAvailable}/{seatsInfo.BusinessTotal}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading seats info: {ex.Message}");
                // Используем значения по умолчанию
            }
        }

        // Устанавливаем через DataContext для привязок
        var viewModel = new FlightCardViewModel
        {
            Flight = flight,
            Route = route,
            FormattedDate = formattedDate,
            Duration = duration,
            FlightNumber = flightNumber,
            AvailableSeats = availableSeats,
            EconomyPrice = flight.EconomyPrice,
            BusinessPrice = flight.BusinessPrice,
            DepartureTime = flight.DepartureTime,
            ArrivalTime = flight.ArrivalTime,
            Airline = flight.Airline,
            BuyTicketCommand = BuyTicketCommand
        };

        DataContext = viewModel;
        System.Diagnostics.Debug.WriteLine($"FlightCard: DataContext set. Route={route}, Price={flight.EconomyPrice}");
    }

    private static void OnBuyTicketCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FlightCard card && card.DataContext is FlightCardViewModel viewModel)
        {
            viewModel.BuyTicketCommand = card.BuyTicketCommand;
        }
        else if (d is FlightCard card2 && card2.Flight != null)
        {
            // Если команда изменилась до установки Flight, обновим привязки
            _ = card2.UpdateBindingsAsync(card2.Flight);
        }
    }

    private string FormatDate(DateTime date)
    {
        var culture = new CultureInfo("ru-RU");
        var dayOfWeek = culture.DateTimeFormat.GetDayName(date.DayOfWeek);
        var month = culture.DateTimeFormat.GetMonthName(date.Month);
        return $"{date.Day} {CapitalizeFirst(month)}, {CapitalizeFirst(dayOfWeek)} {date.Year}";
    }

    private string CapitalizeFirst(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        return char.ToUpper(str[0]) + str.Substring(1).ToLower();
    }

    private string CalculateDuration(Flight flight)
    {
        var departure = flight.DepartureDate.Date.Add(flight.DepartureTime);
        var arrival = flight.ArrivalDate.Date.Add(flight.ArrivalTime);
        var duration = arrival - departure;
        
        var hours = (int)duration.TotalHours;
        var minutes = duration.Minutes;
        
        if (hours > 0 && minutes > 0)
            return $"{hours} ч {minutes} мин, без пересадок";
        else if (hours > 0)
            return $"{hours} часов, без пересадок";
        else
            return $"{minutes} мин, без пересадок";
    }
}

public class FlightCardViewModel : ObservableObject
{
    private Flight? _flight;
    private string _route = string.Empty;
    private string _formattedDate = string.Empty;
    private string _duration = string.Empty;
    private string _flightNumber = string.Empty;
    private string _availableSeats = string.Empty;
    private decimal _economyPrice;
    private decimal _businessPrice;
    private TimeSpan _departureTime;
    private TimeSpan _arrivalTime;
    private string _airline = string.Empty;
    private ICommand? _buyTicketCommand;

    public Flight? Flight { get => _flight; set => SetProperty(ref _flight, value); }
    public string Route { get => _route; set => SetProperty(ref _route, value); }
    public string FormattedDate { get => _formattedDate; set => SetProperty(ref _formattedDate, value); }
    public string Duration { get => _duration; set => SetProperty(ref _duration, value); }
    public string FlightNumber { get => _flightNumber; set => SetProperty(ref _flightNumber, value); }
    public string AvailableSeats { get => _availableSeats; set => SetProperty(ref _availableSeats, value); }
    public decimal EconomyPrice { get => _economyPrice; set => SetProperty(ref _economyPrice, value); }
    public decimal BusinessPrice { get => _businessPrice; set => SetProperty(ref _businessPrice, value); }
    public TimeSpan DepartureTime { get => _departureTime; set => SetProperty(ref _departureTime, value); }
    public TimeSpan ArrivalTime { get => _arrivalTime; set => SetProperty(ref _arrivalTime, value); }
    public string Airline { get => _airline; set => SetProperty(ref _airline, value); }
    public ICommand? BuyTicketCommand { get => _buyTicketCommand; set => SetProperty(ref _buyTicketCommand, value); }
}

