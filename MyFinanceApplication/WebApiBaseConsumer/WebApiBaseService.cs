using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MyFinanceModel;
using Newtonsoft.Json;

namespace WebApiBaseConsumer
{
	public abstract class WebApiBaseService
	{
		protected WebApiBaseService()
		{
			ValidateModelState = true;
			ValidateServiceException = true;
		}

		private bool _responseInProcess;

		[DefaultValue(true)]
		protected bool ValidateModelState { get; set; }

		[DefaultValue(true)]
		protected bool ValidateServiceException { get; set; }

		protected abstract string GetApiBaseDomain();

		#region Private Methods

		private string GetBaseDomain()
		{
			var apiDomain = GetApiBaseDomain();
			apiDomain = apiDomain.Replace("/api/", "/");
			return apiDomain;
		}

		private HttpRequestMessage CreateHttpRequestMessage(WebApiRequest webApiRequest)
		{
			if (webApiRequest == null)
			{
				throw new ArgumentNullException(nameof(webApiRequest));
			}

			var url = webApiRequest.UseControllerBaseUrl ? (GetApiBaseDomain() + webApiRequest.Url) : webApiRequest.Url;
			var request = new HttpRequestMessage(webApiRequest.Method, url);
			if (!string.IsNullOrEmpty(webApiRequest.AccessToken))
			{
				request.Headers.Add("Authorization", new[] { "Bearer " + webApiRequest.AccessToken });
			}

			if (webApiRequest.Model != null)
			{
				if (webApiRequest.IsJsonModel)
				{
					var jsonFormatter = new System.Net.Http.Formatting.JsonMediaTypeFormatter
					{
						SerializerSettings = new JsonSerializerSettings
						{
							TypeNameHandling = TypeNameHandling.None,
							Formatting = Formatting.Indented,
							NullValueHandling = NullValueHandling.Ignore
						}
					};
					request.Content = new ObjectContent(webApiRequest.Model.GetType(), webApiRequest.Model, jsonFormatter);
				}
				else
				{
					var content = (HttpContent)webApiRequest.Model;
					request.Content = content;
				}
			}

			if (webApiRequest.Headers != null && webApiRequest.Headers.Any())
			{
				foreach (var header in webApiRequest.Headers)
				{
					request.Headers.Add(header.Key, header.Value);
				}
			}

			return request;
		}

		#endregion

		#region Protected

		protected bool UserIdValid(string userId)
		{
			return Guid.TryParse(userId, out _);
		}

		protected abstract string ControllerName { get; }

		protected async Task<HttpResponseMessage> GetResponseAsync(WebApiRequest webApiRequest)
		{
			var request = CreateHttpRequestMessage(webApiRequest);
			using (var client = new HttpClient())
			{
				try
				{
					var response = await client.SendAsync(request);
					if (webApiRequest.ProcessResponse)
					{
						await ProcessResponseAsync(response);
					}

					return response;
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					throw;
				}
			}
		}

		protected async Task<T> GetResponseAsAsync<T>(WebApiRequest webApiRequest)
		{
			var response = await GetResponseAsync(webApiRequest);
			try
			{
				var contentResponse = await response.Content.ReadAsAsync<T>();
				return contentResponse;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		protected HttpResponseMessage GetResponse(WebApiRequest webApiRequest)
		{
			var request = CreateHttpRequestMessage(webApiRequest);
			using (var client = new HttpClient())
			{
				try
				{
					var response = client.SendAsync(request).Result;
					if (webApiRequest.ProcessResponse)
					{
						var result = ProcessResponseAsync(response);
						result.Wait();
					}

					return response;
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					throw;
				}
			}
		}

		protected T GetResponseAs<T>(WebApiRequest webApiRequest)
		{
			var response = GetResponse(webApiRequest);
			var result = response.Content.ReadAsAsync<T>().Result;
			return result;
		}

		#endregion

		#region Url Helpers

		protected string GetBaseUrl(string method)
		{
			return GetBaseDomain() + method;
		}

		protected string CreateCustomUrl(string customValues, Dictionary<string, object> parameters = null)
		{
			var paramValues = "";
			if (parameters != null && parameters.Count > 0)
			{
				paramValues += "?";
				paramValues += UrlCreatorHelper.ToUrlObjects(parameters);
			}

			return $"{ControllerName}/{customValues}{paramValues}";
		}

		protected string CreateRootUrl(Dictionary<string, object> parameters = null)
		{
			var urlParams = "";
			if (parameters != null && parameters.Any())
			{
				urlParams = $"?{UrlCreatorHelper.ToUrlObjects(parameters)}";
			}

			return $"{ControllerName}{urlParams}";
		}

		protected string CreateMethodUrl(string method = "", Dictionary<string, object> parameters = null)
		{
			var parametersValues = UrlCreatorHelper.ToUrlObjects(parameters);
			const string separator = "?";
			return
				$"{ControllerName}/{method}{(!string.IsNullOrEmpty(parametersValues) ? separator : "")}{parametersValues}";
		}

		#endregion

		#region Response process

		protected async Task ProcessResponseAsync(HttpResponseMessage response)
		{
			try
			{
				if (_responseInProcess)
				{
					return;
				}

				_responseInProcess = true;
				if (response == null)
				{
					throw new ArgumentNullException(nameof(response));
				}

				if (response.IsSuccessStatusCode)
				{
					return;
				}

				if (ValidateModelState)
				{
					HandleModelStateException(response);
				}

				if (ValidateServiceException)
				{
					await HandleServiceExceptionAsync(response);
				}
			}
			finally
			{
				_responseInProcess = false;
			}
		}

		protected async Task HandleServiceExceptionAsync(HttpResponseMessage response)
		{
			if (response == null)
			{
				throw new ArgumentNullException(nameof(response));
			}

			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				var content = await response.Content.ReadAsStringAsync();
				Console.WriteLine(content);
				throw new BackendTokenException();
			}

			if (!response.Headers.TryGetValues("ErrorContentType", out var values) || !values.Any())
			{
				return;
			}

			var headerValue = values.First();
			int errorCode;
			errorCode = int.TryParse(headerValue, out errorCode) ? errorCode : 0;
			if (!ServiceException.IsServiceExceptionErrorCodeValid(errorCode))
			{
				return;
			}

			var exception = await CreateServiceExceptionAsync(response.Content);
			throw exception;
		}

		private static async Task<ServiceException> CreateServiceExceptionAsync(HttpContent content)
		{
			var exception = await content.ReadAsAsync<ServiceException>();
			return exception;
		}

		protected void HandleModelStateException(HttpResponseMessage response)
		{
			if (response == null)
			{
				throw new ArgumentNullException(nameof(response));
			}

			if (!response.Headers.TryGetValues("ErrorContentType", out var values) || !values.Any())
			{
				return;
			}

			var headerValue = values.First();
			int errorCode;
			errorCode = int.TryParse(headerValue, out errorCode) ? errorCode : 0;
			if (!ModelStateException.IsModelStateErrorCodeValid(errorCode))
			{
				return;
			}

			var exception = response.Content.ReadAsAsync<ModelStateException>().Result;
			throw exception;
		}

		#endregion

		#region Protected Utils

		protected string ToUrlComplexObject(string paramName, object value)
		{
			if (string.IsNullOrEmpty(paramName))
			{
				throw new ArgumentNullException(nameof(paramName));
			}

			if (value == null)
			{
				return "";
			}

			var properties = value.GetType().GetProperties();
			var result = "";
			var first = true;
			foreach (var property in properties)
			{
				if (!first)
				{
					result += "&";
				}
				else
				{
					first = false;
				}

				result += $"{paramName}.{property.Name}={property.GetValue(value, null)}";
			}

			return result;
		}

		#endregion
	}
}
