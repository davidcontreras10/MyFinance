using System.Collections.Generic;
using MyFinanceBackend.Attributes;

namespace MyFinanceBackend.Services
{
    public interface IAuthorizationService
    {
        bool IsAuthorized(string authenticatedUserId, IEnumerable<string> targetUserIds,
            ResourceActionRequiredAttribute resourceActionRequiredAttribute);
    }
}