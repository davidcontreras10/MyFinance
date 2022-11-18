using System.Collections.Generic;
using MyFinanceModel.ViewModel;
using System;
using System.Threading.Tasks;
using MyFinanceBackend.Data;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;

namespace MyFinanceBackend.Services
{
	public class AccountService : IAccountService
	{
		#region Constructor

		public AccountService(IAccountRepository accountRepository)
		{
		    _accountRepository = accountRepository;
		}

        #endregion

        #region Attributes

	    private readonly IAccountRepository _accountRepository;

		#endregion

		#region Public methods

		public async Task<IReadOnlyCollection<AccountDetailsPeriodViewModel>> GetAccountDetailsPeriodViewModelAsync(
			string userId,
			DateTime dateTime
		)
		{
			return await _accountRepository.GetAccountDetailsPeriodViewModelAsync(userId, dateTime);
		}

		public IEnumerable<SupportedAccountIncludeViewModel> GetSupportedAccountIncludeViewModel(
			IEnumerable<ClientAddSpendAccountIncludeUpdate> listUpdates, string userId)
		{
			return _accountRepository.GetSupportedAccountIncludeViewModel(listUpdates, userId);
		}

		public UserAccountsViewModel GetAccountsByUserId(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				throw new ArgumentNullException(nameof(userId));
			}

			return _accountRepository.GetAccountsByUserId(userId);
		}

        public void DeleteAccount(string userId, int accountId)
        {
	        _accountRepository.DeleteAccount(userId, accountId);
        }

        public void AddAccount(string userId, ClientAddAccount clientAddAccount)
        {
	        _accountRepository.AddAccount(userId, clientAddAccount);
        }

        public AddAccountViewModel GetAddAccountViewModel(string userId)
        {
	        return _accountRepository.GetAddAccountViewModel(userId);
        }

	    public AccountMainViewModel GetAccountDetailsViewModel(string userId, int? accountGroupId)
	    {
	        return _accountRepository.GetAccountDetailsViewModel(userId, accountGroupId);
	    }

	    public IEnumerable<ItemModified> UpdateAccountPositions(string userId,
			IEnumerable<ClientAccountPosition> accountPositions)
	    {
		    return _accountRepository.UpdateAccountPositions(userId, accountPositions);
	    }

		public void UpdateAccount(string userId, ClientEditAccount clientEditAccount)
		{
			_accountRepository.UpdateAccount(userId, clientEditAccount);
		}

		public IEnumerable<AccountIncludeViewModel> GetAccountIncludeViewModel(string userId, int currencyId)
		{
			return _accountRepository.GetAccountIncludeViewModel(userId, currencyId);
		}

		public IEnumerable<AccountDetailsInfoViewModel> GetAccountDetailsViewModel(IEnumerable<int> accountIds, string userId)
		{
			return _accountRepository.GetAccountDetailsViewModel(accountIds, userId);
		}

		#endregion
    }
}