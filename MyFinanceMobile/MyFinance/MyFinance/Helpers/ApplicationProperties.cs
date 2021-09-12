using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;
namespace MyFinance.Helpers
{
	public static class ApplicationProperties
	{
		#region Private Attributes

		private static readonly IDictionary<string, object> Properties = Application.Current.Properties;

		#endregion

		#region Public Methods

		public static bool ContainsKey(string key)
		{
			return Application.Current.Properties.ContainsKey(key);
		}

		public static bool TryGetValue<T>(string key, out T value) where T : class
		{
			if (!Application.Current.Properties.ContainsKey(key))
			{
				value = default(T);
				return false;
			}

			value = GetValue<T>(key);
			return true;
		}

		public static T GetValue<T>(string key) where T : class
		{
			return GetAnyValue<T>(key);
		}

		public static void SaveValue(string key, object value, bool forceDiskSave = false)
		{
			SaveAnyValue(key, value, forceDiskSave);
		}

		#endregion

		#region Privates

		private static T GetAnyValue<T>(string key) where T : class
		{
			var type = typeof(T);
			if (IsPrimitiveType(type))
			{
				return Application.Current.Properties[key] as T;
			}

			var result = GetComplexValue<T>(key);
			return result;
		}

		private static T GetComplexValue<T>(string key) where T : class
		{
			var value = Application.Current.Properties[key];
			if (string.IsNullOrEmpty(value?.ToString()))
			{
				return default(T);
			}

			var decodedValue = EncodingUtilities.Base64Decode(value.ToString());
			var result = JsonConvert.DeserializeObject<T>(decodedValue);
			return result;
		}

		private static void SaveAnyValue(string key, object value, bool forceDiskSave)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (value == null)
			{
				RemoveProperty(key);
				return;
			}

			var type = value.GetType();
			if (IsPrimitiveType(type))
			{
				SaveValueInAppProperties(key, value, forceDiskSave);
				return;
			}

			var serializedValue = JsonConvert.SerializeObject(value);
			var encodedValue = EncodingUtilities.Base64Encode(serializedValue);
			SaveValueInAppProperties(key, encodedValue, forceDiskSave);
		}

		private static void RemoveProperty(string key)
		{
			if (Properties.ContainsKey(key))
			{
				Properties.Remove(key);
			}
		}

		private static void SaveValueInAppProperties(string key, object value, bool forceDiskSave)
		{
			Properties[key] = value;
			if (forceDiskSave)
			{
				Application.Current.SavePropertiesAsync().Wait();
			}
		}

		private static bool IsPrimitiveType(Type type)
		{
			return type == typeof(bool) || type == typeof(byte) || type == typeof(sbyte) || type == typeof(char) ||
				   type == typeof(decimal) || type == typeof(double) || type == typeof(float) || type == typeof(int) ||
				   type == typeof(uint) || type == typeof(long) || type == typeof(ulong) || type == typeof(short) ||
				   type == typeof(ushort) || type == typeof(string);
		}

		#endregion
	}
}
