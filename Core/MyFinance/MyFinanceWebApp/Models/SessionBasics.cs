using System;
using MyFinance.Utilities;
using Newtonsoft.Json;

namespace MyFinance.MyFinanceWebApp.Models
{
	public class SessionBasics
	{
		private const string SessionTokenKey = "1d5522e6a8bc66acdd0f1cf8b28f9e43";

		public string Jwt { get; set; }
		public DateTime TokenExpires { get; set; }
		public string UserId { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }

		public bool HasExpired()
		{
			return DateTime.Now.AddMinutes(1) > TokenExpires;
		}

		public string GetAsCookieValue()
		{
			var jsonFormat = JsonConvert.SerializeObject(this);
			var encoded = EncodingUtils.Base64Encode(jsonFormat);
			var encrypted = AesHelper.EncryptString(SessionTokenKey, encoded);
			return encrypted;
		}

		public static SessionBasics ReadFromKey(string cookieString)
		{
			var decrypted = AesHelper.DecryptString(SessionTokenKey, cookieString);
			var decoded = EncodingUtils.Base64Decode(decrypted);
			return JsonConvert.DeserializeObject<SessionBasics>(decoded);
		}
	}
}
