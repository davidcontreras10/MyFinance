using System.Threading.Tasks;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using MyFinanceWebApp.Models;

namespace MyFinanceWebApp.Services
{
    public interface IUserService
    {
        Task<LoginResult> AttemptLoginAsync(string username, string password);
	    bool IsSessionValid(string token);
	    AuthToken GetAuthToken(string username, string password);
        AuthToken RefreshToken(string refreshToken);
        ResetPasswordValidationResult ValidateResetPasswordActionResult(string actionLink);
        TokenActionValidationResult UpdateUserPassword(ClientNewPasswordRequest passwordResetRequest);
        PostResetPasswordEmailResponse SendResetPasswordEmail(ClientResetPasswordEmailRequest request);
        bool UpdateUser(ClientUser user, string targetUserId, string token);
        AppUser GetUserById(string userId, string token);
    }
}
