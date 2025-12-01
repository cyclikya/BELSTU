using Avia.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Avia.Views;

public partial class LoginView : Window
{
    private bool _isPasswordVisible = false;

    public LoginView()
    {
        InitializeComponent();
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel viewModel && !_isPasswordVisible)
        {
            viewModel.Password = ((PasswordBox)sender).Password;
        }
    }

    private void PasswordBoxVisible_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is LoginViewModel viewModel && _isPasswordVisible)
        {
            // Синхронизируем PasswordBoxHidden с видимым текстом
            PasswordBoxHidden.Password = PasswordBoxVisible.Text;
        }
    }

    private void PasswordToggle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;
        
        if (_isPasswordVisible)
        {
            // Показывать текст
            PasswordBoxVisible.Text = PasswordBoxHidden.Password;
            PasswordBoxHidden.Visibility = Visibility.Collapsed;
            PasswordBoxVisible.Visibility = Visibility.Visible;
            PasswordBoxVisible.Focus();
            // Устанавливаем курсор в конец текста
            PasswordBoxVisible.CaretIndex = PasswordBoxVisible.Text.Length;
        }
        else
        {
            // Скрывать текст
            PasswordBoxHidden.Password = PasswordBoxVisible.Text;
            PasswordBoxVisible.Visibility = Visibility.Collapsed;
            PasswordBoxHidden.Visibility = Visibility.Visible;
            PasswordBoxHidden.Focus();
        }
    }
}

