using System;
using System.Data;

namespace DContre.MyFinance.StUtilities
{
	public static class SystemDataUtilities
	{
		public static string TrimStringList(string value, char separator)
		{
			if (string.IsNullOrEmpty(value))
			{
				return "";
			}
			value = value.Trim(separator, ',');
			return value;
		}

		public static object GetObject(DataRow dataRow, string columnName)
		{
			return dataRow == null || string.IsNullOrEmpty(columnName) || !dataRow.Table.Columns.Contains(columnName)
				? null
				: dataRow[columnName];
		}

		public static string GetString(DataRow dataRow, string columnName)
		{
			var value = GetObject(dataRow, columnName);
			return value != null ? value.ToString() : "";
		}

		public static float GetFloat(DataRow dataRow, string columnName)
		{
			var value = GetObject(dataRow, columnName);
			return StringUtilities.ConvertToFloat(value);
		}

		public static bool GetBool(DataRow dataRow, string columnName)
		{
			var value = GetObject(dataRow, columnName);
			return StringUtilities.ConvertToBool(value);
		}

		public static int GetInt(DataRow dataRow, string columnName)
		{
			var value = GetObject(dataRow, columnName);
			return StringUtilities.GetInt(value);
		}

		public static DateTime GetDateTime(DataRow dataRow, string columnName)
		{
			var value = GetObject(dataRow, columnName);
			return StringUtilities.ConvertToDateTime(value);
		}

		public static Guid GetGuid(DataRow dataRow, string columnName)
		{
			var value = GetObject(dataRow, columnName);
			return StringUtilities.ConvertToGuid(value);
		}
	}
}
