using System;
using System.Collections.Generic;
using System.Linq;
using MyFinanceBackend.Data;
using MyFinanceModel;

namespace MyFinanceBackend.Services.AuthServices
{
    public class UserAuthorizeService : IUserAuthorizeService
    {
        private readonly IAuthorizationDataRepository _authorizationDataRepository;
        private readonly IUserRespository _userRespository;

        public UserAuthorizeService(IAuthorizationDataRepository authorizationDataRepository, IUserRespository userRespository)
        {
            _authorizationDataRepository = authorizationDataRepository;
            _userRespository = userRespository;
        }

        public bool IsAuthorized(string authenticatedUserId, IEnumerable<string> targetUserIds,
            IEnumerable<ResourceActionNames> actionNames)
        {
            if (actionNames == null || !actionNames.Any())
            {
                throw new ArgumentNullException(nameof(actionNames));
            }

            foreach (var action in actionNames)
            {
                var userAccessData =
                    _authorizationDataRepository.GetUserAssignedAccess(authenticatedUserId, ApplicationResources.Users,
                        action);
                foreach (var assignedAccess in userAccessData)
                {
                    var result = EvaluateResourceAccesLevel(assignedAccess.ResourceAccesLevel, authenticatedUserId,
                        targetUserIds);
                    if (result)
                        return true;
                }
            }

            return false;
        }

        private bool EvaluateResourceAccesLevel(ResourceAccesLevels resourceAccesLevel, string authenticatedUserId,
            IEnumerable<string> targetUserIds)
        {
            switch (resourceAccesLevel)
            {
                case ResourceAccesLevels.Any: return AnyResourceAccesLevelEvaluation();
                case ResourceAccesLevels.Owned:
                    return OwnedResourceAccesLevelEvaluation(authenticatedUserId, targetUserIds);
                case ResourceAccesLevels.Self:
                    return SelfResourceAccesLevelEvaluation(authenticatedUserId, targetUserIds);
                case ResourceAccesLevels.AddRegular:
                    return NoEvaluationRequired();
                default: throw new ArgumentException("Invalid argument");
            }
        }

        private bool NoEvaluationRequired()
        {
            return true;
        }

        private bool AnyResourceAccesLevelEvaluation()
        {
            return true;
        }

        private bool SelfResourceAccesLevelEvaluation(string authenticatedUserId, IEnumerable<string> targetUserIds)
        {
            return targetUserIds.All(id => new Guid(id) == new Guid(authenticatedUserId));
        }

        private bool OwnedResourceAccesLevelEvaluation(string authenticatedUserId, IEnumerable<string> targetUserIds)
        {
            var owendUsers = _userRespository.GetOwendUsersByUserId(authenticatedUserId);
            return owendUsers.All(u => targetUserIds.Any(tu => new Guid(tu) == u.UserId));
        }
    }
}
