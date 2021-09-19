using System.Collections.Generic;
using MyFinance.Backend.Attributes;

namespace MyFinance.Backend.Services
{
    public interface IAuthorizationService
    {
        bool IsAuthorized(string authenticatedUserId, IEnumerable<string> targetUserIds,
            ResourceActionRequiredAttribute resourceActionRequiredAttribute);
    }
}