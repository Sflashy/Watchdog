using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Watchdog
{
    public partial class MainWindow : Window
    {
        private static List<Item> Items = new List<Item>();
        private static List<string> duplicate = new List<string>();
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
            Update();
        }
        public async void Update()
        {
            UpdateBegin();
            var data = await RelicManager.GetRelics();
            foreach (var relic in data.relics)
            {
                if (relic.state != "Intact") continue;
                foreach (var reward in relic.rewards)
                {
                    var itemName = reward.itemName.ToString();
                    if(File.Exists("blacklist.txt"))
                    {
                        if (File.ReadAllText("blacklist.txt").Contains(itemName)) continue;
                    }
                    if (reward.rarity != "Rare") continue;
                    if (duplicate.Contains(itemName)) continue;
                    duplicate.Add(itemName);
                    var item = new Item();
                    item.Name = itemName;
                    item.Price = await RelicManager.GetItemPrice(itemName);
                    Items.Add(item);
                }
            }
            UpdateEnd();
        }

        private void UpdateEnd()
        {
            DataGrid.ItemsSource = Items;
            DataGrid.Visibility = Visibility.Visible;
            Progressbar.Visibility = Visibility.Collapsed;
            RefreshButton.IsEnabled = true;
        }

        private void UpdateBegin()
        {
            duplicate.Clear();
            Items.Clear();
            RefreshButton.IsEnabled = false;
            DataGrid.Visibility = Visibility.Collapsed;
            Progressbar.Visibility = Visibility.Visible;
        }
        private void ToggleTopMost(object sender, MouseButtonEventArgs e)
        {
            Topmost = !Topmost;
        }

        private void MinimizeApp(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            Update();
        }
    }
}
