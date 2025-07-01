using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Avia
{
    public partial class MainWin : Window
    {
        public string CurrentLogin { get; private set; }
        public string CurrentRole { get; private set; }
        public ActionHistory History { get; } = new ActionHistory();


        public MainWin(string login, string role)
        {
            InitializeComponent();
            CurrentLogin = login;
            CurrentRole = role;

            txtFooter.Text = Application.Current.FindResource("CurrentUser") as string
                            + login
                            + Application.Current.FindResource("Author") as string;

            if (role != "admin")
                adminToolsMenu.Visibility = Visibility.Collapsed;

            NavigateToMainPage();
            this.DataContext = this;
        }

        public void NavigateToMainPage()
        {
            mainFrame.Navigate(new MainPg(CurrentLogin, CurrentRole));
            txtNameFrame.Text = Application.Current.FindResource("Flights") as string;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void adminToolsMenu_Click(object sender, RoutedEventArgs e)
        {
            var adminWin = new AdminTablesWin();
            adminWin.ShowDialog();
        }

        private void accountMenu_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new MyAccountPage(CurrentLogin, CurrentRole));
            txtNameFrame.Text = Application.Current.FindResource("Account") as string + ":\n" + Application.Current.FindResource("Welcom") + ", " + CurrentLogin;
        }
    }
}