using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceBackend.Services
{
    public interface IUsersService
    {
        AppUser GetUser(string userId);
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