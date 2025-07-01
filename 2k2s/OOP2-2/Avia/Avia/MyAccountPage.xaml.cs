using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace Avia
{
    public partial class MyAccountPage : Page
    {
        private readonly string login;
        private string role;

        public MyAccountPage(string login, string role)
        {
            InitializeComponent();
            this.login = login;
            this.role = role;

            LoadUserBookings();
        }

        public void LoadUserBookings()
        {
            BookingsPanel.Children.Clear();

            using (var db = new AppDbContext())
            {
                var bookings = db.Bookings
                    .Include(b => b.Flight)
                    .Include(b => b.User)
                    .Where(b => b.User.Login == login && b.Status != "cancelled")
                    .OrderByDescending(b => b.BookingDate)
                    .ToList();

                foreach (var booking in bookings)
                {
                    var card = CreateBookingCard(booking, booking.Flight);
                    BookingsPanel.Children.Add(card);
                }
            }
        }

        private Border CreateBookingCard(Booking booking, Flight flight)
        {
            var border = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(0, 10, 0, 10),
                Padding = new Thickness(15),
                Background = Brushes.White,
                Width = 600
            };

            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            border.Child = grid;

            TextBlock departureDestinationTextBlock = new TextBlock
            {
                Text = $"{flight.Departure.ToUpper()} → {flight.Destination.ToUpper()}",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 10, 0)
            };
            Grid.SetColumn(departureDestinationTextBlock, 0);
            Grid.SetColumnSpan(departureDestinationTextBlock, 2);
            grid.Children.Add(departureDestinationTextBlock);

            TextBlock flightDateTextBlock = new TextBlock
            {
                Text = $"{flight.Date:dd.MM.yyyy HH:mm}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkSlateGray,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
            };
            Grid.SetColumn(flightDateTextBlock, 2);
            grid.Children.Add(flightDateTextBlock);

            TextBlock statusTextBlock = new TextBlock
            {
                Text = $"{Application.Current.FindResource("Status")}: {booking.Status}",
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Grid.SetRow(statusTextBlock, 1);
            Grid.SetColumn(statusTextBlock, 0);
            grid.Children.Add(statusTextBlock);

            TextBlock seatsReservedTextBlock = new TextBlock
            {
                Text = $"{Application.Current.FindResource("SeatsReserved")}: {booking.SeatsReserved}",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Grid.SetRow(seatsReservedTextBlock, 1);
            Grid.SetColumn(seatsReservedTextBlock, 1);
            grid.Children.Add(seatsReservedTextBlock);

            StackPanel buttonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            Grid.SetRow(buttonsPanel, 2);
            Grid.SetColumn(buttonsPanel, 2);
            grid.Children.Add(buttonsPanel);

            Button cancelButton = new Button
            {
                Content = Application.Current.FindResource("CancelBooking"),
                Background = Brushes.Green,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 10, 0, 0),
                BorderBrush = Brushes.Transparent,
                Cursor = Cursors.Hand
            };
            cancelButton.Command = new CancelBookingCommand(booking, flight, this);
            buttonsPanel.Children.Add(cancelButton);

            Button payButton = new Button
            {
                Content = Application.Current.FindResource("PayBooking"),
                Background = Brushes.Green,
                Foreground = Brushes.White,
                Margin = new Thickness(5, 10, 0, 0),
                BorderBrush = Brushes.Transparent,
                Cursor = Cursors.Hand,
                Visibility = booking.Status == "booked" ? Visibility.Visible : Visibility.Collapsed
            };
            payButton.Command = new PayBookingCommand(booking, flight, this);
            buttonsPanel.Children.Add(payButton);

            return border;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPg(login, role));
        }
    }

    public class CancelBookingCommand : ICommand
    {
        private readonly Booking _booking;
        private readonly Flight _flight;
        private readonly MyAccountPage _page;

        public CancelBookingCommand(Booking booking, Flight flight, MyAccountPage page)
        {
            _booking = booking;
            _flight = flight;
            _page = page;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            using (var db = new AppDbContext())
            {
                var booking = db.Bookings.FirstOrDefault(b => b.Id == _booking.Id);
                if (booking != null)
                {
                    booking.Status = "cancelled";
                    db.Bookings.Update(booking);

                    db.SaveChanges();
                    _page.LoadUserBookings();
                    MessageBox.Show("cancelled");
                }
            }
        }

        public event EventHandler? CanExecuteChanged;
    }

    public class PayBookingCommand : ICommand
    {
        private readonly Booking _booking;
        private readonly Flight _flight;
        private readonly MyAccountPage _page;

        public PayBookingCommand(Booking booking, Flight flight, MyAccountPage page)
        {
            _booking = booking;
            _flight = flight;
            _page = page;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            using (var db = new AppDbContext())
            {
                var booking = db.Bookings.FirstOrDefault(b => b.Id == _booking.Id);
                if (booking != null)
                {
                    booking.Status = "paid";
                    db.Bookings.Update(booking);
                    db.SaveChanges();
                    _page.LoadUserBookings();
                    MessageBox.Show("paid");
                }
            }
        }

        public event EventHandler? CanExecuteChanged;
    }
}