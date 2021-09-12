using System;
using System.Collections.Generic;
using MyFinanceBackend.Attributes;
using MyFinanceBackend.Services.AuthServices;
using MyFinanceModel;

namespace MyFinanceBackend.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        #region Attributes

        private readonly IUserAuthorizeService _userAuthorizeService;

        #endregion

        #region Constructor

        public AuthorizationService(IUserAuthorizeService userAuthorizeService)
        {
            _userAuthorizeService = userAuthorizeService;
        }

        #endregion

        public bool IsAuthorized(string authenticatedUserId, IEnumerable<string> targetUserIds,
            ResourceActionRequiredAttribute resourceActionRequiredAttribute)
        {
            if (string.IsNullOrEmpty(authenticatedUserId))
            {
                throw new ArgumentNullException(nameof(authenticatedUserId));
            }

            if (resourceActionRequiredAttribute == null)
            {
                return true;
            }

            if (resourceActionRequiredAttribute.Resource == ApplicationResources.Users)
            {
                return _userAuthorizeService.IsAuthorized(authenticatedUserId, targetUserIds,
                    resourceActionRequiredAttribute.Actions);
            }

            throw new NotImplementedException();
        }
    }
}
