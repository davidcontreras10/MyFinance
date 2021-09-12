using System;
using System.Collections.Generic;
using System.Linq;

namespace SystemAuthManager
{
    public static class LocalAuthManager
    {
        public enum TokenValidationResult
        {
            Invalid,
            Expired,
            NotExist,
            Valid
        }

        public static void RemoveTokenUserId(string userId)
        {
            TokensList().RemoveAll(t => t.UserId == userId);
        }

        public static void RemoveTokenId(string tokenKey)
        {
            var token = GetToken(tokenKey);
            TokensList().RemoveAll(t => t.UserId == token.UserId);
        }

        public static string CreateToken(TimeSpan expiration, string userId)
        {
            var customToken = new CustomToken
            {
                Expiration = expiration,
                LastUpdate = DateTime.Now,
                Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                UserId = userId
            };

            if (TokensList().Exists(item => item.Token == customToken.Token))
            {
                throw new Exception("Error to create token. It was already registered");
            }

            TokensList().RemoveAll(t => t.UserId == userId);
            TokensList().Add(customToken);
            return customToken.Token;
        }

        public static TokenValidationResult ValidateToken(string userId, string tokenKey)
        {
            var token = GetToken(tokenKey);
            if (token == null)
                return TokenValidationResult.NotExist;
            var result = token.UserId != userId
                ? TokenValidationResult.Invalid
                : (token.HasExpired() ? TokenValidationResult.Expired : TokenValidationResult.Valid);
            return result;
        }

        public static bool UpgradeToken(string tokenKey)
        {
            var token = GetToken(tokenKey);
            if (token == null)
                return false;
            token.LastUpdate = DateTime.Now;
            return true;
        }

        private static CustomToken GetToken(string tokenKey)
        {
            return TokensList().FirstOrDefault(item => item.Token == tokenKey);
        }

        private static List<CustomToken> TokensList()
        {
            return _customTokens ?? (_customTokens = new List<CustomToken>());
        }

        private static List<CustomToken> _customTokens;
    }
}
