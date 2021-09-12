using System.Collections.Generic;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceWebApp.Services
{
    public interface ISpendTypeService
    {
        //IEnumerable<SpendTypeViewModel> GetSpendTypeByAccountViewModels(string userId, int? accountId);
        IEnumerable<SpendTypeViewModel> GetAllSpendTypes(string token);
        IEnumerable<SpendTypeViewModel> GetUserSpendTypes(string token);
        IEnumerable<int> EditSpendTypes(string token, ClientSpendType clientSpendType);
        IEnumerable<int> AddSpendTypes(string token, ClientSpendType clientSpendType);
        void DeleteSpendType(string token, int spendTypeId);
        IEnumerable<int> DeleteSpendTypeUser(string token, int spendTypeId);
        IEnumerable<int> AddSpendTypeUser(string token, int spendTypeId);
    }
}
