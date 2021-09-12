using System;
using System.Data;

namespace Utilities
{
    public static class DataRowConvert
    {
        #region Publics

        public static T CustomConvert<T>(this DataRow dataRow, string columnName, Func<string, T> simpleParseFunction,
            Func<DataRow,string, T> forceParseFunction, ParseBehaviorOption parseBehaviorOption = ParseBehaviorOption.DefaultValue)
        {
            if (parseBehaviorOption == ParseBehaviorOption.DefaultValue)
            {
                if (forceParseFunction == null)
                    throw new ArgumentException("Must specify simpleParseFunction for DefaultValue option",
                        nameof(simpleParseFunction));
                return forceParseFunction(dataRow, columnName);
            }
            if (parseBehaviorOption == ParseBehaviorOption.ThrowException)
            {
                if (simpleParseFunction == null)
                    throw new ArgumentException("Must specify forceParseFunction for ThrowException option",
                        nameof(simpleParseFunction));
                EvalThrowException(dataRow, columnName);
                var value = dataRow[columnName];
                if (value is T)
                    return (T) value;
                return simpleParseFunction(value.ToString());
            }
            throw new ArgumentException("Option not supported", nameof(parseBehaviorOption));
        }

        public static Guid ToGuid(this DataRow dataRow, string columnName,
            ParseBehaviorOption parseBehaviorOption = ParseBehaviorOption.DefaultValue)
        {
            if (parseBehaviorOption == ParseBehaviorOption.DefaultValue)
            {
                return CustomConvert(dataRow, columnName, null, SystemDataUtilities.GetGuid, parseBehaviorOption);
            }
            if (parseBehaviorOption == ParseBehaviorOption.ThrowException)
            {
                return CustomConvert(dataRow, columnName, Guid.Parse, null, parseBehaviorOption);
            }
            throw new ArgumentException("Option not supported", "parseBehaviorOption");
        }

        public static bool ToBool(this DataRow dataRow, string columnName,
            ParseBehaviorOption parseBehaviorOption = ParseBehaviorOption.DefaultValue)
        {
            if (parseBehaviorOption == ParseBehaviorOption.DefaultValue)
            {
                return CustomConvert(dataRow, columnName, null, SystemDataUtilities.GetBool, parseBehaviorOption);
            }
            if (parseBehaviorOption == ParseBehaviorOption.ThrowException)
            {
                return CustomConvert(dataRow, columnName, bool.Parse, null, parseBehaviorOption);
            }
            throw new ArgumentException("Option not supported", "parseBehaviorOption");
        }

        public static DateTime ToDateTime(this DataRow dataRow, string columnName,
            ParseBehaviorOption parseBehaviorOption = ParseBehaviorOption.DefaultValue)
        {
            if (parseBehaviorOption == ParseBehaviorOption.DefaultValue)
            {
                return CustomConvert(dataRow, columnName, null, SystemDataUtilities.GetDateTime, parseBehaviorOption);
            }
            if (parseBehaviorOption == ParseBehaviorOption.ThrowException)
            {
                return CustomConvert(dataRow, columnName, DateTime.Parse, null, parseBehaviorOption);
            }
            throw new ArgumentException("Option not supported", "parseBehaviorOption");
        }

        public static float ToFloat(this DataRow dataRow, string columnName,
            ParseBehaviorOption parseBehaviorOption = ParseBehaviorOption.DefaultValue)
        {
            if (parseBehaviorOption == ParseBehaviorOption.DefaultValue)
            {
                return CustomConvert(dataRow, columnName, null, SystemDataUtilities.GetFloat, parseBehaviorOption);
            }
            if (parseBehaviorOption == ParseBehaviorOption.ThrowException)
            {
                return CustomConvert(dataRow, columnName, float.Parse, null, parseBehaviorOption);
            }
            throw new ArgumentException("Option not supported", "parseBehaviorOption");
        }

        public static int ToInt(this DataRow dataRow, string columnName,
            ParseBehaviorOption parseBehaviorOption = ParseBehaviorOption.DefaultValue)
        {
            if (parseBehaviorOption == ParseBehaviorOption.DefaultValue)
            {
                return CustomConvert(dataRow, columnName, null, SystemDataUtilities.GetInt, parseBehaviorOption);
            }
            if (parseBehaviorOption == ParseBehaviorOption.ThrowException)
            {
                return CustomConvert(dataRow, columnName, int.Parse, null, parseBehaviorOption);
            }
            throw new ArgumentException("Option not supported", "parseBehaviorOption");
        }

        public static object ToObject(this DataRow dataRow, string columnName,
            ParseBehaviorOption parseBehaviorOption = ParseBehaviorOption.DefaultValue)
        {
            if (parseBehaviorOption == ParseBehaviorOption.DefaultValue)
            {
                return CustomConvert(dataRow, columnName, null, SystemDataUtilities.GetObject, parseBehaviorOption);
            }
            if (parseBehaviorOption == ParseBehaviorOption.ThrowException)
            {
                return CustomConvert(dataRow, columnName, int.Parse, null, parseBehaviorOption);
            }
            throw new ArgumentException("Option not supported", "parseBehaviorOption");
        }

        public static string ToString(this DataRow dataRow, string columnName,
            ParseBehaviorOption parseBehaviorOption = ParseBehaviorOption.DefaultValue)
        {
            if (parseBehaviorOption == ParseBehaviorOption.DefaultValue)
            {
                return CustomConvert(dataRow, columnName, null, SystemDataUtilities.GetString, parseBehaviorOption);
            }
            if (parseBehaviorOption == ParseBehaviorOption.ThrowException)
            {
                return CustomConvert(dataRow, columnName, ToString, null, parseBehaviorOption);
            }
            throw new ArgumentException("Option not supported", "parseBehaviorOption");
        }

        public static bool IsDbNull(this DataRow dataRow,string columnName)
        {
            var value = CustomConvert(dataRow, columnName, null, SystemDataUtilities.GetObject);
            return IsDbNull(value);
        }

        #endregion

        private static string ToString(string value)
        {
            return value;
        }

        private static void EvalThrowException(DataRow dataRow, string columnName)
        {
            var value = dataRow[columnName];
            if (IsDbNull(value))
                throw new ArgumentException("Invalid value");
        }

        private static bool IsDbNull(object value)
        {
            return value == null || value is DBNull || string.IsNullOrEmpty(value.ToString());
        }

        public enum ParseBehaviorOption
        {
            ThrowException,
            DefaultValue
        }
    }
}
