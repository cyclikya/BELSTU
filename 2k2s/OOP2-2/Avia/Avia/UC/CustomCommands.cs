using System.Windows.Input;

namespace Avia.UC 
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand LoginCommand = new RoutedUICommand(
            "Войти", "LoginCommand", typeof(CustomCommands));
    }
}
