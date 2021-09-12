using System;
using MyFinanceWebApi.Helpers;

namespace MyFinanceWebApi.Authorization
{
    internal static class AuthCommonSettings
    {
        public static TimeSpan AuthExpireToken => CustomAppSettings.AuthExpireToken;
    }
}