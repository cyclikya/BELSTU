using Avia.Data.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Avia.Views;

public partial class TicketCard : UserControl
{
    public static readonly DependencyProperty TicketProperty =
        DependencyProperty.Register(nameof(Ticket), typeof(Ticket), typeof(TicketCard),
            new PropertyMetadata(null, OnTicketChanged));

    public static readonly DependencyProperty CancelTicketCommandProperty =
        DependencyProperty.Register(nameof(CancelTicketCommand), typeof(ICommand), typeof(TicketCard),
            new PropertyMetadata(null, OnCancelTicketCommandChanged));

    public Ticket Ticket
    {
        get => (Ticket)GetValue(TicketProperty);
        set => SetValue(TicketProperty, value);
    }

    public ICommand CancelTicketCommand
    {
        get => (ICommand)GetValue(CancelTicketCommandProperty);
        set => SetValue(CancelTicketCommandProperty, value);
    }

    public TicketCard()
    {
        InitializeComponent();
        Loaded += TicketCard_Loaded;
    }

    private void TicketCard_Loaded(object sender, RoutedEventArgs e)
    {
        // Если Ticket уже установлен, обновляем привязки
        // Но только если DataContext еще не установлен (чтобы избежать конфликта)
        if (Ticket != null && DataContext == null)
        {
            UpdateBindings(Ticket);
        }
    }

    private static void OnTicketChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TicketCard card)
        {
            if (e.NewValue is Ticket ticket)
            {
                // Обновляем привязки только если Ticket установлен
                // Используем Dispatcher для обновления после завершения привязки
                _ = card.Dispatcher.BeginInvoke(new Action(() => card.UpdateBindings(ticket)), 
                    DispatcherPriority.Loaded);
            }
            else if (e.OldValue != null && e.NewValue == null)
            {
                // Если Ticket стал null, очищаем DataContext
                card.DataContext = null;
            }
        }
    }

    private static void OnCancelTicketCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TicketCard card && card.DataContext is TicketCardViewModel viewModel)
        {
            viewModel.CancelTicketCommand = card.CancelTicketCommand;
        }
        else if (d is TicketCard card2 && card2.Ticket != null)
        {
            // Если команда изменилась до установки Ticket, обновим привязки
            card2.UpdateBindings(card2.Ticket);
        }
    }

    private void UpdateBindings(Ticket ticket)
    {
        if (ticket == null)
        {
            System.Diagnostics.Debug.WriteLine("TicketCard: Ticket is null");
            return;
        }

        if (ticket.Flight == null)
        {
            System.Diagnostics.Debug.WriteLine($"TicketCard: Flight is null for ticket {ticket.TicketId}");
            return;
        }

        System.Diagnostics.Debug.WriteLine($"TicketCard: Updating bindings for ticket {ticket.TicketId}");

        var flight = ticket.Flight;
        var route = $"{flight.DepartureCity}-{flight.ArrivalCity}";
        var formattedDate = FormatDate(flight.DepartureDate);
        var duration = CalculateDuration(flight);
        var flightNumber = $"Рейс: {flight.FlightId}";
        
        // Вычисляем цену билета
        var basePrice = ticket.ClassType == ClassType.Economy ? flight.EconomyPrice : flight.BusinessPrice;
        var totalPrice = basePrice + (ticket.Baggage ? flight.BaggagePrice : 0);
        
        // Форматируем информацию о классе и покупке
        var classType = ticket.ClassType == ClassType.Economy ? "эконом" : "бизнес";
        var ticketClassText = $"класс билета: {classType}";
        var purchaseNumberText = $"номер покупки: {ticket.TicketId}";
        
        // Проверка, прошел ли рейс
        var isPastFlight = flight.DepartureDateTime < DateTime.Now;
        
        // Статус
        var statusText = ticket.Status == TicketStatus.Active ? "активен" : "отменён";
        var statusColor = ticket.Status == TicketStatus.Active 
            ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#059669")) 
            : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626"));
        
        // Дата покупки
        var purchaseDate = $"куплен: {ticket.PurchaseDate:dd.MM.yyyy HH:mm}";
        var statusLabel = $"текущий статус: {statusText}";

        var viewModel = new TicketCardViewModel
        {
            Ticket = ticket,
            Route = route,
            FormattedDate = formattedDate,
            Duration = duration,
            FlightNumber = flightNumber,
            TicketClassText = ticketClassText,
            PurchaseNumberText = purchaseNumberText,
            Price = $"{totalPrice:F0} р.",
            PurchaseDate = purchaseDate,
            StatusText = statusLabel,
            StatusColor = statusColor,
            DepartureTime = flight.DepartureTime,
            ArrivalTime = flight.ArrivalTime,
            Airline = flight.Airline,
            CanCancel = (ticket.Status == TicketStatus.Active && !isPastFlight) ? Visibility.Visible : Visibility.Collapsed,
            IsPastFlight = isPastFlight,
            CancelTicketCommand = CancelTicketCommand
        };

        DataContext = viewModel;
        System.Diagnostics.Debug.WriteLine($"TicketCard: DataContext set. Route={route}, Price={totalPrice}");
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

public class TicketCardViewModel : ObservableObject
{
    private Ticket? _ticket;
    private string _route = string.Empty;
    private string _formattedDate = string.Empty;
    private string _duration = string.Empty;
    private string _flightNumber = string.Empty;
    private string _ticketClassText = string.Empty;
    private string _purchaseNumberText = string.Empty;
    private string _price = string.Empty;
    private string _purchaseDate = string.Empty;
    private string _statusText = string.Empty;
    private Brush _statusColor = Brushes.Black;
    private TimeSpan _departureTime;
    private TimeSpan _arrivalTime;
    private string _airline = string.Empty;
    private Visibility _canCancel;
    private bool _isPastFlight;
    private ICommand? _cancelTicketCommand;

    public Ticket? Ticket { get => _ticket; set => SetProperty(ref _ticket, value); }
    public string Route { get => _route; set => SetProperty(ref _route, value); }
    public string FormattedDate { get => _formattedDate; set => SetProperty(ref _formattedDate, value); }
    public string Duration { get => _duration; set => SetProperty(ref _duration, value); }
    public string FlightNumber { get => _flightNumber; set => SetProperty(ref _flightNumber, value); }
    public string TicketClassText { get => _ticketClassText; set => SetProperty(ref _ticketClassText, value); }
    public string PurchaseNumberText { get => _purchaseNumberText; set => SetProperty(ref _purchaseNumberText, value); }
    public string Price { get => _price; set => SetProperty(ref _price, value); }
    public string PurchaseDate { get => _purchaseDate; set => SetProperty(ref _purchaseDate, value); }
    public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }
    public Brush StatusColor { get => _statusColor; set => SetProperty(ref _statusColor, value); }
    public TimeSpan DepartureTime { get => _departureTime; set => SetProperty(ref _departureTime, value); }
    public TimeSpan ArrivalTime { get => _arrivalTime; set => SetProperty(ref _arrivalTime, value); }
    public string Airline { get => _airline; set => SetProperty(ref _airline, value); }
    public Visibility CanCancel { get => _canCancel; set => SetProperty(ref _canCancel, value); }
    public bool IsPastFlight { get => _isPastFlight; set => SetProperty(ref _isPastFlight, value); }
    public ICommand? CancelTicketCommand { get => _cancelTicketCommand; set => SetProperty(ref _cancelTicketCommand, value); }
}

