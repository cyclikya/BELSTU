using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Avia.Views;

public partial class BuyTicketView : Window
{
    public BuyTicketView()
    {
        InitializeComponent();
    }

    private void NumberOnlyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Разрешаем только цифры
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }
}

