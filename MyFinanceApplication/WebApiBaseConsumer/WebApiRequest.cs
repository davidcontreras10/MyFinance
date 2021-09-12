using System.Collections.Generic;
using System.Net.Http;

namespace WebApiBaseConsumer
{
	public class WebApiRequest
	{
		public WebApiRequest(string url, HttpMethod httpMethod)
		{
			Url = url;
			Method = httpMethod;
		}

		public WebApiRequest(string url, HttpMethod httpMethod, string accessToken) : this(url, httpMethod)
		{
			AccessToken = accessToken;
		}

		public string AccessToken { get; set; }
		public string Url { get; set; }
		public HttpMethod Method { get; set; }
		public object Model { get; set; }
		public bool IsJsonModel { get; set; } = true;
		public bool ProcessResponse { get; set; } = true;
		public bool UseControllerBaseUrl { get; set; } = true;
		public Dictionary<string, string> Headers { get; set; }
	}
}
