using System.Collections.Generic;
using MyFinanceBackend.Data;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceBackend.Services
{
    public interface ISpendTypeService
    {
		IEnumerable<int> DeleteSpendTypeUser(string userId, int spendTypeId);
		IEnumerable<int> AddSpendTypeUser(string userId, int spendTypeId);
		IEnumerable<SpendTypeViewModel> GeSpendTypeByAccountViewModels(string userId, int? accountId);
		IEnumerable<SpendTypeViewModel> GeSpendTypes(string userId, bool includeAll = true);
        IEnumerable<int> AddEditSpendTypes(string userId, ClientSpendType clientSpendType);
        void DeleteSpendType(string userId, int spendTypeId);
    }

    public class SpendTypeService :  ISpendTypeService
	{
		#region Attributes

	    private readonly ISpendTypeRepository _spendTypeRepository;

		#endregion

		#region Constructor

		public SpendTypeService(ISpendTypeRepository spendTypeRepository)
		{
			_spendTypeRepository = spendTypeRepository;
		}

		#endregion

		#region Methods

	    public IEnumerable<int> DeleteSpendTypeUser(string userId, int spendTypeId)
	    {
		    return _spendTypeRepository.DeleteSpendTypeUser(userId, spendTypeId);
	    }

	    public IEnumerable<int> AddSpendTypeUser(string userId, int spendTypeId)
	    {
		    return _spendTypeRepository.AddSpendTypeUser(userId, spendTypeId);
	    }

		public IEnumerable<SpendTypeViewModel> GeSpendTypeByAccountViewModels(string userId, int? accountId)
		{
			var result = _spendTypeRepository.GetSpendTypeByAccountViewModels(userId, accountId);
			return result;
		}

	    public IEnumerable<SpendTypeViewModel> GeSpendTypes(string userId, bool includeAll = true)
	    {
		    var result = _spendTypeRepository.GetSpendTypes(userId, includeAll);
		    return result;
	    }

        public IEnumerable<int> AddEditSpendTypes(string userId, ClientSpendType clientSpendType)
        {
            var result = _spendTypeRepository.AddEditSpendTypes(userId, clientSpendType);
            return result;
        }

        public void DeleteSpendType(string userId, int spendTypeId)
        {
            _spendTypeRepository.DeleteSpendType(userId, spendTypeId);
        }

        #endregion
    }
}
