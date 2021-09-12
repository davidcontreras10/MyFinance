using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Utilities
{
    public static class StringUtilities
    {
        public static Guid ConvertToGuid(object value)
        {
            if (value == null)
                return Guid.Empty;
            if (value is Guid)
                return (Guid)value;
            Guid result;
            return Guid.TryParse(value.ToString(), out result) ? result : Guid.Empty;
        }

        public static bool ConvertToBool(object value)
        {
            if (value == null)
                return false;
            if (value is bool)
                return (bool) value;
            if (value is int)
                return (int)value != 0;

            bool result;
            return bool.TryParse(value.ToString(), out result) && result;
        }

        public static string StringDefinedPart(this string value, char initial, char final)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            if (!value.Contains(initial) || !value.Contains(final))
                return "";
            var result = "";
            var active = false;
            foreach (char c in value)
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
            if (string.IsNullOrEmpty(value1) && string.IsNullOrEmpty(value2))
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
            if (string.IsNullOrEmpty(value1) && string.IsNullOrEmpty(value2))
                return true;
            value1 = RemoveAccentsWithNormalization(value1);
            value2 = RemoveAccentsWithNormalization(value2);
            value1 = value1.ToLower();
            value2 = value2.ToLower();
            return value1.Contains(value2);
        }

        public static string Trim(string value)
        {
            return string.IsNullOrEmpty(value) ? "" : value.Trim();
        }

        public static int ConverToInt(string value)
        {
            int result;
            return !int.TryParse(value, out result) ? 0 : result;
        }

        public static int GetInt(object value)
        {
            if (value == null)
                return 0;
            if (value is int)
                return (int) value;
            int result;
            return int.TryParse(value.ToString(), out result) ? result : 0;
        }

        public static bool EqualStrings(object value1, object value2)
        {
            if (value1 == null)
                value1 = "";
            if (value2 == null)
                value2 = "";
            return value1.ToString().Trim() == value2.ToString().Trim();
        }

        public static double ConvertToDouble(object value)
        {
            if (value == null)
                return 0;
            if (value is double)
                return (double) value;
            string sValue = value.ToString();
            sValue = sValue.Replace(',', '.');
            double result;
            return double.TryParse(sValue,NumberStyles.Any,CultureInfo.InvariantCulture, out result) ? result : 0;
        }

        public static float ConvertToFloat(object value)
        {
            if (value == null)
                return 0;
            if (value is float)
                return (float)value;
            string sValue = value.ToString();
            float result;
            return float.TryParse(sValue, out result) ? result : 0;
        }

        public static DateTime ConvertToDateTime(object value)
        {
            if (value == null)
                return new DateTime();
            if (value is DateTime)
                return (DateTime) value;
            DateTime result;
            return DateTime.TryParse(value.ToString(), out result) ? result : new DateTime();
        }

    }
}
