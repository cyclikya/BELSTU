using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace Avia
{
    public partial class AdminTablesWin : Window
    {
        private ObservableCollection<User> users;
        private ObservableCollection<Flight> flights;
        private ObservableCollection<Booking> bookings;

        private Stack<List<User>> usersUndoStack = new();
        private Stack<List<User>> usersRedoStack = new();

        private Stack<List<Flight>> flightsUndoStack = new();
        private Stack<List<Flight>> flightsRedoStack = new();

        private Stack<List<Booking>> bookingsUndoStack = new();
        private Stack<List<Booking>> bookingsRedoStack = new();
        public AdminTablesWin()
        {
            InitializeComponent();
            LoadAllTables();
        }

        private void LoadAllTables()
        {
            using (var db = new AppDbContext())
            {
                users = new ObservableCollection<User>(db.Users.ToList());
                flights = new ObservableCollection<Flight>(db.Flights.ToList());
                bookings = new ObservableCollection<Booking>(db.Bookings.ToList());
            }
            UsersGrid.ItemsSource = users;
            FlightsGrid.ItemsSource = flights;
            BookingsGrid.ItemsSource = bookings;

            // Очистить undo/redo при загрузке из БД
            usersUndoStack.Clear();
            usersRedoStack.Clear();
            flightsUndoStack.Clear();
            flightsRedoStack.Clear();
            bookingsUndoStack.Clear();
            bookingsRedoStack.Clear();
            SaveState(); // первая точка отмены
            UpdateUndoRedoMenu();
        }
        private void SaveState()
        {
            usersUndoStack.Push(users.Select(u => new User
            {
                Id = u.Id,
                Login = u.Login,
                Password = u.Password,
                Role = u.Role
            }).ToList());
            usersRedoStack.Clear();

            flightsUndoStack.Push(flights.Select(f => new Flight
            {
                Id = f.Id,
                Departure = f.Departure,
                Destination = f.Destination,
                Date = f.Date,
                Airline = f.Airline,
                Price = f.Price,
                SeatsTotal = f.SeatsTotal,
                SeatsAvailable = f.SeatsAvailable,
                BaggageInfo = f.BaggageInfo
            }).ToList());
            flightsRedoStack.Clear();

            bookingsUndoStack.Push(bookings.Select(b => new Booking
            {
                Id = b.Id,
                UserId = b.UserId,
                FlightId = b.FlightId,
                BookingDate = b.BookingDate,
                Status = b.Status,
                SeatsReserved = b.SeatsReserved
            }).ToList());
            bookingsRedoStack.Clear();

            UpdateUndoRedoMenu();
        }
        private void UndoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (TablesTabControl.SelectedIndex == 0 && usersUndoStack.Count > 1)
            {
                usersRedoStack.Push(users.ToList());
                usersUndoStack.Pop(); // текущий
                users = new ObservableCollection<User>(usersUndoStack.Peek().Select(u => new User
                {
                    Id = u.Id,
                    Login = u.Login,
                    Password = u.Password,
                    Role = u.Role
                }));
                UsersGrid.ItemsSource = users;
            }
            else if (TablesTabControl.SelectedIndex == 1 && flightsUndoStack.Count > 1)
            {
                flightsRedoStack.Push(flights.ToList());
                flightsUndoStack.Pop();
                flights = new ObservableCollection<Flight>(flightsUndoStack.Peek().Select(f => new Flight
                {
                    Id = f.Id,
                    Departure = f.Departure,
                    Destination = f.Destination,
                    Date = f.Date,
                    Airline = f.Airline,
                    Price = f.Price,
                    SeatsTotal = f.SeatsTotal,
                    SeatsAvailable = f.SeatsAvailable,
                    BaggageInfo = f.BaggageInfo
                }));
                FlightsGrid.ItemsSource = flights;
            }
            else if (TablesTabControl.SelectedIndex == 2 && bookingsUndoStack.Count > 1)
            {
                bookingsRedoStack.Push(bookings.ToList());
                bookingsUndoStack.Pop();
                bookings = new ObservableCollection<Booking>(bookingsUndoStack.Peek().Select(b => new Booking
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    FlightId = b.FlightId,
                    BookingDate = b.BookingDate,
                    Status = b.Status,
                    SeatsReserved = b.SeatsReserved
                }));
                BookingsGrid.ItemsSource = bookings;
            }
            UpdateUndoRedoMenu();
        }
        private void RedoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (TablesTabControl.SelectedIndex == 0 && usersRedoStack.Count > 0)
            {
                usersUndoStack.Push(usersRedoStack.Pop());
                users = new ObservableCollection<User>(usersUndoStack.Peek().Select(u => new User
                {
                    Id = u.Id,
                    Login = u.Login,
                    Password = u.Password,
                    Role = u.Role
                }));
                UsersGrid.ItemsSource = users;
            }
            else if (TablesTabControl.SelectedIndex == 1 && flightsRedoStack.Count > 0)
            {
                flightsUndoStack.Push(flightsRedoStack.Pop());
                flights = new ObservableCollection<Flight>(flightsUndoStack.Peek().Select(f => new Flight
                {
                    Id = f.Id,
                    Departure = f.Departure,
                    Destination = f.Destination,
                    Date = f.Date,
                    Airline = f.Airline,
                    Price = f.Price,
                    SeatsTotal = f.SeatsTotal,
                    SeatsAvailable = f.SeatsAvailable,
                    BaggageInfo = f.BaggageInfo
                }));
                FlightsGrid.ItemsSource = flights;
            }
            else if (TablesTabControl.SelectedIndex == 2 && bookingsRedoStack.Count > 0)
            {
                bookingsUndoStack.Push(bookingsRedoStack.Pop());
                bookings = new ObservableCollection<Booking>(bookingsUndoStack.Peek().Select(b => new Booking
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    FlightId = b.FlightId,
                    BookingDate = b.BookingDate,
                    Status = b.Status,
                    SeatsReserved = b.SeatsReserved
                }));
                BookingsGrid.ItemsSource = bookings;
            }
            UpdateUndoRedoMenu();
        }
        private void UpdateUndoRedoMenu()
        {
            undoMenuItem.IsEnabled = usersUndoStack.Count > 1 && TablesTabControl.SelectedIndex == 0 ||
                                     flightsUndoStack.Count > 1 && TablesTabControl.SelectedIndex == 1 ||
                                     bookingsUndoStack.Count > 1 && TablesTabControl.SelectedIndex == 2;

            redoMenuItem.IsEnabled = usersRedoStack.Count > 0 && TablesTabControl.SelectedIndex == 0 ||
                                     flightsRedoStack.Count > 0 && TablesTabControl.SelectedIndex == 1 ||
                                     bookingsRedoStack.Count > 0 && TablesTabControl.SelectedIndex == 2;
        }


        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var user = new User();
            var win = new EditRowWindow("Users", user, "add");
            if (win.ShowDialog() == true)
            {
                users.Add((User)win.ResultObject);
                SaveState();
            }
        }
        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User user)
            {
                var userCopy = new User
                {
                    Id = user.Id,
                    Login = user.Login,
                    Password = user.Password,
                    Role = user.Role
                };
                var win = new EditRowWindow("Users", userCopy, "edit");
                if (win.ShowDialog() == true)
                {
                    user.Login = userCopy.Login;
                    if (!string.IsNullOrEmpty(userCopy.Password) && userCopy.Password != user.Password)
                        user.Password = userCopy.Password;
                    user.Role = userCopy.Role;
                    SaveState();
                }
            }
        }
        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User user)
            {
                users.Remove(user);
                SaveState();
            }
        }
        private void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var dbUsers = db.Users.ToList();
                foreach (var dbUser in dbUsers)
                {
                    if (!users.Any(u => u.Id == dbUser.Id))
                        db.Users.Remove(dbUser);
                }

                foreach (var user in users)
                {
                    if (user.Id == 0)
                    {
                        var newUser = new User
                        {
                            Login = user.Login,
                            Password = user.Password,
                            Role = user.Role
                        };
                        db.Users.Add(newUser);
                    }
                    else
                    {
                        var dbUser = db.Users.FirstOrDefault(u => u.Id == user.Id);
                        if (dbUser != null)
                        {
                            dbUser.Login = user.Login;
                            dbUser.Password = user.Password;
                            dbUser.Role = user.Role;
                        }
                    }
                }
                db.SaveChanges();
            }
            LoadAllTables();
        }

        private void AddFlight_Click(object sender, RoutedEventArgs e)
        {
            var flight = new Flight { Date = DateTimeOffset.Now };
            var win = new EditRowWindow("Flights", flight, "add");
            if (win.ShowDialog() == true)
            {
                flights.Add((Flight)win.ResultObject);
                SaveState();
            }
        }
        private void EditFlight_Click(object sender, RoutedEventArgs e)
        {
            if (FlightsGrid.SelectedItem is Flight flight)
            {
                var flightCopy = new Flight
                {
                    Id = flight.Id,
                    Departure = flight.Departure,
                    Destination = flight.Destination,
                    Date = flight.Date,
                    Airline = flight.Airline,
                    Price = flight.Price,
                    SeatsTotal = flight.SeatsTotal,
                    SeatsAvailable = flight.SeatsAvailable,
                    BaggageInfo = flight.BaggageInfo
                };
                var win = new EditRowWindow("Flights", flightCopy, "edit");
                if (win.ShowDialog() == true)
                {
                    flight.Departure = flightCopy.Departure;
                    flight.Destination = flightCopy.Destination;
                    flight.Date = flightCopy.Date;
                    flight.Airline = flightCopy.Airline;
                    flight.Price = flightCopy.Price;
                    flight.SeatsTotal = flightCopy.SeatsTotal;
                    flight.SeatsAvailable = flightCopy.SeatsAvailable;
                    flight.BaggageInfo = flightCopy.BaggageInfo;
                    SaveState();
                }
            }
        }
        private void DeleteFlight_Click(object sender, RoutedEventArgs e)
        {
            if (FlightsGrid.SelectedItem is Flight flight)
            {
                flights.Remove(flight);
                SaveState();
            }
        }
        private void SaveFlight_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var dbFlights = db.Flights.ToList();
                foreach (var dbFlight in dbFlights)
                {
                    if (!flights.Any(f => f.Id == dbFlight.Id))
                        db.Flights.Remove(dbFlight);
                }

                foreach (var flight in flights)
                {
                    if (flight.Id == 0)
                    {
                        var newFlight = new Flight
                        {
                            Departure = flight.Departure,
                            Destination = flight.Destination,
                            Date = flight.Date,
                            Airline = flight.Airline,
                            Price = flight.Price,
                            SeatsTotal = flight.SeatsTotal,
                            SeatsAvailable = flight.SeatsAvailable,
                            BaggageInfo = flight.BaggageInfo
                        };
                        db.Flights.Add(newFlight);
                    }
                    else
                    {
                        var dbFlight = db.Flights.FirstOrDefault(f => f.Id == flight.Id);
                        if (dbFlight != null)
                        {
                            dbFlight.Departure = flight.Departure;
                            dbFlight.Destination = flight.Destination;
                            dbFlight.Date = flight.Date;
                            dbFlight.Airline = flight.Airline;
                            dbFlight.Price = flight.Price;
                            dbFlight.SeatsTotal = flight.SeatsTotal;
                            dbFlight.SeatsAvailable = flight.SeatsAvailable;
                            dbFlight.BaggageInfo = flight.BaggageInfo;
                        }
                    }
                }
                db.SaveChanges();
            }
            LoadAllTables();
        }

        private void AddBooking_Click(object sender, RoutedEventArgs e)
        {
            var booking = new Booking { BookingDate = DateTimeOffset.Now };
            var win = new EditRowWindow("Bookings", booking, "add");
            if (win.ShowDialog() == true)
            {
                bookings.Add((Booking)win.ResultObject);
                SaveState();
            }
        }
        private void EditBooking_Click(object sender, RoutedEventArgs e)
        {
            if (BookingsGrid.SelectedItem is Booking booking)
            {
                var bookingCopy = new Booking
                {
                    Id = booking.Id,
                    UserId = booking.UserId,
                    FlightId = booking.FlightId,
                    BookingDate = booking.BookingDate,
                    Status = booking.Status,
                    SeatsReserved = booking.SeatsReserved
                };
                var win = new EditRowWindow("Bookings", bookingCopy, "edit");
                if (win.ShowDialog() == true)
                {
                    booking.UserId = bookingCopy.UserId;
                    booking.FlightId = bookingCopy.FlightId;
                    booking.BookingDate = bookingCopy.BookingDate;
                    booking.Status = bookingCopy.Status;
                    booking.SeatsReserved = bookingCopy.SeatsReserved;
                    SaveState();
                }
            }
        }
        private void DeleteBooking_Click(object sender, RoutedEventArgs e)
        {
            if (BookingsGrid.SelectedItem is Booking booking)
            {
                bookings.Remove(booking);
                SaveState();
            }
        }
        private void SaveBooking_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var dbBookings = db.Bookings.ToList();
                foreach (var dbBooking in dbBookings)
                {
                    if (!bookings.Any(b => b.Id == dbBooking.Id))
                        db.Bookings.Remove(dbBooking);
                }

                foreach (var booking in bookings)
                {
                    if (booking.Id == 0)
                    {
                        var newBooking = new Booking
                        {
                            UserId = booking.UserId,
                            FlightId = booking.FlightId,
                            BookingDate = booking.BookingDate,
                            Status = booking.Status,
                            SeatsReserved = booking.SeatsReserved
                        };
                        db.Bookings.Add(newBooking);
                    }
                    else
                    {
                        var dbBooking = db.Bookings.FirstOrDefault(b => b.Id == booking.Id);
                        if (dbBooking != null)
                        {
                            dbBooking.UserId = booking.UserId;
                            dbBooking.FlightId = booking.FlightId;
                            dbBooking.BookingDate = booking.BookingDate;
                            dbBooking.Status = booking.Status;
                            dbBooking.SeatsReserved = booking.SeatsReserved;
                        }
                    }
                }
                db.SaveChanges();
            }
            LoadAllTables();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            undoMenuItem.Click += UndoMenuItem_Click;
            redoMenuItem.Click += RedoMenuItem_Click;
        }


        // фильтрация и сортировка
        private void SearchAndSortUsers_Click(object sender, RoutedEventArgs e)
        {
            string loginSearch = UserLoginSearchBox.Text?.Trim() ?? "";
            string roleSearch = (UserRoleSearchBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            using (var db = new AppDbContext())
            {
                var filtered = db.Users
                    .Where(u => u.Login.Contains(loginSearch) && (roleSearch == "" || u.Role == roleSearch))
                    .OrderBy(u => u.Login)
                    .ToList();
                users = new ObservableCollection<User>(filtered);
                UsersGrid.ItemsSource = users;
            }
        }

        private void FilterAndSortFlights_Click(object sender, RoutedEventArgs e)
        {
            string departure = FlightDepartureBox.Text?.Trim() ?? "";
            string destination = FlightDestinationBox.Text?.Trim() ?? "";

            using (var db = new AppDbContext())
            {
                var filtered = db.Flights
                    .Where(f => (departure == "" || f.Departure.Contains(departure)) &&
                                (destination == "" || f.Destination.Contains(destination)))
                    .OrderByDescending(f => f.Date)
                    .ToList();
                flights = new ObservableCollection<Flight>(filtered);
                FlightsGrid.ItemsSource = flights;
            }
        }

        //асинхрон
        private async void AsyncFlights_Click(object sender, RoutedEventArgs e)
        {
            string departure = FlightDepartureBox.Text?.Trim() ?? "";
            await LoadFlightsAsync(departure);
        }

        private async Task LoadFlightsAsync(string departure)
        {
            using (var db = new AppDbContext())
            {
                var result = await db.Flights
                    .Where(f => departure == "" || f.Departure.Contains(departure))
                    .ToListAsync(); 
                flights = new ObservableCollection<Flight>(result);
                FlightsGrid.ItemsSource = flights;
            }
        }

        //транзакция
        private void AddUserWithTransaction_Click(object sender, RoutedEventArgs e)
        {
            var newUser = new User
            {
                Login = "user" + DateTime.Now.Ticks,
                Password = PasswordHasher.HashPassword("1234"),
                Role = "client"
            };

            using (var db = new AppDbContext())
            {
                var flight = db.Flights.FirstOrDefault(f => f.SeatsAvailable > 0);
                if (flight == null)
                {
                    MessageBox.Show("Нет доступных рейсов для бронирования!");
                    return;
                }

                var newBooking = new Booking
                {
                    FlightId = flight.Id,
                    Status = "booked", 
                    SeatsReserved = 1,
                    BookingDate = DateTimeOffset.UtcNow 
                };

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Users.Add(newUser);
                        db.SaveChanges();

                        newBooking.UserId = newUser.Id;
                        db.Bookings.Add(newBooking);

                        flight.SeatsAvailable -= newBooking.SeatsReserved;

                        db.SaveChanges();

                        transaction.Commit();
                        MessageBox.Show("Пользователь и бронирование добавлены в одной транзакции!");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Ошибка! Транзакция откатилась.\n" + ex.ToString());
                    }
                }
            }
            LoadAllTables();
        }

    }
}
