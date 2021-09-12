using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MyFinanceWebApi.Tests
{
    public class HttpClientUtilities
    {
        private const string baseDomain = "http://localhost:51142/";

        internal static HttpResponseMessage GetResponse(string url, bool post, object jsonObject = null)
        {
            using (var client = new HttpClient())
            {
                string fullUrl = baseDomain + url;
                client.BaseAddress = new Uri(fullUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // New code:
                HttpResponseMessage response = post ? client.PostAsJsonAsync(fullUrl, jsonObject).Result : client.GetAsync(fullUrl).Result;
                return response;
            }
        }

        internal static T GetResponseType<T>(string url, bool post, object jsonObject)
        {
            HttpResponseMessage response = GetResponse(url, post, jsonObject);
            if (response.IsSuccessStatusCode)
            {
                T value = response.Content.ReadAsAsync<T>().Result;
                return value;
            }
            return default(T);
        }
    }
}
