using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Watchdog.Data;

namespace Watchdog
{
    [SuppressMessage("ReSharper", "AsyncVoidLambda")]
    [SuppressMessage("ReSharper", "FunctionNeverReturns")]
    public static class AppManager
    {
        private static readonly MainWindow MainWindow = (MainWindow)Application.Current.MainWindow;
        public static readonly List<Item> ItemList = new List<Item>();
        public static readonly List<Riven> RivenList = new List<Riven>();
        private static readonly List<string> Duplicate = new List<string>();

        private static async Task<dynamic> Request(string url)
        {
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0");
                    var response = await httpClient.GetAsync(url);
                    if (response.RequestMessage.RequestUri.AbsoluteUri == "https://warframe.market/error/404") return null;
                    while (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        await Task.Delay(5000);
                        response = await httpClient.GetAsync(url);
                    }

                    return JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                }
                catch (Exception)
                {
                    MainWindow.UpdateIcon.Visibility = Visibility.Collapsed;
                    MainWindow.NoInternetIcon.Visibility = Visibility.Visible;
                    return null;
                }

            }
        }

        private static async Task LoadRelicItems()
        {
            Duplicate.Clear();
            ItemList.Clear();
            var data = await AppManager.Request("https://drops.warframestat.us/data/relics.json");
            foreach (var relic in data.relics)
            {
                if (relic.state != "Intact" || relic.tier == "Requiem") continue;
                foreach (var reward in relic.rewards)
                {
                    var itemName = reward.itemName.ToString();
                    if (InBlacklist(itemName)) continue;
                    if (reward.rarity != "Rare") continue;
                    if (Duplicate.Contains(itemName)) continue;
                    Duplicate.Add(itemName);
                    ItemList.Add(new Item { Name = itemName });
                }
            }
            LoadWhitelistItems();
        }

        private static void LoadWhitelistItems()
        {
            if (File.Exists("whitelist.txt"))
            {
                foreach (var item in File.ReadAllLines("whitelist.txt"))
                {
                    ItemList.Add(new Item { Name = item });
                }
            }
            Items.Instance.ItemsDataGrid.Items.Refresh();

        }

        private static bool InBlacklist(string itemName)
        {
            return File.Exists("blacklist.txt") && File.ReadAllText("blacklist.txt").Contains(itemName);
        }

        private static async Task<dynamic> GetItemMarketData(string itemName)
        {
            itemName = itemName.Replace(" ", "_").ToLower().Replace("&", "and");
            if (Regex.Match(itemName, @"chassis|system|neuroptics|wings").Success) itemName = itemName.Replace("_blueprint", "");
            dynamic data = await AppManager.Request($"https://api.warframe.market/v1/items/{itemName}/statistics");
            if (data == null) return null;
            data = data.payload.statistics_live["48hours"][data.payload.statistics_live["48hours"].Count - 1];
            return data;
        }


        public static class Update
        {
            public static async void Items()
            {
                await LoadRelicItems();
                Application.Current.Dispatcher.Invoke((Action)async delegate
                {
                    while (true)
                    {
                        AppManager.MainWindow.UpdateIcon.Visibility = Visibility.Visible;
                        foreach (var item in ItemList)
                        {
                            var marketData = await GetItemMarketData(item.Name);
                            if (marketData == null) continue;
                            item.Date = marketData.datetime;
                            item.Volume = marketData.volume;
                            item.Min = marketData.min_price;
                            item.Max = marketData.max_price;
                            item.Avg = marketData.avg_price;
                        }
                        AppManager.MainWindow.UpdateIcon.Visibility = Visibility.Collapsed;
                        await Task.Delay(new TimeSpan(0, 5, 0));
                    }
                });

            }

            public static async void Rivens()
            {
                var rivens = await AppManager.Request("http://n9e5v4d8.ssl.hwcdn.net/repos/weeklyRivensPC.json");
                if (rivens == null) return;
                foreach (var riven in rivens)
                {
                    RivenList.Add(new Riven
                    {
                        ItemType = riven.itemType,
                        Compatibility = riven.compatibility,
                        Rerolled = riven.rerolled,
                        Avg = riven.avg,
                        Stddev = riven.stddev,
                        Min = riven.min,
                        Max = riven.max,
                        Popularity = riven.pop
                    });
                }
            }

        }
    }
}
