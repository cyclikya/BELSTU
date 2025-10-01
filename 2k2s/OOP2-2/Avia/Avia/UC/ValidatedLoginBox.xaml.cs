using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Avia.UC
{
    public partial class ValidatedLoginBox : UserControl
    {
        public ValidatedLoginBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LoginProperty =
            DependencyProperty.Register(
                "Login",
                typeof(string),
                typeof(ValidatedLoginBox),
                new FrameworkPropertyMetadata(
                    "",
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnLoginChanged,
                    CoerceLogin),
                ValidateLogin);

        public string Login
        {
            get => (string)GetValue(LoginProperty);
            set => SetValue(LoginProperty, value);
        }

        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register(
                "IsValid",
                typeof(bool),
                typeof(ValidatedLoginBox),
                new PropertyMetadata(true));

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }

        public event Action<string>? ValidationFailed;

        private static bool ValidateLogin(object value)
        {
            string s = value as string ?? "";
            if (string.IsNullOrEmpty(s)) return true;
            return s.All(c => char.IsLetterOrDigit(c));
        }

        private static object CoerceLogin(DependencyObject d, object baseValue)
        {
            string s = baseValue as string ?? "";
            if (s.Length > 10) return s.Substring(0, 10);
            return s;
        }

        private static void OnLoginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ValidatedLoginBox;
            var login = e.NewValue as string ?? "";
            var error = GetValidationError(login);
            control.IsValid = error == null;
            control.ValidationFailed?.Invoke(error);

            if (control.PART_TextBox.Text != login)
            {
                control.PART_TextBox.Text = login;
                control.PART_TextBox.CaretIndex = login.Length;
            }
        }

        private void PART_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateAndNotify();
        }

        private void PART_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ValidateAndNotify();
            }
        }

        private void ValidateAndNotify()
        {
            string s = PART_TextBox.Text;
            var error = GetValidationError(s);
            IsValid = error == null;
            ValidationFailed?.Invoke(error);
        }

        private static string? GetValidationError(string? s)
        {
            if (string.IsNullOrEmpty(s))
                return null;
            if (s.Length < 4)
                return "Логин должен быть не менее 4 символов";
            if (!s.All(c => char.IsLetterOrDigit(c)))
                return "Логин должен содержать только латинские буквы и цифры";
            return null;
        }
    }
}
