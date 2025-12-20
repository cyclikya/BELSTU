using Avia.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Avia.Views;

public partial class LoginView : Window
{
    private bool _isPasswordVisible = false;

    public LoginView()
    {
        InitializeComponent();
    }

    private static bool IsAllowedPassportChar(char c)
    {
        // Разрешаем только цифры и латинские буквы
        return char.IsDigit(c) || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
    }

    private static string NormalizePassportText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var sb = new StringBuilder(text.Length);
        foreach (var c in text)
        {
            if (IsAllowedPassportChar(c))
            {
                sb.Append(char.ToUpperInvariant(c));
            }
        }
        return sb.ToString();
    }

    private static bool ContainsCyrillic(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        foreach (var c in text)
        {
            if ((c >= '\u0400' && c <= '\u04FF') || (c >= '\u0500' && c <= '\u052F'))
                return true;
        }
        return false;
    }

    // Ограничение ввода для поля серии и номера паспорта
    private void PassportTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Разрешаем только цифры и латиницу
        if (e.Text.Any(ch => !IsAllowedPassportChar(ch)))
        {
            e.Handled = true;
        }
        // Преобразование в верхний регистр делаем в TextChanged через NormalizePassportText
    }

    private void PassportTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            var normalized = NormalizePassportText(textBox.Text);
            if (textBox.Text != normalized)
            {
                var caretIndex = textBox.CaretIndex;
                textBox.Text = normalized;
                textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length);
            }
        }
    }

    private void PassportTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(DataFormats.Text))
        {
            var text = e.DataObject.GetData(DataFormats.Text) as string ?? string.Empty;
            var normalized = NormalizePassportText(text);
            if (string.IsNullOrEmpty(normalized))
            {
                e.CancelCommand();
            }
            else
            {
                e.DataObject = new DataObject(DataFormats.Text, normalized);
            }
        }
        else
        {
            e.CancelCommand();
        }
    }

    // Ограничение ввода для пароля - запрещаем кириллицу
    private void PasswordBoxHidden_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (ContainsCyrillic(e.Text))
        {
            e.Handled = true;
        }
    }

    private void PasswordBoxHidden_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(DataFormats.Text))
        {
            var text = e.DataObject.GetData(DataFormats.Text) as string ?? string.Empty;
            if (ContainsCyrillic(text))
            {
                e.CancelCommand();
            }
        }
        else
        {
            e.CancelCommand();
        }
    }

    private void PasswordBoxVisible_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (ContainsCyrillic(e.Text))
        {
            e.Handled = true;
        }
    }

    private void PasswordBoxVisible_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(DataFormats.Text))
        {
            var text = e.DataObject.GetData(DataFormats.Text) as string ?? string.Empty;
            if (ContainsCyrillic(text))
            {
                e.CancelCommand();
            }
        }
        else
        {
            e.CancelCommand();
        }
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

    private void PasswordToggle_MouseDown(object sender, MouseButtonEventArgs e)
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

