using System.Data.SqlClient;
using System.Threading.Tasks;
using DataAccess;
using MyFinanceBackend.Constants;
using MyFinanceBackend.Services;
using MyFinanceModel.ClientViewModel;

namespace MyFinanceBackend.Data
{
	public interface IAutomaticTaskRepository
	{
		Task InsertBasicTrxAsync(
			string userId,
			ClientScheduledTask.Basic clientScheduledTask
		);

		Task InsertTransferTrxAsync(
			string userId,
			ClientScheduledTask.Transfer clientScheduledTask
		);
	}

	public class AutomaticTaskRepository : SqlServerBaseService, IAutomaticTaskRepository
	{
		public AutomaticTaskRepository(IConnectionConfig config) : base(config)
		{
		}

		public async Task InsertBasicTrxAsync(
			string userId,
			ClientScheduledTask.Basic clientScheduledTask
		)
		{
			var sqlDays = ServicesUtils.CreateStringCharSeparated(clientScheduledTask.Days);
			var parameters = new[]
			{
				new SqlParameter(DatabaseConstants.PAR_USER_ID, userId),
				new SqlParameter(DatabaseConstants.PAR_AMOUNT, clientScheduledTask.Amount),
				new SqlParameter(DatabaseConstants.PAR_SPEND_TYPE_ID, clientScheduledTask.SpendTypeId),
				new SqlParameter(DatabaseConstants.PAR_CURRENCY_ID, clientScheduledTask.CurrencyId),
				new SqlParameter(DatabaseConstants.PAR_DESCRIPTION, clientScheduledTask.Description),
				new SqlParameter(DatabaseConstants.PAR_ACCOUNT_ID, clientScheduledTask.AccountId),
				new SqlParameter(DatabaseConstants.PAR_IS_SPEND_TRX, clientScheduledTask.IsSpendTrx),
				new SqlParameter(DatabaseConstants.PAR_PERIOD_TYPE_ID, clientScheduledTask.FrequencyType),
				new SqlParameter(DatabaseConstants.PAR_DAYS, sqlDays)
			};

			await ExecuteStoredProcedureAsync(DatabaseConstants.SP_AUTO_TASK_BASIC_INSERT, parameters);
		}

		public async Task InsertTransferTrxAsync(
			string userId,
			ClientScheduledTask.Transfer clientScheduledTask
		)
		{
			var sqlDays = ServicesUtils.CreateStringCharSeparated(clientScheduledTask.Days);
			var parameters = new[]
			{
				new SqlParameter(DatabaseConstants.PAR_USER_ID, userId),
				new SqlParameter(DatabaseConstants.PAR_AMOUNT, clientScheduledTask.Amount),
				new SqlParameter(DatabaseConstants.PAR_SPEND_TYPE_ID, clientScheduledTask.SpendTypeId),
				new SqlParameter(DatabaseConstants.PAR_CURRENCY_ID, clientScheduledTask.CurrencyId),
				new SqlParameter(DatabaseConstants.PAR_DESCRIPTION, clientScheduledTask.Description),
				new SqlParameter(DatabaseConstants.PAR_ACCOUNT_ID, clientScheduledTask.AccountId),
				new SqlParameter(DatabaseConstants.PAR_TO_ACCOUNT_ID, clientScheduledTask.ToAccountId),
				new SqlParameter(DatabaseConstants.PAR_PERIOD_TYPE_ID, clientScheduledTask.FrequencyType),
				new SqlParameter(DatabaseConstants.PAR_DAYS, sqlDays)
			};

			await ExecuteStoredProcedureAsync(DatabaseConstants.SP_AUTO_TASK_TRANSFER_INSERT, parameters);
		}
	}
}
