using Avia.ViewModels;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace Avia.Views;

public partial class RegistrationView : Window
{
    private bool _isPasswordVisible = false;
    private bool _isConfirmPasswordVisible = false;

    public RegistrationView()
    {
        InitializeComponent();
        // Подписываемся на событие после инициализации компонентов
        Loaded += RegistrationView_Loaded;
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

    // Ограничение ввода для паролей - запрещаем кириллицу
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

    private void RegistrationView_Loaded(object sender, RoutedEventArgs e)
    {
        // Форматируем дату после загрузки окна
        if (BirthDatePicker.SelectedDate.HasValue)
        {
            Dispatcher.BeginInvoke(new Action(() => FormatDatePickerText()), System.Windows.Threading.DispatcherPriority.Loaded);
        }
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegistrationViewModel viewModel && !_isPasswordVisible)
        {
            viewModel.Password = ((PasswordBox)sender).Password;
        }
    }

    private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegistrationViewModel viewModel && !_isConfirmPasswordVisible)
        {
            viewModel.ConfirmPassword = ((PasswordBox)sender).Password;
        }
    }

    private void PasswordBoxVisible_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is RegistrationViewModel viewModel && _isPasswordVisible)
        {
            // Синхронизируем PasswordBoxHidden с видимым текстом
            PasswordBoxHidden.Password = PasswordBoxVisible.Text;
        }
    }

    private void ConfirmPasswordBoxVisible_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is RegistrationViewModel viewModel && _isConfirmPasswordVisible)
        {
            // Синхронизируем ConfirmPasswordBoxHidden с видимым текстом
            ConfirmPasswordBoxHidden.Password = ConfirmPasswordBoxVisible.Text;
        }
    }

    private void PasswordToggle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;
        
        if (_isPasswordVisible)
        {
            // Показывать текст
            PasswordBoxVisible.Text = PasswordBoxHidden.Password;
            PasswordBoxBorder.Visibility = Visibility.Collapsed;
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
            PasswordBoxBorder.Visibility = Visibility.Visible;
            PasswordBoxHidden.Visibility = Visibility.Visible;
            PasswordBoxHidden.Focus();
        }
    }

    private void ConfirmPasswordToggle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _isConfirmPasswordVisible = !_isConfirmPasswordVisible;
        
        if (_isConfirmPasswordVisible)
        {
            // Показывать текст
            ConfirmPasswordBoxVisible.Text = ConfirmPasswordBoxHidden.Password;
            ConfirmPasswordBoxBorder.Visibility = Visibility.Collapsed;
            ConfirmPasswordBoxHidden.Visibility = Visibility.Collapsed;
            ConfirmPasswordBoxVisible.Visibility = Visibility.Visible;
            ConfirmPasswordBoxVisible.Focus();
            // Устанавливаем курсор в конец текста
            ConfirmPasswordBoxVisible.CaretIndex = ConfirmPasswordBoxVisible.Text.Length;
        }
        else
        {
            // Скрывать текст
            ConfirmPasswordBoxHidden.Password = ConfirmPasswordBoxVisible.Text;
            ConfirmPasswordBoxVisible.Visibility = Visibility.Collapsed;
            ConfirmPasswordBoxBorder.Visibility = Visibility.Visible;
            ConfirmPasswordBoxHidden.Visibility = Visibility.Visible;
            ConfirmPasswordBoxHidden.Focus();
        }
    }

    private void BirthDatePicker_Loaded(object sender, RoutedEventArgs e)
    {
        // Используем Dispatcher для отложенного выполнения после полной загрузки
        Dispatcher.BeginInvoke(new Action(() => FormatDatePickerText()), System.Windows.Threading.DispatcherPriority.Loaded);
        // Пробуем еще раз с другим приоритетом
        Dispatcher.BeginInvoke(new Action(() => FormatDatePickerText()), System.Windows.Threading.DispatcherPriority.Render);
    }

    private void BirthDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        // Используем Dispatcher для отложенного выполнения после изменения даты
        Dispatcher.BeginInvoke(new Action(() => FormatDatePickerText()), System.Windows.Threading.DispatcherPriority.Loaded);
        // Пробуем еще раз с другим приоритетом
        Dispatcher.BeginInvoke(new Action(() => FormatDatePickerText()), System.Windows.Threading.DispatcherPriority.Render);
    }

    private void BirthDatePicker_CalendarClosed(object sender, RoutedEventArgs e)
    {
        // Форматируем дату после закрытия календаря
        Dispatcher.BeginInvoke(new Action(() => FormatDatePickerText()), System.Windows.Threading.DispatcherPriority.Loaded);
        // Пробуем еще раз с другим приоритетом
        Dispatcher.BeginInvoke(new Action(() => FormatDatePickerText()), System.Windows.Threading.DispatcherPriority.Render);
    }

    private void BirthDatePicker_GotFocus(object sender, RoutedEventArgs e)
    {
        // Форматируем дату при получении фокуса
        Dispatcher.BeginInvoke(new Action(() => FormatDatePickerText()), System.Windows.Threading.DispatcherPriority.Loaded);
    }

    private void FormatDatePickerText()
    {
        if (BirthDatePicker.SelectedDate.HasValue)
        {
            // Пробуем найти TextBox через Template.FindName
            var datePickerTextBox = BirthDatePicker.Template?.FindName("PART_TextBox", BirthDatePicker) as TextBox;
            if (datePickerTextBox != null)
            {
                // Форматируем дату в формате DD.MM.YYYY
                var formattedDate = BirthDatePicker.SelectedDate.Value.ToString("dd.MM.yyyy");
                datePickerTextBox.Text = formattedDate;
                return;
            }
            
            // Если не нашли через Template, ищем в визуальном дереве
            var textBox = FindVisualChild<TextBox>(BirthDatePicker);
            if (textBox != null)
            {
                // Форматируем дату в формате DD.MM.YYYY
                var formattedDate = BirthDatePicker.SelectedDate.Value.ToString("dd.MM.yyyy");
                textBox.Text = formattedDate;
            }
            else
            {
                // Если TextBox еще не найден, пробуем еще раз через небольшое время
                Dispatcher.BeginInvoke(new Action(() => FormatDatePickerText()), 
                    System.Windows.Threading.DispatcherPriority.Input);
            }
        }
        else
        {
            // Если дата не выбрана, очищаем поле
            var datePickerTextBox = BirthDatePicker.Template?.FindName("PART_TextBox", BirthDatePicker) as TextBox;
            if (datePickerTextBox != null)
            {
                datePickerTextBox.Text = string.Empty;
                return;
            }
            
            var textBox = FindVisualChild<TextBox>(BirthDatePicker);
            if (textBox != null)
            {
                textBox.Text = string.Empty;
            }
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
}

