using System.Collections.Generic;
using MyFinance.MyFinanceModel;
using MyFinance.MyFinanceModel.ClientViewModel;

namespace MyFinance.Backend.Data
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
