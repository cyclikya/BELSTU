using Avia.Data.Entities;
using Avia.Infrastructure;
using Avia.Services.Interfaces;
using Avia.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Avia.ViewModels;

public partial class PersonalCabinetViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly ITicketService _ticketService;

    [ObservableProperty]
    private ObservableCollection<Ticket> myTickets = new();

    [ObservableProperty]
    private ObservableCollection<object> displayedItems = new();

    public PersonalCabinetViewModel(IAuthService authService, ITicketService ticketService)
    {
        _authService = authService;
        _ticketService = ticketService;
        _ = LoadDataAsync();
    }

    public string UserDisplayName
    {
        get
        {
            var user = _authService.CurrentUser;
            if (user == null) return string.Empty;
            return $"{user.LastName} {user.FirstName}";
        }
    }

    public string UserPassportNumber
    {
        get
        {
            var user = _authService.CurrentUser;
            if (user == null) return string.Empty;
            return $"Паспорт: {user.PassportNumber}";
        }
    }

    public string UserBirthDate
    {
        get
        {
            var user = _authService.CurrentUser;
            if (user == null) return string.Empty;
            return $"Дата рождения: {user.BirthDate:dd.MM.yyyy}";
        }
    }

    public string UserTicketsCount
    {
        get
        {
            var now = DateTime.Now;
            var cutoff = now.AddHours(1);
            var activeCount = MyTickets.Count(t => 
                t.Status == TicketStatus.Active && 
                t.Flight.DepartureDateTime > cutoff);
            return $"Всего активных билетов: {activeCount}";
        }
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        try
        {
            var userId = _authService.CurrentUser?.UserId ?? 0;
            var ticketsList = await _ticketService.GetUserTicketsAsync(userId);
            MyTickets.Clear();
            foreach (var ticket in ticketsList)
            {
                MyTickets.Add(ticket);
            }
            UpdateDisplayedItems();
            OnPropertyChanged(nameof(UserPassportNumber));
            OnPropertyChanged(nameof(UserBirthDate));
            OnPropertyChanged(nameof(UserTicketsCount));
        }
        catch (Exception)
        {
        }
    }

    private void UpdateDisplayedItems()
    {
        DisplayedItems.Clear();
        var now = DateTime.Now;
        var cutoff = now.AddHours(1);

        // Показываем только билеты, у которых рейс ещё не прошёл
        // и до вылета осталось больше часа
        var visibleTickets = MyTickets.Where(t => t.Flight.DepartureDateTime > cutoff);

        foreach (var ticket in visibleTickets)
        {
            DisplayedItems.Add(ticket);
        }
        OnPropertyChanged(nameof(UserTicketsCount));
    }

    [RelayCommand]
    private async Task CancelTicketById(Ticket ticket)
    {
        if (ticket != null && ticket.Status == TicketStatus.Active)
        {
            try
            {
                await _ticketService.CancelTicketAsync(ticket.TicketId);
                await LoadDataAsync();
                // Обновляем список рейсов в главном окне после отмены билета
                RefreshMainWindowFlights();
            }
            catch (Exception)
            {
            }
        }
    }

    private async void RefreshMainWindowFlights()
    {
        // Находим главное окно ClientMainView и обновляем его данные
        var mainWindow = Application.Current.Windows.OfType<ClientMainView>().FirstOrDefault();
        if (mainWindow?.DataContext is ClientMainViewModel viewModel)
        {
            await viewModel.LoadDataAsync();
            // Дополнительно обновляем FlightCard после небольшой задержки
            await System.Threading.Tasks.Task.Delay(500);
            viewModel.RefreshFlightCards();
        }
    }

    [RelayCommand]
    private void Close()
    {
        var window = System.Windows.Application.Current.Windows.OfType<PersonalCabinetView>().FirstOrDefault();
        window?.Close();
    }
}

