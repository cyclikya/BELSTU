using labs10_11.Data;
using labs10_11.ViewModels;
using System.Windows;

namespace labs10_11
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var context = new Data.AppDbContext();
            var unitOfWork = new Data.UnitOfWork(context);
            DataContext = new ViewModels.MainViewModel(unitOfWork);
        }
    }
}
