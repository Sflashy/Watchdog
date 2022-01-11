using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Watchdog
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AppManager.UpdateItems();
            AppManager.UpdateRivens();
        }
        private void MinimizeApp(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void RivenSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var foundItems = new List<Riven>();
            foreach (var riven in AppManager.RivenList)
            {
                if(!string.IsNullOrEmpty(riven.Compatibility) && riven.Compatibility.ToLower().Contains(RivenSearch.Text.ToLower()))
                {
                    foundItems.Add(riven);
                }
            }
            if(!string.IsNullOrEmpty(RivenSearch.Text))
            {
                RivensDataGrid.ItemsSource = foundItems;
            } else
            {
                RivensDataGrid.ItemsSource = AppManager.RivenList;
            }
        }

        private void ItemSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var foundItems = new List<Item>();
            foreach (var item in AppManager.ItemList)
            {
                if (!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains(ItemSearch.Text.ToLower()))
                {
                    foundItems.Add(item);
                }
            }
            if (!string.IsNullOrEmpty(ItemSearch.Text))
            {
                ItemsDataGrid.ItemsSource = foundItems;
            }
            else
            {
                ItemsDataGrid.ItemsSource = AppManager.ItemList;
            }
        }

        private void AlwaysOnTop(object sender, RoutedEventArgs e)
        {
            Topmost = !Topmost;
        }
    }
}
