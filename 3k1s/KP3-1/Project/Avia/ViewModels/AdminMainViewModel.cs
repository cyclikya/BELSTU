using Avia.Data.Entities;
using Avia.Infrastructure;
using Avia.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Avia.ViewModels;

public partial class AdminMainViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private string selectedTab = "Users";

    [ObservableProperty]
    private ObservableCollection<User> users = new();

    [ObservableProperty]
    private ObservableCollection<Flight> flights = new();

    [ObservableProperty]
    private ObservableCollection<Ticket> tickets = new();

    [ObservableProperty]
    private User? selectedUser;

    [ObservableProperty]
    private Flight? selectedFlight;

    [ObservableProperty]
    private Ticket? selectedTicket;

    [ObservableProperty]
    private string searchTerm = string.Empty;

    public AdminMainViewModel(IAuthService authService, NavigationService navigationService,
        IUserService userService, IFlightService flightService, ITicketService ticketService)
    {
        _authService = authService;
        _navigationService = navigationService;
        _userService = userService;
        _flightService = flightService;
        _ticketService = ticketService;

        LoadDataAsync();
    }

    private readonly IUserService _userService;
    private readonly IFlightService _flightService;
    private readonly ITicketService _ticketService;

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        try
        {
            if (SelectedTab == "Users")
            {
                var usersList = await _userService.GetAllUsersAsync();
                Users.Clear();
                foreach (var user in usersList)
                {
                    Users.Add(user);
                }
            }
            else if (SelectedTab == "Flights")
            {
                var flightsList = await _flightService.GetAllFlightsAsync();
                Flights.Clear();
                foreach (var flight in flightsList)
                {
                    Flights.Add(flight);
                }
            }
            else if (SelectedTab == "Tickets")
            {
                var ticketsList = await _ticketService.GetAllTicketsAsync();
                Tickets.Clear();
                foreach (var ticket in ticketsList)
                {
                    Tickets.Add(ticket);
                }
            }
        }
        catch (Exception ex)
        {
            // Handle error
        }
    }

    [RelayCommand]
    private void Logout()
    {
        _authService.Logout();
        _navigationService.NavigateTo<LoginViewModel>();
    }

    [RelayCommand]
    private void CreateUser()
    {
        _navigationService.ShowDialog<AdminUserEditViewModel>(vm => { });
        LoadDataAsync();
    }

    [RelayCommand]
    private void EditUser()
    {
        if (SelectedUser != null)
        {
            _navigationService.ShowDialog<AdminUserEditViewModel>(vm => vm.SetUser(SelectedUser));
            LoadDataAsync();
        }
    }

    [RelayCommand]
    private async Task DeleteUser()
    {
        if (SelectedUser != null)
        {
            try
            {
                await _userService.DeleteUserAsync(SelectedUser.UserId);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                // Handle error
            }
        }
    }

    [RelayCommand]
    private void CreateFlight()
    {
        _navigationService.ShowDialog<AdminFlightEditViewModel>(vm => { });
        LoadDataAsync();
    }

    [RelayCommand]
    private void EditFlight()
    {
        if (SelectedFlight != null)
        {
            _navigationService.ShowDialog<AdminFlightEditViewModel>(vm => vm.SetFlight(SelectedFlight));
            LoadDataAsync();
        }
    }

    [RelayCommand]
    private async Task DeleteFlight()
    {
        if (SelectedFlight != null)
        {
            try
            {
                await _flightService.DeleteFlightAsync(SelectedFlight.FlightId);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                // Handle error
            }
        }
    }

    [RelayCommand]
    private async Task CreateTicket()
    {
        await _navigationService.ShowDialogAsync<AdminTicketEditViewModel>(vm => Task.CompletedTask);
        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task EditTicket()
    {
        if (SelectedTicket != null)
        {
            await _navigationService.ShowDialogAsync<AdminTicketEditViewModel>(vm => vm.SetTicketAsync(SelectedTicket));
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    private async Task DeleteTicket()
    {
        if (SelectedTicket != null)
        {
            try
            {
                await _ticketService.DeleteTicketAsync(SelectedTicket.TicketId);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                // Handle error
            }
        }
    }

    partial void OnSelectedTabChanged(string value)
    {
        LoadDataAsync();
    }
}

