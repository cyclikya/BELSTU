using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Avia
{
    public partial class LogWin : Window
    {
        public LogWin()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(
                UC.CustomCommands.LoginCommand,
                LoginCommand_Executed,
                LoginCommand_CanExecute));
        }

        private void btnLinkReg_Click(object sender, RoutedEventArgs e)
        {
            RegWin regWin = new RegWin();
            regWin.Show();
            this.Close();
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                string login = txtLogin.Text;
                string password = txtPassword.Password;
                var user = db.Users.FirstOrDefault(u => u.Login == login);

                if (user != null && PasswordHasher.VerifyPassword(password, user.Password))
                {
                    MainWin mainWin = new MainWin(user.Login, user.Role);
                    mainWin.Show();
                    this.Close();
                }
                else
                {
                    errorMessage.Content = Application.Current.FindResource("ErrorLog") as string;
                }
            }
        }
        private void btnChangeLanguage_Click(object sender, RoutedEventArgs e)
        {
            string newLanguage = LanguageManager.CurrentLanguage == "ru-RU" ? "en-US" : "ru-RU";
            LanguageManager.ChangeLanguage(newLanguage);
        }

        // --- Tunneling
        private void txtLogin_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txtLogin.Background = Brushes.Yellow;
        }
        // --- Bubbling
        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (var child in mainStackPanel.Children)
            {
                if (child is Button btn)
                {
                    btn.Background = Brushes.LightSkyBlue;
                }
            }
        }
        // --- Direct
        private async void txtPassword_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            txtPassword.BorderBrush = Brushes.Red;
            txtPassword.BorderThickness = new Thickness(3);
            await Task.Delay(1500);
            txtPassword.ClearValue(PasswordBox.BorderBrushProperty);
            txtPassword.ClearValue(PasswordBox.BorderThicknessProperty);
        }

        private void btnResetStyles_Click(object sender, RoutedEventArgs e)
        {
            txtLogin.ClearValue(TextBox.BackgroundProperty);
            txtPassword.ClearValue(PasswordBox.BorderBrushProperty);
            txtPassword.ClearValue(PasswordBox.BorderThicknessProperty);

            foreach (var child in mainStackPanel.Children)
            {
                if (child is Button btn)
                {
                    btn.ClearValue(Button.BackgroundProperty);
                }
            }
        }

        private void LoginCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btnLogin_Click(sender, e); 
        }
        private void LoginCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrWhiteSpace(txtLogin.Text) && !string.IsNullOrWhiteSpace(txtPassword.Password);
        }

        private void txtLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }
        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
