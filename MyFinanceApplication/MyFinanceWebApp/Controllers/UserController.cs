using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceWebApp.Contants;
using MyFinanceWebApp.Helpers;
using MyFinanceWebApp.Models;
using MyFinanceWebApp.Services;

namespace MyFinanceWebApp.Controllers
{
    public class UserController : FinanceAppBaseController
    {
        #region Constructor

        public UserController(IUserService userService, IHtmlHeaderHelper htmlHeaderHelper)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _htmlHeaderHelper = htmlHeaderHelper;
        }

        #endregion

        #region Attributes

	    private readonly IUserService _userService;
        private readonly IHtmlHeaderHelper _htmlHeaderHelper;

        #endregion

        #region Public Action Methods

        [HttpGet]
        public ActionResult MeUser()
        {
            var userId = User.Identity.Name;
            var token = GetUserToken();
            var appUser = _userService.GetUserById(userId, token);
            var user = new EditAppUserClientViewModel
            {
                BirthDate = new DateTime(1990, 6, 15),
                Name = appUser.Name,
                Username = appUser.Username,
                PrimaryEmail = appUser.PrimaryEmail,
                HeaderModel = CreateMainHeaderModel(),
                HtmlHeaderHelper = _htmlHeaderHelper
            };

            ViewData["userId"] = userId;
            ViewData["title"] = "My Data";
            return View("UpdateUserForm", user);
        }

        [HttpGet]
        public ActionResult UpdateUser(string userId)
        {
            var token = GetUserToken();
            var appUser = _userService.GetUserById(userId, token);
            var user = new EditAppUserClientViewModel
            {
                BirthDate = new DateTime(1990, 6, 15),
                Name = appUser.Name,
                Username = appUser.Username,
                PrimaryEmail = appUser.PrimaryEmail,
                HeaderModel = CreateMainHeaderModel(),
                HtmlHeaderHelper = _htmlHeaderHelper
            };

            ViewData["userId"] = userId;
            ViewData["title"] = "Update User";
            return View("UpdateUserForm", user);
        }

        [HttpPost]
        public ActionResult UpdateUser(string userId, EditAppUserClientViewModel user)
        {
            if (!ModelState.IsValid)
            {
                user.HeaderModel = CreateMainHeaderModel();
                ViewData["userId"] = userId;
                ViewData["title"] = "Update User";
                return View("UpdateUserForm", user);
            }

            var token = GetUserToken();
            var editUser = CreateClientEditUser(user);
            var result = _userService.UpdateUser(editUser, userId, token);
            return result ? RedirectToAction("Index", "Home") : UpdateUser(userId);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ForgottenPassword()
        {
            var emptyModel = new ForgottenPasswordModel();
            return View("ForgottenPasswordForm", emptyModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ForgottenPassword(ForgottenPasswordModel model)
        {
            if (model == null)
            {
                return ForgottenPassword();
            }

            if (!ModelState.IsValid)
            {
                return View("ForgottenPasswordForm", model);
            }

            model.HostUrl = GetResetPasswordUrl();
            var response = _userService.SendResetPasswordEmail(model);
            ForgottenPasswordModel responseModel = null;
            switch (response)
            {
                case PostResetPasswordEmailResponse.Ok:
                    responseModel = new ForgottenPasswordModel
                    {
                        ReadyToUpdate = false,
                        Success = true,
                        PageMessage = "An email has been sent",
                    };
                    break;
                case PostResetPasswordEmailResponse.Invalid:
                    ModelState.AddModelError(nameof(ForgottenPasswordModel.Username), "Invalid information");
                    ModelState.AddModelError(nameof(ForgottenPasswordModel.Email), "Invalid information");
                    responseModel = model;
                    break;
            }

            return responseModel == null ? ForgottenPassword() : View("ForgottenPasswordForm", responseModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult SetNewPassword(CustomClientNewPassword passwordRequest)
        {
            if (!ModelState.IsValid)
            {
                return View("ResetPasswordForm", passwordRequest);
            }

            if (passwordRequest == null)
            {
                throw new ArgumentNullException(nameof(passwordRequest));
            }

            if (passwordRequest.Password == "12345")
            {
                ModelState.AddModelError(nameof(ClientNewPasswordRequest.Password), "Password is already in use");
                return View("ResetPasswordForm", passwordRequest);
            }

            var result = _userService.UpdateUserPassword(passwordRequest);
            switch (result)
            {
                case TokenActionValidationResult.Invalid:
                    var invalidModel = new CustomClientNewPassword
                    {
                        ConfirmPassword = "Link has expired",
                        ReadyToUpdate = false
                    };

                    return View("ResetPasswordForm", invalidModel);
                case TokenActionValidationResult.Unknown:
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            var successModel = new CustomClientNewPassword
            {
                Success = true,
                PageMessage = "Password updated"
            };

            return View("ResetPasswordForm", successModel);
        }

        [AllowAnonymous]
        public ActionResult Test(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new HttpUnauthorizedResult();
            }

            var emptyRequest = new CustomClientNewPassword
            {
                Token = token
            };

            return View("ResetPasswordForm", emptyRequest);
        }

        public ActionResult LogOff()
        {
            //TODO: code a signOut static method
            FormsAuthentication.SignOut();
            return Redirect(FormsAuthentication.LoginUrl);
        }

		[RequireHttps]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
            return View("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = TempData["returnUrl"]?.ToString();
                    TempData["returnUrl"] = null;
                }

                var loginResult = await AuthenticateUserAsync(model.Username, model.Password, model.RememberMe);
                TempData[Keys.CURRENT_USER_NAME] = loginResult.User.Name;
                return RedirectToLocal(returnUrl);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[UserController].[AuthenticateUser] Error: " + ex);
                return View("Login");
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string actionLink)
        {
            var validationResult = ResetPasswordActionValidation(actionLink);
            if (validationResult.ActionValidationResult == TokenActionValidationResult.Unknown)
            {
                return HttpNotFound();
            }

            if (validationResult.ActionValidationResult == TokenActionValidationResult.Invalid)
            {
                var expiredRequest = new CustomClientNewPassword
                {
                    PageMessage = "Link has expired",
                    ReadyToUpdate = false
                };

                return View("ResetPasswordForm", expiredRequest);
            }

            var emptyRequest = new CustomClientNewPassword
            {
                Token = actionLink
            };
            return View("ResetPasswordForm", emptyRequest);
        }

        #endregion

        #region Private Methods

        private MainHeaderModel CreateMainHeaderModel()
        {
            return PageHeaderBuilder.GetHeader(Url, PageHeaderBuilder.AppMenuItem.MyAccount);
        }

        private static ClientUser CreateClientEditUser(EditAppUserClientViewModel user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var result = new ClientUser
            {
                Username = user.Username,
                BirthDate = user.BirthDate,
                Name = user.Name,
                Email = user.PrimaryEmail
            };

            return result;
        }

        private string GetResetPasswordUrl()
        {
            if (Request.Url != null)
            {
                var fullRequestUrl = Request.Url.AbsoluteUri;
                var rootPath = fullRequestUrl.Replace(Request.Url.AbsolutePath, "");
                var resetPasswordPath = rootPath + Url.Action("ResetPassword", "User");
                return resetPasswordPath;
            }

            throw new Exception("Invalid URI object");
        }

        private ResetPasswordValidationResult ResetPasswordActionValidation(string actionLink)
        {
            if (string.IsNullOrEmpty(actionLink))
            {
                return new ResetPasswordValidationResult();
            }

            var result = _userService.ValidateResetPasswordActionResult(actionLink);
            return result;
        }

        private AuthToken GetAuthToken(string username, string password)
	    {
		    var authToken = _userService.GetAuthToken(username, password);
		    return authToken;
	    }

        private async Task<LoginResult> AuthenticateUserAsync(string username, string password, bool rememberme)
        {
            FormsAuthentication.SignOut();
            var attemptLogin = await _userService.AttemptLoginAsync(username, password);
            if (attemptLogin == null)
            {
                Debug.WriteLine("[UserController].[AuthenticateUser] Empty login result");
                throw new UserAuthenticationException("No login result");
            }

            if (!attemptLogin.IsAuthenticated)
            {
                Debug.WriteLine($"[UserController].[AuthenticateUser] Error: {attemptLogin.ResultMessage}");
                throw new UserAuthenticationException(attemptLogin);
            }

			//var timeout = FormsAuthentication.Timeout;
            var token = GetAuthToken(username,password);
	        if (token == null)
	        {
		        throw new UserAuthenticationException();
	        }

            var encryptedAccessToken = LocalHelper.Protect(token.AccessToken, attemptLogin.User.UserId.ToString());
            var encryptedRefreshToken = LocalHelper.Protect(token.RefreshToken, attemptLogin.User.UserId.ToString());
            var lastUpdated = DateTime.Now;
            var lastUpdatedString = lastUpdated.ToString("O");

            var authorizationCookie = new HttpCookie("TokenAuthorization");//instantiate an new cookie and give it a name
            authorizationCookie.Values.Add("AuthToken", encryptedAccessToken);//populate it with key, value pairs
            authorizationCookie.Values.Add("RefreshToken", encryptedRefreshToken);//populate it with key, value pairs
            authorizationCookie.Values.Add("LastUpdate",lastUpdatedString);
            Response.Cookies.Add(authorizationCookie);//add it to the client
            FormsAuthentication.SetAuthCookie(attemptLogin.User.UserId.ToString(), rememberme);           
            return attemptLogin;
        }

        #endregion

		#region Helpers

		private ActionResult RedirectToLocal(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl) ? (ActionResult) Redirect(returnUrl) : RedirectToAction("Index", "Home");
        }

        #endregion

    }
}