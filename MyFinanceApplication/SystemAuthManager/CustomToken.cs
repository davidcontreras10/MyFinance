using System;

namespace SystemAuthManager
{
    public class CustomToken
    {
        public string Token { get; set; }
        public TimeSpan Expiration { get; set; }
        public DateTime LastUpdate { get; set; }
        public string UserId { set; get; }

        public bool HasExpired()
        {
            var tokenExpiration = LastUpdate.Add(Expiration);
            return DateTime.Now > tokenExpiration;
        }
    }
}
