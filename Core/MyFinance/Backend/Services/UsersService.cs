using System;
using MyFinance.Backend.Data;
using MyFinance.Backend.Utils;
using MyFinance.MyFinanceModel;
using MyFinance.MyFinanceModel.ClientViewModel;
using MyFinance.MyFinanceModel.ViewModel;

namespace MyFinance.Backend.Services
{
    public class UsersService:IUsersService
	{
		#region Attributes

	    private readonly IUserRespository _userRepository;
	    private readonly IEmailService _emailService;
	    private readonly IMyFinanceSettings _myFinanceSettings;

	    #endregion

		#region Constructor

		public UsersService(
			IUserRespository userRepository,
			IEmailService emailService,
			IMyFinanceSettings myFinanceSettings
		)
		{
			_userRepository = userRepository;
			_emailService = emailService;
			_myFinanceSettings = myFinanceSettings;
		}

		#endregion

		#region Public Methods

        public AppUser GetUser(string userId)
		{
			var user = _userRepository.GetUserByUserId(userId);
			return user;
		}

        public LoginResult AttemptToLogin(string username, string password)
        {
	        var encryptedPassword = PasswordUtils.EncryptText(password);
	        var result = _userRepository.AttemptToLogin(username, encryptedPassword);
	        return result;
        }

	    public bool SetPassword(string userId, string newPassword)
	    {
		    var encryptedPassword = PasswordUtils.EncryptText(newPassword);
		    var result = _userRepository.SetPassword(userId, encryptedPassword);
		    return result;
	    }

	    public PostResetPasswordEmailResponse SendResetPasswordEmail(ClientResetPasswordEmailRequest request)
	    {
	        var userInfo = _userRepository.GetUserByUsername(request.Username);
	        if (userInfo == null)
	        {
	            throw new UnauthorizedAccessException();
	        }

	        var resetPasswordLink = CreateResetPasswordLink(request.HostUrl, userInfo.UserId.ToString());
	        var body = GetResetPasswordEmailTemplate(resetPasswordLink, userInfo.Username);
	        var result = _emailService.SendEmailAsync(userInfo.PrimaryEmail, "My Finance Reset Password", body, true).Result;
	        return result ? PostResetPasswordEmailResponse.Ok : PostResetPasswordEmailResponse.Unknown;
	    }

	    public bool ValidResetPasswordEmailRequest(ClientResetPasswordEmailRequest request)
	    {
	        if (string.IsNullOrEmpty(request?.Username) || string.IsNullOrEmpty(request.Email))
	        {
	            return false;   
	        }

	        var user = _userRepository.GetUserByUsername(request.Username);
	        return !string.IsNullOrEmpty(user?.PrimaryEmail) &&
	               (user.Username == request.Username && user.PrimaryEmail == request.Email);
	    }

	    public ResetPasswordValidationResult ValidateResetPasswordActionResult(string actionLink)
	    {
	        if (string.IsNullOrEmpty(actionLink))
	        {
	            return new ResetPasswordValidationResult();
	        }

	        var token = CreateClientResetPasswordToken(actionLink);
	        if (token == null)
	        {
	            return new ResetPasswordValidationResult();
	        }

	        if (token.DateTimeIssued.Add(token.ExpireTime) <= DateTime.Now)
	        {
	            return new ResetPasswordValidationResult(TokenActionValidationResult.Invalid);
	        }

	        var user = _userRepository.GetUserByUserId(token.UserId);
	        var newToken = new UserToken
	        {
	            ExpireTime = _myFinanceSettings.ResetPasswordTokenExpireTime,
	            UserId = token.UserId,
	            DateTimeIssued = DateTime.Now
	        };

            return new ResetPasswordValidationResult(user)
            {
                Token = newToken.SerializeToken()
            };
	    }

	    public TokenActionValidationResult UpdateUserPassword(ClientNewPasswordRequest passwordResetRequest)
	    {
	        UserToken token;
	        token = TokenUtils.TryDeserializeToken(passwordResetRequest.Token, out token) ? token : null;
	        if (token == null)
	        {
	            return TokenActionValidationResult.Unknown;
	        }

	        if (token.HasExpired())
	        {
	            return TokenActionValidationResult.Invalid;
	        }

	        var result = SetPassword(token.UserId, passwordResetRequest.Password);
	        return result ? TokenActionValidationResult.Ok : TokenActionValidationResult.Unknown;
	    }

	    public bool UpdateUser(string userId, ClientEditUser user)
	    {
	        if (user == null)
	        {
	            throw new ArgumentNullException(nameof(user));
	        }

	        return _userRepository.UpdateUser(user);
	    }

	    public bool AddUser(ClientAddUser user, string userId)
	    {
	        if (user == null)
	        {
	            throw new ArgumentNullException(nameof(user));
	        }

	        user.CreatedByUserId = userId;
            throw new NotImplementedException();
	    }

	    #endregion

        #region Private Methods



	    private static UserToken CreateClientResetPasswordToken(string action)
	    {
	        UserToken token;
	        token = TokenUtils.TryDeserializeToken(action, out token) ? token : null;
	        return token;
	    }

	    private string CreateResetPasswordLink(string hostUrl, string userId)
	    {
	        var token = new UserToken
	        {
	              UserId = userId,
	            DateTimeIssued = DateTime.Now,
	            ExpireTime = TimeSpan.FromMinutes(5)
	        };

	        var encodedEncryptToken = token.SerializeToken();
	        var result = $"{hostUrl}?actionLink={encodedEncryptToken}";
	        return result;
	    }

	    private static string GetResetPasswordEmailTemplate(string link, string user, int expireTimeHours = 2)
	    {
	        var fileContent = Resources.ResetPassword;
	        fileContent = string.Format(fileContent, user, link, expireTimeHours);
	        return fileContent;
	    }

	    //private bool CanUserUpdatePassword(string userId, string targetUser)
	    //{
     //       var userGuid = new Guid(userId);
     //       var targetUserGuid = new Guid(targetUser);
     //       return userGuid == targetUserGuid;
	    //}

		#endregion
	}
}