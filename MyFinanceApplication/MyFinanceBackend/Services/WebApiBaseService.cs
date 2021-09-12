using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace MyFinanceBackend.Services
{
    public abstract class WebApiBaseService
    {
        protected abstract string GetApiBaseDomain();

        protected abstract string ControllerName();

        protected HttpResponseMessage GetResponse(string url, bool post, object jsonObject = null)
        {
            using (var client = new HttpClient())
            {
                var fullUrl = GetApiBaseDomain() + url;
                client.BaseAddress = new Uri(fullUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = post ? client.PostAsJsonAsync(fullUrl, jsonObject).Result : client.GetAsync(fullUrl).Result;
                return response;
            }
        }

        protected T GetResponseType<T>(string url, bool post, object jsonObject)
        {
            HttpResponseMessage response = GetResponse(url, post, jsonObject);
            if (response.IsSuccessStatusCode)
            {
                T value = response.Content.ReadAsAsync<T>().Result;
                return value;
            }
            throw new HttpResponseException(response);
        }

        private string CreateParametersUrl(Dictionary<string, object> value)
        {
            bool first = true;
            string url = "";
            foreach (string key in value.Keys)
            {
                if (!first)
                {
                    url += "&";
                }
                else
                {
                    first = false;
                }
                url += $"{key}={value[key]}";
            }
            return url;
        }

        protected string CreateMethodUrl(string method, Dictionary<string, object> parameters)
        {
            return $"{ControllerName()}/{method}?{CreateParametersUrl(parameters)}";
        }

        protected string CreateMethodUrl(string method)
        {
            return $"{ControllerName()}/{method}";
        }
    }
}
