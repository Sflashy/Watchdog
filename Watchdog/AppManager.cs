using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Watchdog
{
    public static class AppManager
    {
        private static MainWindow MainWindow = (MainWindow)Application.Current.MainWindow;
        public static List<Item> ItemList = new List<Item>();
        public static List<Riven> RivenList = new List<Riven>();
        private static List<string> duplicate = new List<string>();
        public static async Task<dynamic> Request(string url)
        {
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0");
                var response = await httpClient.GetAsync(url);
                if (response.RequestMessage.RequestUri.AbsoluteUri == "https://warframe.market/error/404") return null;
                while (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    await Task.Delay(5000);
                    response = await httpClient.GetAsync(url);
                }

                try
                {
                    return JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                }
                catch (Exception)
                {

                    return null;
                }
            }
        }

        public static async void UpdateItems()
        {
            ItemList.Clear();
            MainWindow.ItemsDataGrid.Visibility = Visibility.Collapsed;
            MainWindow.LoadingGrid.Visibility = Visibility.Visible;
            var data = await AppManager.Request("https://drops.warframestat.us/data/relics.json");
            foreach (var relic in data.relics)
            {
                if (relic.state != "Intact") continue;
                foreach (var reward in relic.rewards)
                {
                    var itemName = reward.itemName.ToString();
                    if (InBlacklist(itemName)) continue;
                    if (reward.rarity != "Rare") continue;
                    if (duplicate.Contains(itemName)) continue;
                    duplicate.Add(itemName);
                    var item = new Item();
                    item.Name = itemName;
                    var sellData = await GetItemSellData(itemName);
                    if(sellData == null) continue;
                    item.Date = sellData.datetime;
                    item.Volume = sellData.volume;
                    item.Min = sellData.min_price;
                    item.Max = sellData.max_price;
                    item.Avg = sellData.avg_price;
                    ItemList.Add(item);
                }
            }
            MainWindow.ItemsDataGrid.ItemsSource = ItemList;
            MainWindow.LoadingGrid.Visibility = Visibility.Collapsed;
            MainWindow.ItemsDataGrid.Visibility = Visibility.Visible;
        }

        public static async void UpdateRivens()
        {
            var datas = await AppManager.Request("http://n9e5v4d8.ssl.hwcdn.net/repos/weeklyRivensPC.json");
            if (datas == null) return;
            foreach (var data in datas)
            {
                var riven = new Riven();
                riven.ItemType = data.itemType;
                riven.Compatibility = data.compatibility;
                riven.Rerolled = data.rerolled;
                riven.Avg = data.avg;
                riven.Stddev = data.stddev;
                riven.Min = data.min;
                riven.Max = data.max;
                riven.Popularity = data.pop;
                RivenList.Add(riven);
            }
            MainWindow.RivensDataGrid.ItemsSource = RivenList;
        }

        private static bool InBlacklist(string itemName)
        {
            if (File.Exists("blacklist.txt"))
            {
                return File.ReadAllText("blacklist.txt").Contains(itemName);
            }
            return false;
        }

        public static async Task<dynamic> GetItemSellData(string itemName)
        {
            itemName = itemName.Replace(" ", "_").ToLower().Replace("&", "and");
            if (Regex.Match(itemName, @"chassis|system|neuroptics|wings").Success) itemName = itemName.Replace("_blueprint", "");
            dynamic data = await AppManager.Request($"https://api.warframe.market/v1/items/{itemName}/statistics");
            if (data == null) return null;
            data = data.payload.statistics_live["48hours"][data.payload.statistics_live["48hours"].Count - 1];
            return data;
        }
    }
}
