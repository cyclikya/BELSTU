using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Avia
{
    public partial class BookingWin : Window
    {
        private Flight flight;
        private string login;
        private MainPg mainPage;

        public BookingWin(Flight flight, string login, MainPg mainPage) 
        {
            InitializeComponent();
            this.flight = flight;
            this.login = login;
            this.mainPage = mainPage;

            if (flight != null)
            {
                FlightInfoTextBlock.Text = $"{flight.Date.ToLocalTime():dd.MM.yyyy HH:mm}";
                DepartureTextBlock.Text = flight.Departure;
                DestinationTextBlock.Text = flight.Destination;
                UpdateTotalPrice();
            }
        }

        private void UpdateTotalPrice()
        {
            if (flight != null)
            {
                if (int.TryParse(SeatsTextBox.Text, out int seats) && seats > 0)
                {
                    decimal totalPrice = flight.Price * seats;
                    TotalPriceTextBlock.Text = $"{totalPrice}$" ;
                }
            }
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessBooking("booked");
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessBooking("paid");
        }

        private void ProcessBooking(string status)
        {
            if (!int.TryParse(SeatsTextBox.Text, out int seats) || seats <= 0)
            {
                ErrorMessageTextBlock.Text = FindResource("EnterValidSeatCount") as string;
                return;
            }

            using (var db = new AppDbContext())
            {
                var currentFlight = db.Flights.FirstOrDefault(f => f.Id == flight.Id);
                if (currentFlight == null)
                {
                    ErrorMessageTextBlock.Text = FindResource("FlightNotFound") as string;
                    return;
                }

                if (seats > currentFlight.SeatsAvailable)
                {
                    ErrorMessageTextBlock.Text = string.Format(
                        FindResource("OnlySeatsAvailable") as string,
                        currentFlight.SeatsAvailable);
                    return;
                }

                var user = db.Users.FirstOrDefault(u => u.Login == login);
                if (user == null)
                {
                    ErrorMessageTextBlock.Text = FindResource("UserNotFound") as string;
                    return;
                }

                var booking = new Booking
                {
                    UserId = user.Id,
                    FlightId = currentFlight.Id,
                    BookingDate = DateTime.UtcNow,
                    Status = status,
                    SeatsReserved = seats
                };

                try
                {
                    db.Bookings.Add(booking);
                    db.SaveChanges();
                    mainPage.UpdateFlightsPanel();


                    MessageBox.Show(string.Format(
                        FindResource("BookingSuccess") as string,
                        status == "booked" ? "забронированы" : "куплены"));

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка бронирования: {ex.Message}");
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SeatsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotalPrice();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            mainPage.UpdateFlightsPanel();
        }

    }
}
