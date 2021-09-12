using System;
using System.Collections.Generic;
using System.Text;

namespace MyFinance.Helpers
{
	public class EncodingUtilities
	{
		public static string Base64Encode(string plainText)
		{
			var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			return Convert.ToBase64String(plainTextBytes);
		}

		public static string Base64Decode(string base64EncodedData)
		{
			var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
			return Encoding.UTF8.GetString(base64EncodedBytes, 0, base64EncodedBytes.Length);
		}
	}
}
