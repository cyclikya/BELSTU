using System;
using System.Linq;
using System.Windows;

namespace Avia
{
    public partial class RegWin : Window
    {
        public RegWin()
        {
            InitializeComponent();
            rbUser_CheckedChanged(null, null);

            txtLoginValidated.ValidationFailed += ShowError;
            txtPasswordValidated.ValidationFailed += ShowError;
            txtPasswordRepeatValidated.ValidationFailed += ShowError;
        }

        private void linkLog_Click(object sender, RoutedEventArgs e)
        {
            LogWin loginForm = new LogWin();
            loginForm.Show();
            this.Close();
        }

        private void rbUser_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (txtAdminID == null || lbAdminID == null) return;
            txtAdminID.Visibility = Visibility.Collapsed;
            lbAdminID.Visibility = Visibility.Collapsed;
        }

        private void rbAdmin_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (txtAdminID == null || lbAdminID == null) return;
            txtAdminID.Visibility = Visibility.Visible;
            lbAdminID.Visibility = Visibility.Visible;
        }

        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                string login = txtLoginValidated.Login;
                string password = txtPasswordValidated.Password;
                string role = rbAdmin.IsChecked == true ? "admin" : "client";

                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                {
                    errorMessage.Content = Application.Current.FindResource("Error1") as string; 
                    return;
                }

                if (db.Users.Any(u => u.Login == login))
                {
                    errorMessage.Content = Application.Current.FindResource("Error2") as string; 
                    return;
                }

                if (txtPasswordValidated.Password != txtPasswordRepeatValidated.Password)
                {
                    errorMessage.Content = Application.Current.FindResource("Error3") as string; 
                    return;
                }

                if (txtAdminID.Text != "1234" && role == "admin")
                {
                    errorMessage.Content = Application.Current.FindResource("Error4") as string; 
                    return;
                }

                string hashedPassword = PasswordHasher.HashPassword(password);
                var newUser = new User { Login = login, Password = hashedPassword, Role = role };
                db.Users.Add(newUser);
                db.SaveChanges();

                MainWin mainWin = new MainWin(login, role);
                mainWin.Show();
                this.Close();
            }
        }
        private void ShowError(string? message)
        {
            errorMessage.Content = message ?? "";
        }
    }
}
