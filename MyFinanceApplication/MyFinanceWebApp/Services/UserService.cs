using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.Utilities;
using MyFinanceModel.ViewModel;
using MyFinanceWebApp.Contants;
using MyFinanceWebApp.Models;

namespace MyFinanceWebApp.Services
{
    public class UserService:BaseService, IUserService
    {
        public Task<LoginResult> AttemptLoginAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

	    public bool IsSessionValid(string token)
	    {
		    throw new System.NotImplementedException();
	    }

	    public AuthToken GetAuthToken(string username, string password)
	    {
		    throw new System.NotImplementedException();
	    }

        public AuthToken RefreshToken(string refreshToken)
        {
            throw new System.NotImplementedException();
        }

        public ResetPasswordValidationResult ValidateResetPasswordActionResult(string action)
        {
            throw new System.NotImplementedException();
        }

        public TokenActionValidationResult UpdateUserPassword(ClientNewPasswordRequest passwordResetRequest)
        {
            throw new System.NotImplementedException();
        }

        public PostResetPasswordEmailResponse SendResetPasswordEmail(ClientResetPasswordEmailRequest request)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateUser(ClientUser user, string userId, string token)
        {
            throw new System.NotImplementedException();
        }

        public AppUser GetUserById(string userId, string token)
        {
            throw new System.NotImplementedException();
        }
    }
}