using System;
using System.Globalization;
using System.Linq;
using System.Text;

// ReSharper disable UnusedMember.Global

namespace MyFinance.Utilities
{
    public static class StringUtilities
    {
	    public static string ToCamelCase(this string value)
	    {
		    if (string.IsNullOrWhiteSpace(value))
		    {
			    throw new ArgumentNullException(nameof(value));
		    }

		    var stringBuilder = new StringBuilder(value);
		    stringBuilder[0] = stringBuilder[0].ToString().ToLower()[0];
		    return stringBuilder.ToString();
	    }

        public static Guid ConvertToGuid(object value)
        {
            if (value == null)
                return Guid.Empty;
            if (value is Guid guid)
                return guid;
            return Guid.TryParse(value.ToString(), out var result) ? result : Guid.Empty;
        }

        public static bool ConvertToBool(object value)
        {
            if (value == null)
                return false;
            if (value is bool b)
                return b;
            if (value is int i)
                return i != 0;

            return bool.TryParse(value.ToString(), out var result) && result;
        }

        public static string StringDefinedPart(this string value, char initial, char final)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";
            if (!value.Contains(initial) || !value.Contains(final))
                return "";
            var result = "";
            var active = false;
            foreach (var c in value)
            {
                if (active)
                {
                    if (final == c)
                    {
                        return result;
                    }
                    result += c;
                }
                if (c == initial)
                {
                    active = true;
                }
            }
            return result;
        }

        public static string RemoveAccentsWithNormalization(string inputString)
        {
            var normalizedString = inputString.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var t in from t in normalizedString
                              let uc = CharUnicodeInfo.GetUnicodeCategory(t)
                              where uc != UnicodeCategory.NonSpacingMark
                              select t)
            {
                sb.Append(t);
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        public static bool StringEquals(this string value1, string value2)
        {
            value1 = Trim(value1);
            value2 = Trim(value2);
            if (string.IsNullOrWhiteSpace(value1) && string.IsNullOrWhiteSpace(value2))
                return true;
            value1 = RemoveAccentsWithNormalization(value1);
            value2 = RemoveAccentsWithNormalization(value2);
            value1 = value1.ToLower();
            value2 = value2.ToLower();
            return value1 == value2;
        }

        public static bool StringContains(this string value1, string value2)
        {
            value1 = Trim(value1);
            value2 = Trim(value2);
            if (string.IsNullOrWhiteSpace(value1) && string.IsNullOrWhiteSpace(value2))
                return true;
            value1 = RemoveAccentsWithNormalization(value1);
            value2 = RemoveAccentsWithNormalization(value2);
            value1 = value1.ToLower();
            value2 = value2.ToLower();
            return value1.Contains(value2);
        }

        public static string Trim(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "" : value.Trim();
        }

        public static int ConverToInt(string value)
        {
	        return !int.TryParse(value, out var result) ? 0 : result;
        }

        public static int GetInt(object value)
        {
            if (value == null)
                return 0;
            if (value is int i)
                return i;
            return int.TryParse(value.ToString(), out var result) ? result : 0;
        }

        public static bool EqualStrings(object value1, object value2)
        {
            value1 ??= "";
            value2 ??= "";
            return value1.ToString().Trim() == value2.ToString().Trim();
        }

        public static double ConvertToDouble(object value)
        {
            if (value == null)
                return 0;
            if (value is double d)
                return d;
            var sValue = value.ToString();
            sValue = sValue.Replace(',', '.');
            return double.TryParse(sValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : 0;
        }

        public static float ConvertToFloat(object value)
        {
            if (value == null)
                return 0;
            if (value is float f)
                return f;
            var sValue = value.ToString();
            return float.TryParse(sValue, out var result) ? result : 0;
        }

        public static DateTime ConvertToDateTime(object value)
        {
            if (value == null)
                return new DateTime();
            if (value is DateTime time)
                return time;
            return DateTime.TryParse(value.ToString(), out var result) ? result : new DateTime();
        }

    }
}
