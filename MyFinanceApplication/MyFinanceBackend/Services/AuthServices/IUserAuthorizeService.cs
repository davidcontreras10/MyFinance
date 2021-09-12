using System.Collections.Generic;
using MyFinanceModel;

namespace MyFinanceBackend.Services.AuthServices
{
    public interface IUserAuthorizeService
    {
        bool IsAuthorized(string authenticatedUserId, IEnumerable<string> targetUserIds,
            IEnumerable<ResourceActionNames> actionNames);
    }
}
