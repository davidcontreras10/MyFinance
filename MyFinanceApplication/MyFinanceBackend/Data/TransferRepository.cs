using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DataAccess;
using MyFinanceBackend.Constants;
using MyFinanceBackend.Services;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceBackend.Data
{
    public interface ITransferRepository : ITransactional
    {
        IEnumerable<AccountViewModel> GetPossibleDestinationAccount(int accountPeriodId, int currencyId, string userId);
        int GetDefaultCurrencyConvertionMethods(int originAccountId, int amountCurrencyId, int destinationCurrencyId,
            string userId);
        void AddTransferRecord(IEnumerable<int> spendIds, string userId);
    }

    public class TransferRepository : SqlServerBaseService, ITransferRepository
    {
        #region Constructor

        public TransferRepository(IConnectionConfig config) : base(config)
        {
        }

        #endregion

        #region Public Methods

        public IEnumerable<AccountViewModel> GetPossibleDestinationAccount(int accountPeriodId, int currencyId,
            string userId)
        {
            if (accountPeriodId == 0)
                throw new ArgumentException(@"Value cannot be zero", nameof(accountPeriodId));

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            if (currencyId == 0)
                throw new ArgumentException(@"Value cannot be zero", nameof(currencyId));

            var parameters = new List<SqlParameter>
            {
                new SqlParameter(DatabaseConstants.PAR_USER_ID, userId),
                new SqlParameter(DatabaseConstants.PAR_CURRENCY_ID, currencyId),
                new SqlParameter(DatabaseConstants.PAR_ACCOUNT_PERIOD_ID, accountPeriodId)
            };

            var dataSet = ExecuteStoredProcedure(DatabaseConstants.SP_TRANSFER_POSSIBLE_DESTINATION_ACCOUNTS, parameters);
            if (dataSet == null || dataSet.Tables.Count < 0)
                return new AccountViewModel[] { };
            return ServicesUtils.CreateGenericList(dataSet.Tables[0], ServicesUtils.CreateAccountViewModel);
        }

        public int GetDefaultCurrencyConvertionMethods(int originAccountId,
            int amountCurrencyId, int destinationCurrencyId, string userId)
        {

            var parameters = new List<SqlParameter>
            {
                new SqlParameter(DatabaseConstants.PAR_USER_ID, userId),
                new SqlParameter(DatabaseConstants.PAR_AMOUNT_CURRENCY_ID, amountCurrencyId),
                new SqlParameter(DatabaseConstants.PAR_ACCOUNT_ID, originAccountId),
                new SqlParameter(DatabaseConstants.PAR_DESTINATION_CURRENCY_ID, destinationCurrencyId)
            };

            var dataSet = ExecuteStoredProcedure(DatabaseConstants.SP_ACCOUNT_DEFAULT_CURRENCY_VALUES_LIST, parameters);
            if (dataSet == null || dataSet.Tables.Count == 0)
                return 0;
            return Utilities.DataRowConvert.ToInt(dataSet.Tables[0].Rows[0],
                DatabaseConstants.COL_ACCOUNT_CURRENCY_CONVERTER_METHOD_ID);
        }

        public void AddTransferRecord(IEnumerable<int> spendIds, string userId)
        {
            if (spendIds == null || !spendIds.Any())
                return;

            var strSpendIds = ServicesUtils.CreateStringCharSeparated(spendIds);
            var parameters = new List<SqlParameter>
            {
                new SqlParameter(DatabaseConstants.PAR_USER_ID, userId),
                new SqlParameter(DatabaseConstants.PAR_SPEND_IDS, strSpendIds)
            };

            ExecuteStoredProcedure(DatabaseConstants.SP_TRANSFER_RECORD_ADD, parameters);
        }

        #endregion

        public new void BeginTransaction()
        {
            base.BeginTransaction();
        }

        public new void RollbackTransaction()
        {
            base.RollbackTransaction();
        }

        public new void Commit()
        {
            base.Commit();
        }
    }
}
