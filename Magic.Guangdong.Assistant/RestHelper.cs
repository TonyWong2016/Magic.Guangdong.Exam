using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant
{
    public class RestHelper
    {
        public static async Task<string> Get(RestParams restParams)
        {
            try
            {
                var client = new HttpClient();
                //var request = new HttpRequestMessage(HttpMethod.Get, "https://www.xiaoxiaotong.org/api/Article/GetRecord?editdatetime=" + DateTime.Now.AddDays(-30));
                if (restParams.UrlParms.Any())
                {
                    restParams.Url += "?";
                    foreach (var item in restParams.UrlParms)
                    {
                        restParams.Url += item.Key + "=" + item.Value + "&";
                    }
                }
                var request = new HttpRequestMessage(HttpMethod.Get, restParams.Url);

                if (restParams.Headers.Any())
                {
                    foreach (var item in restParams.Headers)
                    {
                        request.Headers.Add(item.Key, item.Value);
                    }
                }
                //request.Headers.Add("Token", "gdjx#7W5ojU");
                CancellationTokenSource cts = new CancellationTokenSource(10000);

                var response = await client.SendAsync(request, cts.Token);
                response.EnsureSuccessStatusCode();
                string responseStr = await response.Content.ReadAsStringAsync();
                if (responseStr.Contains("\\\"") && responseStr.StartsWith("\"{\\\""))
                    responseStr = responseStr.Substring(1, responseStr.Length - 2).Replace("\\\"", "\"");

                return responseStr;
            }
            catch(Exception ex)
            {
                Assistant.Logger.Error(ex.Message);
                return ex.Message;
            }
        }
    }

    public class RestParams
    {
        public string Url { get; set; }
        
        public Dictionary<string,string> UrlParms {  get; set; }

        public Dictionary<string,string> Headers { get; set; }
    }

    
}
