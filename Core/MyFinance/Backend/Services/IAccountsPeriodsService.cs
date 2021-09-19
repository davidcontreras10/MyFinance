using System;
using MyFinance.MyFinanceModel;

namespace MyFinance.Backend.Services
{
    public interface IAccountsPeriodsService
    {
        void CreateAccountPeriod(string userId, int accountId, DateTime initial, DateTime end, float budget);
        AddPeriodData GetAddPeriodData(int accountId, string userId);
    }
}