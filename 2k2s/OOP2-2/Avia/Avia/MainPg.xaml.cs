using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Avia;

namespace Avia
{
    public partial class MainPg : Page
    {
        private List<Flight> allFlights;
        private string login;
        private string role;

        public MainPg(string login, string role)
        {
            InitializeComponent();
            this.login = login;
            this.role = role;

            using (var db = new AppDbContext())
            {
                var distinctDepartureCities = db.Flights.Select(f => f.Departure).Distinct().ToList();
                var distinctDestinationCities = db.Flights.Select(f => f.Destination).Distinct().ToList();

                DepartureCityComboBox.ItemsSource = distinctDepartureCities;
                DestinationCityComboBox.ItemsSource = distinctDestinationCities;
            }

            allFlights = LoadAllFlights(); 
            UpdateFlightsPanel(); 
        }

        private List<Flight> LoadAllFlights()
        {
            using (var db = new AppDbContext())
            {
                return db.Flights.ToList();
            }
        }

        public void UpdateFlightsPanel()
        {
            FlightsPanel.Children.Clear();

            Func<Flight, bool> departureFilter = f =>
                DepartureCityComboBox.SelectedItem == null ||
                f.Departure == DepartureCityComboBox.SelectedItem.ToString();

            Func<Flight, bool> destinationFilter = f =>
                DestinationCityComboBox.SelectedItem == null ||
                f.Destination == DestinationCityComboBox.SelectedItem.ToString();

            Func<Flight, bool> seatsFilter = f =>
                !int.TryParse(RequiredSeatsTextBox.Text, out int requiredSeats) ||
                f.SeatsAvailable >= requiredSeats;

            Func<Flight, bool> dateFilter = f =>
                FlightDatePicker.SelectedDate == null ||
                f.Date.Date == FlightDatePicker.SelectedDate.Value.Date;

            var filteredFlights = allFlights
                .Where(departureFilter)
                .Where(destinationFilter)
                .Where(seatsFilter)
                .Where(dateFilter)
                .ToList();

            string priceLabel = $"{this.FindResource("Price") as string}" as string;
            string baggageLabel = $"{this.FindResource("Baggage") as string}" as string;
            string seatsLabel = $"{this.FindResource("Seats") as string}" as string;

            if (SortByComboBox.SelectedItem is ComboBoxItem sortByItem)
            {
                string selected = sortByItem.Content.ToString();

                if (selected == priceLabel)
                {
                    filteredFlights = filteredFlights.OrderBy(f => f.Price).ToList();
                }
                else if (selected == baggageLabel)
                {
                    filteredFlights = filteredFlights.OrderBy(f => f.BaggageInfo).ToList();
                }
                else if (selected == seatsLabel)
                {
                    filteredFlights = filteredFlights.OrderBy(f => f.SeatsAvailable).ToList();
                }
            }


            foreach (var flight in filteredFlights)
            {
                var border = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(10),
                    Margin = new Thickness(0, 10, 0, 10),
                    Padding = new Thickness(15),
                    Background = Brushes.White,
                    Child = new Grid
                    {
                        RowDefinitions =
                        {
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto }
                        },
                        Children =
                        {
                            new DockPanel
                            {
                                LastChildFill = false,
                                Margin = new Thickness(0, 0, 0, 10),
                                Children =
                                {
                                    new TextBlock
                                    {
                                        Text = $"✈ {flight.Departure.ToUpper()} → {flight.Destination.ToUpper()}",
                                        FontSize = 18,
                                        FontWeight = FontWeights.Bold,
                                        VerticalAlignment = VerticalAlignment.Top
                                    },
                                    new TextBlock
                                    {
                                        Text = $"{flight.Date:dd.MM.yyyy HH:mm}",
                                        FontSize = 16,
                                        FontWeight = FontWeights.Bold,
                                        Foreground = Brushes.DarkSlateGray,
                                        HorizontalAlignment = HorizontalAlignment.Right,
                                        Margin = new Thickness(20, 0, 0, 0),
                                        VerticalAlignment = VerticalAlignment.Top
                                    }
                                }
                            },
                            new StackPanel
                            {
                                Margin = new Thickness(0, 60, 0, 10),
                                Children =
                                {
                                    new TextBlock
                                    {
                                        Inlines = {
                                            new Run(this.FindResource("Company") as string + ": ") { FontWeight = FontWeights.Bold },
                                            new Run(flight.Airline)
                                        }
                                    },
                                    new TextBlock
                                    {
                                        Inlines = {
                                            new Run(this.FindResource("Seats") as string + ": ") { FontWeight = FontWeights.Bold },
                                            new Run(this.FindResource("Total") as string + " " + flight.SeatsTotal + ", " + this.FindResource("Avalble") as string + " " +  flight.SeatsAvailable)
                                        }
                                    },
                                    new TextBlock
                                    {
                                        Inlines = {
                                            new Run(this.FindResource("Baggage") as string + ": ") { FontWeight = FontWeights.Bold },
                                            new Run(flight.BaggageInfo)
                                        }
                                    }
                                }
                            },
                            new StackPanel
                            {
                                Orientation = Orientation.Horizontal,
                                HorizontalAlignment = HorizontalAlignment.Right,
                                Children =
                                {
                                    CreateBuyButton(flight) 
                                }
                            },
                            new TextBlock
                            {
                                Text = this.FindResource("Price") as string + " " + flight.Price + "$",
                                FontSize = 16,
                                FontWeight = FontWeights.Bold,
                                Foreground = Brushes.Green,
                                HorizontalAlignment = HorizontalAlignment.Right,
                                VerticalAlignment = VerticalAlignment.Bottom
                            }
                        }
                    }
                };
                FlightsPanel.Children.Add(border);
            }

        }
        private Button CreateBuyButton(Flight flight)
        {
            var button = new Button
            {
                Content = this.FindResource("Buy") as string,
                Width = 100,
                Height = 30,
                Background = (Brush)new BrushConverter().ConvertFromString("#244226"),
                Foreground = Brushes.White,
                Margin = new Thickness(0, 10, 0, 60),
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            button.Click += (sender, e) =>
            {
                var bookingWin = new BookingWin(flight, login, this);
                bookingWin.Show();
            };

            return button;
        }
        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            DepartureCityComboBox.SelectedItem = null;
            DestinationCityComboBox.SelectedItem = null;
            RequiredSeatsTextBox.Text = string.Empty;
            FlightDatePicker.SelectedDate = null;
            SortByComboBox.SelectedItem = null;
            OnFiltersChanged();
        }
        private void SwapCitiesButton_Click(object sender, RoutedEventArgs e)
        {
            string temp = DepartureCityComboBox.Text;
            DepartureCityComboBox.Text = DestinationCityComboBox.Text;
            DestinationCityComboBox.Text = temp;
        }
        private void DepartureCityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnFiltersChanged();
        }
        private void DestinationCityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnFiltersChanged();
        }
        private void RequiredSeatsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnFiltersChanged();
        }
        private void FlightDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            OnFiltersChanged();
        }
        private void SortByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnFiltersChanged();
        }
        private void OnFiltersChanged()
        {
            UpdateFlightsPanel();
        }
    }
}
