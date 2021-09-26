using System.Threading.Tasks;
using MyFinance.MyFinanceModel;
using MyFinance.MyFinanceModel.ClientViewModel;
using MyFinance.MyFinanceModel.ViewModel;
using MyFinance.MyFinanceWebApp.Models;

namespace MyFinance.MyFinanceWebApp.Services
{
    public interface IUserService
    {
        Task<LoginResult> AttemptLoginAsync(string username, string password);
	    Task<bool> IsSessionValidAsync(string token);
	    Task<AuthToken> GetAuthTokenAsync(string username, string password);
        AuthToken RefreshToken(string refreshToken);
        ResetPasswordValidationResult ValidateResetPasswordActionResult(string actionLink);
        TokenActionValidationResult UpdateUserPassword(ClientNewPasswordRequest passwordResetRequest);
        PostResetPasswordEmailResponse SendResetPasswordEmail(ClientResetPasswordEmailRequest request);
        bool UpdateUser(ClientUser user, string targetUserId, string token);
        Task<AppUser> GetUserByIdAsync(string userId, string token);
    }
}
