using System.Collections.Generic;
using MyFinance.MyFinanceModel;

namespace MyFinance.Backend.Data
{
    public interface IAuthorizationDataRepository
    {
        IEnumerable<UserAssignedAccess> GetUserAssignedAccess(string userId,
            ApplicationResources applicationResource = ApplicationResources.Unknown,
            ResourceActionNames actionName = ResourceActionNames.Unknown);
    }
}