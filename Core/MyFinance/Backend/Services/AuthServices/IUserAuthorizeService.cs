using System.Collections.Generic;
using MyFinance.MyFinanceModel;

namespace MyFinance.Backend.Services.AuthServices
{
    public interface IUserAuthorizeService
    {
        bool IsAuthorized(string authenticatedUserId, IEnumerable<string> targetUserIds,
            IEnumerable<ResourceActionNames> actionNames);
    }
}
