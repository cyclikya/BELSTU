using Avia.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Avia.Views;

public partial class AdminMainView : Window
{
    public AdminMainView()
    {
        InitializeComponent();
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is AdminMainViewModel viewModel && sender is TabControl tabControl)
        {
            if (tabControl.SelectedItem is TabItem selectedTab)
            {
                var tag = selectedTab.Tag?.ToString();
                if (!string.IsNullOrEmpty(tag))
                {
                    viewModel.SelectedTab = tag;
                }
            }
        }
    }
}

