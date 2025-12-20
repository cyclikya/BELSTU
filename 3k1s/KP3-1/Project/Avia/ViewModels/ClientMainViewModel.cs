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

public partial class ClientMainViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly NavigationService _navigationService;
    private readonly IFlightService _flightService;
    private readonly ITicketService _ticketService;


    [ObservableProperty]
    private ObservableCollection<Flight> flights = new();

    private List<Flight> _allFlights = new();

    [ObservableProperty]
    private ObservableCollection<Ticket> myTickets = new();

    [ObservableProperty]
    private ObservableCollection<object> displayedItems = new();

    [ObservableProperty]
    private Flight? selectedFlight;

    [ObservableProperty]
    private Ticket? selectedTicket;

    // Поля поиска для главной страницы
    [ObservableProperty]
    private string departureCity = string.Empty;

    [ObservableProperty]
    private string arrivalCity = string.Empty;

    [ObservableProperty]
    private DateTime? departureDate;

    [ObservableProperty]
    private DateTime? arrivalDate;

    [ObservableProperty]
    private string? selectedSeats;

    // Поля поиска для панели билетов
    [ObservableProperty]
    private string ticketSearchDepartureCity = string.Empty;

    [ObservableProperty]
    private string ticketSearchArrivalCity = string.Empty;

    [ObservableProperty]
    private string? ticketSearchStatus;

    [ObservableProperty]
    private DateTime? ticketSearchFlightDate;

    [ObservableProperty]
    private string? ticketSearchClass;

    [ObservableProperty]
    private string ticketSearchNumber = string.Empty;

    // Фильтры
    [ObservableProperty]
    private bool filterHasBaggage;

    [ObservableProperty]
    private bool filterHasHandLuggage;

    [ObservableProperty]
    private bool filterWithTransfer;

    [ObservableProperty]
    private bool filterWithoutTransfer;

    [ObservableProperty]
    private string filterPriceFrom = string.Empty;

    [ObservableProperty]
    private string filterPriceTo = string.Empty;

    [ObservableProperty]
    private string filterMinSeats = string.Empty;

    [ObservableProperty]
    private string? filterCompany;

    [ObservableProperty]
    private ObservableCollection<string> availableCompanies = new();

    [ObservableProperty]
    private bool filterCheaper;

    [ObservableProperty]
    private bool filterMoreExpensive;

    [ObservableProperty]
    private bool filterMorning;

    [ObservableProperty]
    private bool filterDay;

    [ObservableProperty]
    private bool filterEvening;

    [ObservableProperty]
    private bool hasFlights;

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
            return $"Всего билетов: {MyTickets.Count}";
        }
    }

    public IFlightService FlightService => _flightService;

    public ClientMainViewModel(IAuthService authService, NavigationService navigationService,
        IFlightService flightService, ITicketService ticketService)
    {
        _authService = authService;
        _navigationService = navigationService;
        _flightService = flightService;
        _ticketService = ticketService;

        _ = LoadCompaniesAsync();
        _ = LoadDataInternalAsync();
    }

    private async Task LoadCompaniesAsync()
    {
        try
        {
            var flightsList = await _flightService.GetAllFlightsAsync();
            var companies = flightsList.Select(f => f.Airline).Distinct().OrderBy(c => c).ToList();
            AvailableCompanies.Clear();
            AvailableCompanies.Add("Все компании");
            foreach (var company in companies)
            {
                AvailableCompanies.Add(company);
            }
            FilterCompany = "Все компании";
        }
        catch (Exception)
        {
            // Handle error
        }
    }


    [RelayCommand]
    private void NavigateToTickets()
    {
        _navigationService.ShowPersonalCabinet();
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        await LoadDataInternalAsync();
    }

    public async Task LoadDataInternalAsync()
    {
        var flightsList = await _flightService.GetAllFlightsAsync();
        _allFlights = flightsList.ToList();
        ApplyFiltersAndUpdate();
        _ = System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(async () =>
        {
            await System.Threading.Tasks.Task.Delay(300);
            RefreshFlightCards();
        }), System.Windows.Threading.DispatcherPriority.Loaded);
    }

    [RelayCommand]
    private async Task Search()
    {
        await SearchFlights();
    }

    [RelayCommand]
    private async Task SearchFlights()
    {
        var flightsList = await _flightService.SearchFlightsAsync(DepartureCity, ArrivalCity);
        _allFlights = flightsList.ToList();
        ApplyFiltersAndUpdate();
    }



    private bool ApplyFilters(Flight flight)
    {
        if (flight.DepartureDateTime < DateTime.Now)
        {
            return false;
        }

        if (DepartureDate.HasValue && flight.DepartureDate.Date != DepartureDate.Value.Date)
        {
            return false;
        }

        if (ArrivalDate.HasValue && flight.ArrivalDate.Date != ArrivalDate.Value.Date)
        {
            return false;
        }

        // Фильтр по цене: рейс подходит, если хотя бы одна из цен (эконом или бизнес) попадает в диапазон
        decimal priceFrom = 0;
        decimal priceTo = 0;
        bool hasPriceFrom = !string.IsNullOrEmpty(FilterPriceFrom) && decimal.TryParse(FilterPriceFrom, out priceFrom);
        bool hasPriceTo = !string.IsNullOrEmpty(FilterPriceTo) && decimal.TryParse(FilterPriceTo, out priceTo);

        if (hasPriceFrom || hasPriceTo)
        {
            bool economyInRange = true;
            bool businessInRange = true;

            if (hasPriceFrom)
            {
                economyInRange = flight.EconomyPrice >= priceFrom;
                businessInRange = flight.BusinessPrice >= priceFrom;
            }

            if (hasPriceTo)
            {
                economyInRange = economyInRange && flight.EconomyPrice <= priceTo;
                businessInRange = businessInRange && flight.BusinessPrice <= priceTo;
            }

            // Рейс подходит, если хотя бы одна из цен попадает в диапазон
            if (!economyInRange && !businessInRange)
            {
                return false;
            }
        }

        if (!string.IsNullOrEmpty(FilterCompany) && FilterCompany != "Все компании" && !flight.Airline.Contains(FilterCompany, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(FilterMinSeats) && int.TryParse(FilterMinSeats, out var minSeats))
        {
            var totalSeats = flight.EconomySeats + flight.BusinessSeats;
            if (totalSeats < minSeats)
            {
                return false;
            }
        }

        // Фильтр по времени суток - интервалы могут суммироваться (Утро/День/Вечер)
        if (FilterMorning || FilterDay || FilterEvening)
        {
            var hour = flight.DepartureTime.Hours;
            var matchesMorning = FilterMorning && hour < 12;
            var matchesDay = FilterDay && hour >= 12 && hour < 18;
            var matchesEvening = FilterEvening && hour >= 18;

            if (!(matchesMorning || matchesDay || matchesEvening))
            {
                return false;
            }
        }

        return true;
    }

    private bool ApplyTicketFilters(Ticket ticket)
    {
        if (!string.IsNullOrEmpty(TicketSearchDepartureCity) && 
            !ticket.Flight.DepartureCity.Contains(TicketSearchDepartureCity, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(TicketSearchArrivalCity) && 
            !ticket.Flight.ArrivalCity.Contains(TicketSearchArrivalCity, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(TicketSearchStatus))
        {
            var statusMatch = TicketSearchStatus switch
            {
                "Активен" => ticket.Status == TicketStatus.Active,
                "Отменён" => ticket.Status == TicketStatus.Cancelled,
                _ => true
            };
            if (!statusMatch)
            {
                return false;
            }
        }

        if (TicketSearchFlightDate.HasValue && 
            ticket.Flight.DepartureDate.Date != TicketSearchFlightDate.Value.Date)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(TicketSearchClass))
        {
            var classMatch = TicketSearchClass switch
            {
                "эконом" => ticket.ClassType == ClassType.Economy,
                "бизнес" => ticket.ClassType == ClassType.Business,
                _ => true
            };
            if (!classMatch)
            {
                return false;
            }
        }

        if (!string.IsNullOrEmpty(TicketSearchNumber) && 
            !ticket.TicketId.ToString().Contains(TicketSearchNumber))
        {
            return false;
        }

        return true;
    }

    private void UpdateDisplayedItems()
    {
        var flightsToDisplay = Flights.ToList();
        
        if (FilterCheaper && !FilterMoreExpensive)
        {
            flightsToDisplay = flightsToDisplay.OrderBy(f => Math.Min(f.EconomyPrice, f.BusinessPrice)).ToList();
        }
        else if (FilterMoreExpensive && !FilterCheaper)
        {
            flightsToDisplay = flightsToDisplay.OrderByDescending(f => Math.Max(f.EconomyPrice, f.BusinessPrice)).ToList();
        }
        
        DisplayedItems.Clear();
        foreach (var flight in flightsToDisplay)
        {
            DisplayedItems.Add(flight);
        }

        HasFlights = DisplayedItems.Any();
        OnPropertyChanged(nameof(DisplayedItems));
    }

    public void RefreshFlightCards()
    {
        var mainWindow = System.Windows.Application.Current.Windows.OfType<Views.ClientMainView>().FirstOrDefault();
        if (mainWindow != null)
        {
            var flightCards = FindVisualChildren<Views.FlightCard>(mainWindow).ToList();
            System.Diagnostics.Debug.WriteLine($"RefreshFlightCards: Found {flightCards.Count} FlightCard(s)");
            
            foreach (var card in flightCards)
            {
                if (card.Flight != null)
                {
                    if (card.FlightService == null)
                    {
                        card.FlightService = _flightService;
                        System.Diagnostics.Debug.WriteLine($"RefreshFlightCards: Set FlightService for flight {card.Flight.FlightId}");
                    }
                    
                    var flight = card.Flight; 
                    var flightId = flight.FlightId; 
                    _ = card.Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        System.Diagnostics.Debug.WriteLine($"RefreshFlightCards: Updating FlightCard for flight {flightId}");
                        await card.UpdateBindingsAsync(flight);
                    }), System.Windows.Threading.DispatcherPriority.Loaded);
                }
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("RefreshFlightCards: ClientMainView not found");
        }
    }

    private static IEnumerable<T> FindVisualChildren<T>(System.Windows.DependencyObject? depObj) where T : System.Windows.DependencyObject
    {
        if (depObj != null)
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                System.Windows.DependencyObject? child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
                if (child != null && child is T t)
                {
                    yield return t;
                }

                if (child != null)
                {
                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }

    [RelayCommand]
    private async Task BuyTicket()
    {
        if (SelectedFlight != null)
        {
            _navigationService.ShowDialog<BuyTicketViewModel>(vm => vm.SetFlight(SelectedFlight));
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    private async Task BuyTicketForFlight(Flight flight)
    {
        if (flight != null)
        {
            _navigationService.ShowDialog<BuyTicketViewModel>(vm => vm.SetFlight(flight));
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    private async Task CancelTicket()
    {
        if (SelectedTicket != null && SelectedTicket.Status == TicketStatus.Active)
        {
            await _ticketService.CancelTicketAsync(SelectedTicket.TicketId);
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    private async Task CancelTicketById(Ticket ticket)
    {
        if (ticket != null && ticket.Status == TicketStatus.Active)
        {
            await _ticketService.CancelTicketAsync(ticket.TicketId);
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    private void Logout()
    {
        _authService.Logout();
        _navigationService.NavigateTo<LoginViewModel>();
    }

    partial void OnFilterPriceFromChanged(string value)
    {
        ApplyFiltersAndUpdate();
    }

    partial void OnFilterPriceToChanged(string value)
    {
        ApplyFiltersAndUpdate();
    }

    partial void OnFilterMinSeatsChanged(string value)
    {
        ApplyFiltersAndUpdate();
    }

    partial void OnFilterCompanyChanged(string? value)
    {
        ApplyFiltersAndUpdate();
    }

    partial void OnFilterCheaperChanged(bool value)
    {
        // Делает чекбокс "Дешевле" взаимоисключаемым с "Дороже"
        if (value && FilterMoreExpensive)
        {
            FilterMoreExpensive = false;
        }
        UpdateDisplayedItems();
    }

    partial void OnFilterMoreExpensiveChanged(bool value)
    {
        // Делает чекбокс "Дороже" взаимоисключаемым с "Дешевле"
        if (value && FilterCheaper)
        {
            FilterCheaper = false;
        }
        UpdateDisplayedItems();
    }

    partial void OnFilterMorningChanged(bool value)
    {
        ApplyFiltersAndUpdate();
    }

    partial void OnFilterDayChanged(bool value)
    {
        ApplyFiltersAndUpdate();
    }

    partial void OnFilterEveningChanged(bool value)
    {
        ApplyFiltersAndUpdate();
    }

    private void ApplyFiltersAndUpdate()
    {
        Flights.Clear();
        var filteredFlights = _allFlights.Where(ApplyFilters).ToList();
        foreach (var flight in filteredFlights)
        {
            Flights.Add(flight);
        }
        UpdateDisplayedItems();
    }
}

