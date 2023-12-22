using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace WebApiBaseConsumer.Models
{
	internal static class CustomSerializationInfoExtensions
	{
		public static bool ExistsKey(this SerializationInfo info, string key, bool ignoreCase = false)
		{
			foreach (var serializationEntry in info)
			{
				var comparison = ignoreCase
					? StringComparison.InvariantCultureIgnoreCase
					: StringComparison.InvariantCulture;
				if (string.Compare(serializationEntry.Name, key, comparison) == 0)
				{
					return true;
				}
			}

			return false;
		}

		internal static SerializationEntry? GetSerializationEntry(this SerializationInfo info, string key,
			bool ignoreCase = false)
		{
			foreach (var serializationEntry in info)
			{
				var comparison = ignoreCase
					? StringComparison.InvariantCultureIgnoreCase
					: StringComparison.InvariantCulture;
				if (string.Compare(serializationEntry.Name, key, comparison) == 0)
				{
					return serializationEntry;
				}
			}

			return null;
		}
	}
}
