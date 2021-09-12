using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyFinanceWebApi.QueryHeaders
{
    public static class QueryProcessor
	{
		private const string REMOVE_QUERY_REGEX = @"^[a-zA-Z0-9-]*[a-zA-Z0-9\[\]]+(?:.[a-zA-Z0-9-]*[a-zA-Z0-9\[\]]+)*$";
		private const string OBJECT_PATH_REGEX =
			@"^[a-zA-Z0-9_]+(\.[a-zA-Z0-9_]+)*$";
		

		public enum QueryActionTtype
		{
			Unknown = 0,
			Remove = 1
		}

		public static JToken ProcessQuery(object o, string query, QueryActionTtype actionTtype)
		{
			if (o == null || actionTtype == QueryActionTtype.Unknown || string.IsNullOrEmpty(query))
			{
				return null;
			}           

            if(actionTtype == QueryActionTtype.Remove)
            {
                var removeResult = ProcessRemoveQuery(o, query);
                return removeResult;
            }

            throw new NotImplementedException();
		}

		private static JToken ProcessFilterQuery(object o, string query)
		{
			if (o == null || string.IsNullOrEmpty(query))
			{
				return null;
			}

			var queryRequests = query.Split('&');
			throw new NotImplementedException();
		}

		private static JToken ProcessRemoveQuery(object o, string query)
		{
			if(o == null || string.IsNullOrEmpty(query))
			{
				return null;
			}

			var queryRequests = query.Split('&');
            var isObjectList = IsListObjectType(o);
			var jObject = isObjectList ? JObject.FromObject(new InternalObjectContainer(o)) : JObject.FromObject(o);
			foreach(var queryRequest in queryRequests)
			{
				if (RemoveQueryValid(queryRequest))
				{
					var queryPathArray = queryRequest.Split('.');
					ProcessRemoveQuery(jObject, queryPathArray);
				}
			}

            return isObjectList ? jObject["CustomObject"] : jObject;
		}

		private static bool RemoveQueryValid(string query)
		{
			if (string.IsNullOrEmpty(query))
			{
				return false;
			}

			return Regex.IsMatch(query, REMOVE_QUERY_REGEX);
		}

		private static bool FullRemoveQueryValid(string query)
		{
			var queryPaths = query.Split('.');
			if (!queryPaths.Any())
			{
				return false;
			}

			return queryPaths.All(path => Regex.IsMatch(path, REMOVE_QUERY_REGEX));
		}	

		private static void ProcessRemoveQuery(JToken jToken, IEnumerable<string> queryArray, int currentLevel = 0)
		{
			var currentQueryParameter = queryArray.ElementAt(currentLevel);
			var isList = currentQueryParameter.Contains("[]");
			currentQueryParameter = currentQueryParameter.Replace("[]", "");
            if (string.IsNullOrEmpty(currentQueryParameter))
            {
                currentQueryParameter = nameof(InternalObjectContainer.CustomObject);
            }

			if (jToken[currentQueryParameter] == null)
			{
				return;
			}

			if (queryArray.Count() - 1 == currentLevel)
			{
				var removeToken = jToken[currentQueryParameter];
				removeToken.Parent.Remove();
				return;
			}

			var nextLevel = currentLevel + 1;
			if (isList)
			{
                var jTokenEnum = string.IsNullOrEmpty(currentQueryParameter) ? jToken.Children() : jToken[currentQueryParameter].AsJEnumerable();
				foreach (var item in jTokenEnum)
				{
					ProcessRemoveQuery(item, queryArray, nextLevel);
				}

				return;
			}

			ProcessRemoveQuery(jToken[currentQueryParameter], queryArray, nextLevel);
		}

        private static bool IsListObjectType(object o)
        {
            return o is IList || o is IEnumerable || o is ICollection;
        }

        private class InternalObjectContainer
        {
            public InternalObjectContainer() { }

            public InternalObjectContainer(object o)
            {
                CustomObject = o;
            }

            public object CustomObject { get; set; }
        }
	}
}