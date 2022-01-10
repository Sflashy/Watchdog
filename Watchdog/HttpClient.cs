using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace Watchdog
{
    public static class HttpClient
    {
        public enum RequestType
        {
            Api,
            Body
        }

        public static async Task<dynamic> Request(string url, RequestType requestType)
        {
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0");
                var response = await httpClient.GetAsync(url);
                if (response.RequestMessage.RequestUri.AbsoluteUri == "https://warframe.market/error/404") return null;
                while(response.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    await Task.Delay(5000);
                    response = await httpClient.GetAsync(url);

                }
                switch (requestType)
                {
                    case RequestType.Api:
                        return JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                    case RequestType.Body:
                        return response.Content.ReadAsStringAsync().Result;
                    default:
                        return null;
                }
            }
        }
    }
}
