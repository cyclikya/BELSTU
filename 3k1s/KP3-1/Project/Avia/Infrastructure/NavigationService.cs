using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Windows;
using Avia.ViewModels;
using Avia.Views;
using Avia.Views.Admin;

namespace Avia.Infrastructure;

public class NavigationService
{
    private Window? _currentWindow;
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        var window = CreateWindowForViewModel<TViewModel>(viewModel);
        
        // Сначала устанавливаем новое окно как главное, чтобы приложение не закрывалось
        Application.Current.MainWindow = window;
        
        // Закрываем все главные окна приложения
        // Модальные диалоги (окна с Owner) не закрываем
        var windowsToClose = Application.Current.Windows.OfType<Window>()
            .Where(w => w != window && 
                       w.Owner == null && 
                       w.IsVisible &&
                       (w is LoginView || 
                        w is RegistrationView || 
                        w is AdminMainView || 
                        w is ClientMainView))
            .ToList();
        
        // Закрываем все найденные главные окна
        foreach (var win in windowsToClose)
        {
            try
            {
                // Сначала скрываем окно, чтобы оно не мешало
                win.Hide();
                // Затем закрываем
                win.Close();
            }
            catch
            {
                // Игнорируем ошибки при закрытии
            }
        }
        
        // Принудительно закрываем MainWindow, если это одно из главных окон
        if (Application.Current.MainWindow != null && 
            Application.Current.MainWindow != window &&
            Application.Current.MainWindow.Owner == null &&
            (Application.Current.MainWindow is LoginView ||
             Application.Current.MainWindow is RegistrationView ||
             Application.Current.MainWindow is AdminMainView ||
             Application.Current.MainWindow is ClientMainView))
        {
            try
            {
                Application.Current.MainWindow.Hide();
                Application.Current.MainWindow.Close();
            }
            catch
            {
            }
        }

        _currentWindow = window;
        
        window.Show();
    }

    private Window CreateWindowForViewModel<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase
    {
        return viewModel switch
        {
            LoginViewModel => new LoginView { DataContext = viewModel },
            RegistrationViewModel => new RegistrationView { DataContext = viewModel },
            AdminMainViewModel => new AdminMainView { DataContext = viewModel },
            ClientMainViewModel => new ClientMainView { DataContext = viewModel },
            AdminUserEditViewModel => new AdminUserEditView { DataContext = viewModel },
            AdminFlightEditViewModel => new AdminFlightEditView { DataContext = viewModel },
            AdminTicketEditViewModel => new AdminTicketEditView { DataContext = viewModel },
            BuyTicketViewModel => new BuyTicketView { DataContext = viewModel },
            PersonalCabinetViewModel => new PersonalCabinetView { DataContext = viewModel },
            _ => throw new InvalidOperationException($"Unknown ViewModel type: {typeof(TViewModel).Name}")
        };
    }

    public void ShowPersonalCabinet()
    {
        var viewModel = _serviceProvider.GetRequiredService<PersonalCabinetViewModel>();
        var window = new PersonalCabinetView { DataContext = viewModel };
        window.Owner = Application.Current.MainWindow;
        window.Show();
    }

    public void ShowDialog<TViewModel>() where TViewModel : ViewModelBase
    {
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        var window = CreateWindowForViewModel<TViewModel>(viewModel);
        window.ShowDialog();
    }

    public void ShowDialog<TViewModel>(Action<TViewModel> configure) where TViewModel : ViewModelBase
    {
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        configure(viewModel);
        var window = CreateWindowForViewModel(viewModel);
        window.ShowDialog();
    }

    public async Task ShowDialogAsync<TViewModel>(Func<TViewModel, Task> configure) where TViewModel : ViewModelBase
    {
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        if (configure != null)
        {
            await configure(viewModel);
        }
        var window = CreateWindowForViewModel(viewModel);
        window.ShowDialog();
    }
}

