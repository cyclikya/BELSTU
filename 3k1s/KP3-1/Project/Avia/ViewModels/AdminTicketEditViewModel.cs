using Avia.Data.Entities;
using Avia.Infrastructure;
using Avia.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Avia.ViewModels;

public partial class AdminTicketEditViewModel : ViewModelBase
{
    private readonly ITicketService _ticketService;
    private readonly IUserService _userService;
    private readonly IFlightService _flightService;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<User> users = new();

    [ObservableProperty]
    private ObservableCollection<Flight> flights = new();

    [ObservableProperty]
    private User? selectedUser;

    [ObservableProperty]
    private Flight? selectedFlight;

    [ObservableProperty]
    private ClassType selectedClass = ClassType.Economy;

    [ObservableProperty]
    private bool includeBaggage = false;

    [ObservableProperty]
    private TicketStatus selectedStatus = TicketStatus.Active;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool isEditMode = false;

    [ObservableProperty]
    private string windowTitle = "Добавление билета";

    private int? _ticketId;
    private readonly Task _initializationTask;

    public AdminTicketEditViewModel(ITicketService ticketService, IUserService userService, 
        IFlightService flightService, NavigationService navigationService)
    {
        _ticketService = ticketService;
        _userService = userService;
        _flightService = flightService;
        _navigationService = navigationService;
        _initializationTask = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var usersList = await _userService.GetAllUsersAsync();
            Users.Clear();
            foreach (var user in usersList)
            {
                Users.Add(user);
            }

            var flightsList = await _flightService.GetAllFlightsAsync();
            Flights.Clear();
            foreach (var flight in flightsList)
            {
                Flights.Add(flight);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка загрузки данных: {ex.Message}";
        }
    }

    public async Task SetTicketAsync(Ticket ticket)
    {
        _ticketId = ticket.TicketId;
        await _initializationTask.ConfigureAwait(false);

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            // Теперь устанавливаем выбранные значения
            SelectedUser = Users.FirstOrDefault(u => u.UserId == ticket.UserId);
            SelectedFlight = Flights.FirstOrDefault(f => f.FlightId == ticket.FlightId);
            SelectedClass = ticket.ClassType;
            IncludeBaggage = ticket.Baggage;
            SelectedStatus = ticket.Status;
            IsEditMode = true;
            WindowTitle = "Редактирование билета";

            // Уведомляем об изменении выбранных элементов
            OnPropertyChanged(nameof(SelectedUser));
            OnPropertyChanged(nameof(SelectedFlight));
        }, DispatcherPriority.Background);
    }

    [RelayCommand]
    private async Task Save()
    {
        ErrorMessage = string.Empty;

        if (SelectedUser == null)
        {
            ErrorMessage = "Выберите пользователя";
            return;
        }

        if (SelectedFlight == null)
        {
            ErrorMessage = "Выберите рейс";
            return;
        }

        try
        {
            if (IsEditMode && _ticketId.HasValue)
            {
                var ticket = await _ticketService.GetTicketByIdAsync(_ticketId.Value);
                if (ticket != null)
                {
                    ticket.UserId = SelectedUser.UserId;
                    ticket.FlightId = SelectedFlight.FlightId;
                    ticket.ClassType = SelectedClass;
                    ticket.Baggage = IncludeBaggage;
                    ticket.Status = SelectedStatus;
                    await _ticketService.UpdateTicketAsync(ticket);
                }
            }
            else
            {
                await _ticketService.BuyTicketAsync(
                    SelectedUser.UserId, SelectedFlight.FlightId, SelectedClass, IncludeBaggage);
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

