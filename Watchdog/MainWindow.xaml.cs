using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            var data = await HttpClient.Request("https://drops.warframestat.us/data/relics.json");
            Progressbar.IsIndeterminate = false;
            Progressbar.Value = 0;
            Progressbar.Maximum = data.relics.Count;
            foreach (var relic in data.relics)
            {
                Progressbar.Value++;
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
                    item.Price = await GetItemPrice(itemName);
                    Items.Add(item);
                }
            }
            UpdateEnd();
        }

        private void UpdateEnd()
        {
            DataGrid.ItemsSource = Items;
            DataGrid.Visibility = Visibility.Visible;
            UpdateGrid.Visibility = Visibility.Collapsed;
        }

        private void UpdateBegin()
        {
            duplicate.Clear();
            Items.Clear();
            DataGrid.Visibility = Visibility.Collapsed;
            UpdateGrid.Visibility = Visibility.Visible;
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


        public static async Task<int> GetItemPrice(string itemName)
        {
            itemName = itemName.Replace(" ", "_").ToLower().Replace("&", "and");
            if (Regex.Match(itemName, @"chassis|system|neuroptics|wings").Success) itemName = itemName.Replace("_blueprint", "");
            var highestPrice = 0;
            dynamic data = await HttpClient.Request($"https://api.warframe.market/v1/items/{itemName}/statistics");
            if (data == null) return 0;
            foreach (var item in data.payload.statistics_live["48hours"])
            {
                if (item.min_price > highestPrice) highestPrice = item.min_price;
            }
            return highestPrice;
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var foundItems = new List<Item>();
            foreach (var item in Items)
            {
                if (item.Name.ToLower().Contains(Search.Text.ToLower()))
                {
                    foundItems.Add(item);
                }
            }
            DataGrid.ItemsSource = foundItems;
        }

        private void KeyEvents(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5) Update();
        }
    }
}
