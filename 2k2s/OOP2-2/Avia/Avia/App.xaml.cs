using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace Avia
{
    public partial class App : Application
    {
        public App() { }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LanguageManager.ChangeLanguage("ru-RU");
        }
    }
    public static class CustomCommands
    {
        public static readonly RoutedUICommand ClearFields = new RoutedUICommand(
            "Clear Fields",
            "ClearFields",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.C, ModifierKeys.Control)
            }
        );
    }
    public static class LanguageManager
    {
        private static string _currentLanguage = "ru-RU";

        public static string CurrentLanguage => _currentLanguage;

        public static void ChangeLanguage(string newLanguage)
        {
            var dict = new ResourceDictionary
            {
                Source = new Uri($"/Avia;component/Resources/lang.{newLanguage}.xaml", UriKind.Relative)
            };

            var oldDict = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source?.OriginalString.Contains("lang.") == true);

            if (oldDict != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(oldDict);
            }

            Application.Current.Resources.MergedDictionaries.Add(dict);

            foreach (Window window in Application.Current.Windows)
            {
                if (window != null)
                {
                    window.Language = XmlLanguage.GetLanguage(newLanguage);
                }
            }

            _currentLanguage = newLanguage;
        }
    }
}
