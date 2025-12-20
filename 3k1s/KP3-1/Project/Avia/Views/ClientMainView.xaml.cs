using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Avia.Views;

public partial class ClientMainView : Window
{
    public ClientMainView()
    {
        InitializeComponent();
        Loaded += ClientMainView_Loaded;
    }

    private void ClientMainView_Loaded(object sender, RoutedEventArgs e)
    {
        var datePickers = FindVisualChildren<DatePicker>(this);
        foreach (var datePicker in datePickers)
        {
            datePicker.SelectedDateChanged += DatePicker_SelectedDateChanged;
            datePicker.CalendarClosed += DatePicker_CalendarClosed;
            datePicker.Loaded += DatePicker_Loaded;
        }

        if (DataContext is ViewModels.ClientMainViewModel viewModel)
        {
            _ = viewModel.LoadDataInternalAsync();
        }
    }

    private void DatePicker_Loaded(object sender, RoutedEventArgs e)
    {
        FormatDatePickerText((DatePicker)sender);
    }

    private void DatePicker_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        FormatDatePickerText((DatePicker)sender);
    }

    private void DatePicker_CalendarClosed(object sender, RoutedEventArgs e)
    {
        FormatDatePickerText((DatePicker)sender);
    }

    private void FormatDatePickerText(DatePicker datePicker)
    {
        if (datePicker.SelectedDate.HasValue)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var textBox = FindVisualChild<TextBox>(datePicker);
                if (textBox != null)
                {
                    textBox.Text = datePicker.SelectedDate.Value.ToString("dd.MM.yyyy");
                }
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }
    }

    private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T result)
            {
                return result;
            }
            var childOfChild = FindVisualChild<T>(child);
            if (childOfChild != null)
            {
                return childOfChild;
            }
        }
        return null;
    }

    private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T result)
            {
                yield return result;
            }
            foreach (var childOfChild in FindVisualChildren<T>(child))
            {
                yield return childOfChild;
            }
        }
    }

    private void NumberOnlyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }
}

