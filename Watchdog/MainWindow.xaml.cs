using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Watchdog.Data;

namespace Watchdog
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppManager.Update.Items();
            AppManager.Update.Rivens();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MinimizeApp(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AlwaysOnTop(object sender, RoutedEventArgs e)
        {
            Topmost = !Topmost;
        }

        private void ChangeWindowState(object sender, MouseButtonEventArgs e)
        {
            WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }

        private void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            ChangeWindowState(null, null);
        }

        private void ChangeFrame(object sender, RoutedEventArgs e)
        {
            if(MainFrame.Tag.ToString() == "Items")
            {
                MainFrame.Source = new Uri("Rivens.xaml", UriKind.RelativeOrAbsolute);
                MainFrame.Tag = "Rivens";
            } else
            {
                MainFrame.Source = new Uri("Items.xaml", UriKind.RelativeOrAbsolute);
                MainFrame.Tag = "Items";
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Source = new Uri("Items.xaml", UriKind.RelativeOrAbsolute);
        }
    }
}
