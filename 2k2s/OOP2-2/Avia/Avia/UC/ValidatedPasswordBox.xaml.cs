using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Avia.UC
{
    public partial class ValidatedPasswordBox : UserControl
    {
        public ValidatedPasswordBox()
        {
            InitializeComponent();
        }

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set
            {
                try
                {
                    SetValue(PasswordProperty, value);
                }
                catch (ArgumentException ex)
                {
                    ValidationFailed?.Invoke(GetValidationError(value));
                }
            }
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                "Password",
                typeof(string),
                typeof(ValidatedPasswordBox),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoercePassword),
                ValidatePassword);

        public event Action<string>? ValidationFailed;

        private static bool ValidatePassword(object value)
        {
            string s = value as string ?? "";
            if (string.IsNullOrEmpty(s)) return true;
            return s.Length >= 4 && !s.Contains(" ");
        }

        private static object CoercePassword(DependencyObject d, object baseValue)
        {
            string s = baseValue as string ?? "";
            if (s.Length > 10) return s.Substring(0, 10);
            return s;
        }

        private void PART_PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                Password = PART_PasswordBox.Password;
                ValidationFailed?.Invoke(null);
            }
            catch (ArgumentException)
            {
                ValidationFailed?.Invoke(GetValidationError(PART_PasswordBox.Password));
            }
        }

        private void PART_PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateAndNotify();
        }

        private void PART_PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ValidateAndNotify();
            }
        }

        private void ValidateAndNotify()
        {
            string s = PART_PasswordBox.Password;
            ValidationFailed?.Invoke(GetValidationError(s));
        }

        private static string? GetValidationError(string? s)
        {
            if (string.IsNullOrEmpty(s))
                return null;
            if (s.Length < 4)
                return "Пароль должен быть не менее 4 символов";
            if (s.Contains(" "))
                return "Пароль не должен содержать пробелов";
            if (s.Length > 10)
                return "Пароль не должен быть длиннее 10 символов";
            return null;
        }
    }
}
