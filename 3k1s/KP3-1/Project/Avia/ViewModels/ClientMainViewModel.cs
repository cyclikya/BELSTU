using Avia.Data.Entities;
using Avia.Infrastructure;
using Avia.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;

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

        LoadCompaniesAsync();
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
    private async Task LoadDataAsync()
    {
        await LoadDataInternalAsync();
    }

    public async Task LoadDataInternalAsync()
    {
        try
        {
            var flightsList = await _flightService.GetAllFlightsAsync();
            _allFlights = flightsList.ToList();
            ApplyFiltersAndUpdate();
        }
        catch (Exception)
        {
            // Handle error
        }
    }

    [RelayCommand]
    private async Task Search()
    {
        try
        {
            await SearchFlights();
        }
        catch (Exception)
        {
            // Handle error
        }
    }

    [RelayCommand]
    private async Task SearchFlights()
    {
        try
        {
            var flightsList = await _flightService.SearchFlightsAsync(DepartureCity, ArrivalCity);
            _allFlights = flightsList.ToList();
            ApplyFiltersAndUpdate();
        }
        catch (Exception)
        {
            // Handle error
        }
    }



    private bool ApplyFilters(Flight flight)
    {
        // Фильтр по дате вылета
        if (DepartureDate.HasValue && flight.DepartureDate.Date != DepartureDate.Value.Date)
        {
            return false;
        }

        // Фильтр по дате прилёта
        if (ArrivalDate.HasValue && flight.ArrivalDate.Date != ArrivalDate.Value.Date)
        {
            return false;
        }

        // Фильтр по цене
        if (!string.IsNullOrEmpty(FilterPriceFrom) && decimal.TryParse(FilterPriceFrom, out var priceFrom))
        {
            var minPrice = Math.Min(flight.EconomyPrice, flight.BusinessPrice);
            if (minPrice < priceFrom)
            {
                return false;
            }
        }

        if (!string.IsNullOrEmpty(FilterPriceTo) && decimal.TryParse(FilterPriceTo, out var priceTo))
        {
            var maxPrice = Math.Max(flight.EconomyPrice, flight.BusinessPrice);
            if (maxPrice > priceTo)
            {
                return false;
            }
        }

        // Фильтр по компании
        if (!string.IsNullOrEmpty(FilterCompany) && FilterCompany != "Все компании" && !flight.Airline.Contains(FilterCompany, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Фильтр по количеству свободных мест (минимум)
        if (!string.IsNullOrEmpty(FilterMinSeats) && int.TryParse(FilterMinSeats, out var minSeats))
        {
            var totalSeats = flight.EconomySeats + flight.BusinessSeats;
            if (totalSeats < minSeats)
            {
                return false;
            }
        }

        // Фильтр по времени суток
        if (FilterMorning && flight.DepartureTime.Hours >= 12)
        {
            return false;
        }

        if (FilterDay && (flight.DepartureTime.Hours < 12 || flight.DepartureTime.Hours >= 18))
        {
            return false;
        }

        if (FilterEvening && flight.DepartureTime.Hours < 18)
        {
            return false;
        }

        // Фильтр "Дешевле" - сортировка по возрастанию цены (применяется при сортировке)
        // Фильтр "Дороже" - сортировка по убыванию цены (применяется при сортировке)
        // Эти фильтры применяются после фильтрации, в методе UpdateDisplayedItems

        return true;
    }

    private bool ApplyTicketFilters(Ticket ticket)
    {
        // Фильтр по городу отправления
        if (!string.IsNullOrEmpty(TicketSearchDepartureCity) && 
            !ticket.Flight.DepartureCity.Contains(TicketSearchDepartureCity, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Фильтр по городу прибытия
        if (!string.IsNullOrEmpty(TicketSearchArrivalCity) && 
            !ticket.Flight.ArrivalCity.Contains(TicketSearchArrivalCity, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Фильтр по статусу
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

        // Фильтр по дате полёта
        if (TicketSearchFlightDate.HasValue && 
            ticket.Flight.DepartureDate.Date != TicketSearchFlightDate.Value.Date)
        {
            return false;
        }

        // Фильтр по классу
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

        // Фильтр по номеру билета
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
        
        // Применяем сортировку по цене, если выбраны фильтры "Дешевле" или "Дороже"
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
        OnPropertyChanged(nameof(DisplayedItems));
    }

    [RelayCommand]
    private void BuyTicket()
    {
        if (SelectedFlight != null)
        {
            _navigationService.ShowDialog<BuyTicketViewModel>(vm => vm.SetFlight(SelectedFlight));
            LoadDataAsync();
        }
    }

    [RelayCommand]
    private void BuyTicketForFlight(Flight flight)
    {
        if (flight != null)
        {
            _navigationService.ShowDialog<BuyTicketViewModel>(vm => vm.SetFlight(flight));
            LoadDataAsync();
        }
    }

    [RelayCommand]
    private async Task CancelTicket()
    {
        if (SelectedTicket != null && SelectedTicket.Status == TicketStatus.Active)
        {
            try
            {
                await _ticketService.CancelTicketAsync(SelectedTicket.TicketId);
                await LoadDataAsync();
            }
            catch (Exception)
            {
                // Handle error
            }
        }
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
            }
            catch (Exception)
            {
                // Handle error
            }
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
        UpdateDisplayedItems();
    }

    partial void OnFilterMoreExpensiveChanged(bool value)
    {
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

