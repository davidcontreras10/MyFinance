using System.Collections.Generic;
using MyFinanceModel;

namespace MyFinanceBackend.Data
{
    public interface IAuthorizationDataRepository
    {
        IEnumerable<UserAssignedAccess> GetUserAssignedAccess(string userId,
            ApplicationResources applicationResource = ApplicationResources.Unknown,
            ResourceActionNames actionName = ResourceActionNames.Unknown);
    }
}