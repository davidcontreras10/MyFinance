using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WebApiBaseConsumer
{
	public static class UrlCreatorHelper
	{
		public static string ToUrlObject(string paramName, object value)
		{
			if (string.IsNullOrEmpty(paramName))
			{
				throw new ArgumentNullException(nameof(paramName));
			}

			if (value == null)
			{
				return "";
			}

			if (IsSimpleObject(value))
			{
				return ToUrlSimpleObject(paramName, value);
			}

			if (IsCollection(value))
			{
				return ToUrlCollectionObject(paramName, value);
			}

			if (IsComplexObject(value))
			{
				return ToUrlComplexObject(paramName, value);
			}

			if (IsDictionaryObject(value))
			{
				return ToUrlDictionaryObject(paramName, value);
			}

			throw new ObjectTypeNotSupportedException(value.GetType());
		}

		public static string ToUrlObjects(Dictionary<string, object> parameters)
		{
			if (parameters == null || !parameters.Any())
			{
				return null;
			}

			var first = true;
			var result = "";
			foreach (var parameter in parameters)
			{
				if (!first)
				{
					result += "&";
				}
				else
				{
					first = false;
				}

				result += ToUrlObject(parameter.Key, parameter.Value);
			}

			return result;
		}

		private static string ToUrlComplexObject(string paramName, object value)
		{
			if (string.IsNullOrEmpty(paramName))
			{
				throw new ArgumentNullException(nameof(paramName));
			}

			if (value == null)
			{
				return "";
			}

			if (IsSimpleObject(value) || IsCollection(value)) //TODO check for class decorated with custom attribute
			{
				throw new InvalidObjectTypeException(value.GetType());
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

		private static string ToUrlSimpleObject(string paramName, object value)
		{
			if (string.IsNullOrEmpty(paramName))
			{
				throw new ArgumentNullException(nameof(paramName));
			}

			if (value == null)
			{
				return "";
			}

			if (!IsSimpleObject(value)) //TODO check for class decorated with custom attribute
			{
				throw new InvalidObjectTypeException(value.GetType());
			}

			if (value is string s && s == string.Empty)
			{
				return string.Empty;
			}

			if (value is DateTime)
			{
				value = ToUrlDateTimeValue((DateTime)value);
			}

			return $"{paramName}={value}";
		}

		private static string ToUrlCollectionObject(string paramName, object value)
		{
			var ienumerable = (IEnumerable)value;
			var result = "";
			var first = true;
			var count = 0;
			foreach (var item in ienumerable)
			{
				if (!first)
				{
					result += "&";
				}
				else
				{
					first = false;
				}

				var collectionParamName = $"{paramName}[{count}]";
				result += ToUrlObject(collectionParamName, item);
				count++;
			}

			return result;
		}

		private static string ToUrlDictionaryObject(string paramName, object value)
		{
			var dictionary = (IDictionary)value;
			var first = true;
			var result = "";
			foreach (DictionaryEntry dictionaryEntry in dictionary)
			{
				if (!first)
				{
					result += "&";
				}
				else
				{
					first = false;
				}

				var dictionaryParamName = $"{paramName}.{dictionaryEntry.Key}";
				result += ToUrlObject(dictionaryParamName, dictionaryEntry.Value);
			}

			return result;
		}

		private static string ToUrlDateTimeValue(DateTime dateTime)
		{
			var result = $"{dateTime.Year}-{dateTime.Month}-{dateTime.Day} {dateTime.Hour}:{dateTime.Minute}";
			return result;
		}

		private static bool IsDictionaryObject(object value)
		{
			return value is IDictionary;
		}

		private static bool IsSimpleObject(object value)
		{
			if (value == null)
			{
				return false;
			}

			return value.GetType().IsValueType || value is string;
		}

		private static bool IsCollection(object value)
		{
			if (value == null)
			{
				return false;
			}

			return value is IEnumerable && !(value is IDictionary);
		}

		private static bool IsComplexObject(object value)
		{
			return !IsSimpleObject(value) && !IsCollection(value);
		}
	}
}
