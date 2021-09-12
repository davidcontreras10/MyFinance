using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System.Security.Cryptography;

namespace MyFinanceWebApi.Authorization
{
    public class ApplicationRefreshTokenProvider : AuthenticationTokenProvider
    {
        private static readonly ConcurrentDictionary<string, AuthenticationTicket> RefreshTokens =
            new ConcurrentDictionary<string, AuthenticationTicket>();
        private static int _counter;

        public override async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            //var guid = CreateRandomString(64);
            var guid = Guid.NewGuid().ToString();

            // copy all properties and set the desired lifetime of refresh token  
            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = DateTime.UtcNow.Add(AuthCommonSettings.AuthExpireToken)//DateTime.UtcNow.AddYears(1)  
            };
            var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

            RefreshTokens.TryAdd(guid, refreshTokenTicket);

            // consider storing only the hash of the handle  
            context.SetToken(guid);
        }

        public override async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            AuthenticationTicket ticket;
            string header = context.OwinContext.Request.Headers["Authorization"];

            if (RefreshTokens.TryRemove(context.Token, out ticket) && DateTime.UtcNow <= ticket.Properties.ExpiresUtc)
            {
                ticket.Properties.ExpiresUtc = DateTime.UtcNow.Add(AuthCommonSettings.AuthExpireToken);
                context.SetTicket(ticket);
            }
        }

        private static string CreateRandomString(int length)
        {
            length -= 12; //12 digits are the counter
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            long count = System.Threading.Interlocked.Increment(ref _counter);
            byte[] randomBytes = new byte[length * 3 / 4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            byte[] buf = new byte[8];
            buf[0] = (byte)count;
            buf[1] = (byte)(count >> 8);
            buf[2] = (byte)(count >> 16);
            buf[3] = (byte)(count >> 24);
            buf[4] = (byte)(count >> 32);
            buf[5] = (byte)(count >> 40);
            buf[6] = (byte)(count >> 48);
            buf[7] = (byte)(count >> 56);
            return Convert.ToBase64String(buf) + Convert.ToBase64String(randomBytes);
        }
    }
}