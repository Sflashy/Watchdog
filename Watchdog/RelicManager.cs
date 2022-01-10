using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Watchdog
{
    public static class RelicManager
    {
        public static async Task<dynamic> GetRelics()
        {
            return await HttpClient.Request("https://drops.warframestat.us/data/relics.json", HttpClient.RequestType.Api);
        }

        public static async Task<int> GetItemPrice(string itemName)
        {
            itemName = itemName.Replace(" ", "_").ToLower().Replace("&", "and");
            if(Regex.Match(itemName, @"chassis|system|neuroptics|wings").Success) itemName = itemName.Replace("_blueprint", "");
            var highestPrice = 0;
            dynamic data = await HttpClient.Request($"https://api.warframe.market/v1/items/{itemName}/statistics", HttpClient.RequestType.Api);
            if (data == null) return 0;
            foreach (var item in data.payload.statistics_live["48hours"])
            {
                if(item.min_price > highestPrice) highestPrice = item.min_price;
            }
            return highestPrice;
        }
        //https://api.warframe.market/v1/items/{item[i]}/statistics
    }
}
