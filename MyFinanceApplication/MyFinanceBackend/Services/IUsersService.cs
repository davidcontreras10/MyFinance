using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using System.Threading.Tasks;

namespace MyFinanceBackend.Services
{
    public interface IUsersService
    {
        Task<AppUser> GetUserAsync(string userId);

		AppUser GetUser(string userId);
        Task<LoginResult> AttemptToLoginAsync(string username, string password);

		LoginResult AttemptToLogin(string username, string password);
        bool SetPassword(string userId, string newPassword);
        PostResetPasswordEmailResponse SendResetPasswordEmail(ClientResetPasswordEmailRequest request);
        bool ValidResetPasswordEmailRequest(ClientResetPasswordEmailRequest request);
        ResetPasswordValidationResult ValidateResetPasswordActionResult(string actionLink);
        TokenActionValidationResult UpdateUserPassword(ClientNewPasswordRequest passwordResetRequest);
        bool UpdateUser(string userId, ClientEditUser user);
        bool AddUser(ClientAddUser user, string userId);
    }
}