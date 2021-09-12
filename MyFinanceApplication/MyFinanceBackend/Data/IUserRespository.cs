using System.Collections.Generic;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;

namespace MyFinanceBackend.Data
{
    public interface IUserRespository : ITransactional
    {
        IEnumerable<AppUser> GetOwendUsersByUserId(string userId);
        AppUser GetUserByUserId(string userId);
        AppUser GetUserByUsername(string username);
        LoginResult AttemptToLogin(string username, string encryptedPassword);
        bool SetPassword(string userId, string encryptedPassword);
        bool UpdateUser(ClientEditUser user);
        string AddUser(ClientAddUser user);
    }
}
