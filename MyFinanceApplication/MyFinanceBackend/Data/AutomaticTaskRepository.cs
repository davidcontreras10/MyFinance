using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using MyFinanceBackend.Constants;
using MyFinanceBackend.Services;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

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

		Task<IReadOnlyCollection<BaseScheduledTaskVm>> GetScheduledByUserId(string userId);
		Task<IReadOnlyCollection<ExecutedTaskViewModel>> GetExecutedTasksByTaskIdAsync(string taskId);
		Task DeleteByIdAsync(string taskId);
	}

	public class AutomaticTaskRepository : SqlServerBaseService, IAutomaticTaskRepository
	{
		public AutomaticTaskRepository(IConnectionConfig config) : base(config)
		{
		}

		public async Task DeleteByIdAsync(string taskId)
		{
			var taskIdPar = new SqlParameter(DatabaseConstants.PAR_AUTOMATIC_TASK_ID, taskId);
			await ExecuteStoredProcedureAsync(DatabaseConstants.SP_AUTO_TASK_DELETE, taskIdPar);
		}

		public async Task<IReadOnlyCollection<ExecutedTaskViewModel>> GetExecutedTasksByTaskIdAsync(string taskId)
		{
			var taskIdPar = new SqlParameter(DatabaseConstants.PAR_AUTOMATIC_TASK_ID, taskId);
			var dataSet = await ExecuteStoredProcedureAsync(DatabaseConstants.SP_EXECUTED_TASKS_LIST, taskIdPar);
			if (dataSet == null || dataSet.Tables.Count < 1)
			{
				return Array.Empty<ExecutedTaskViewModel>();
			}

			return ServicesUtils.CreateGenericList(dataSet.Tables[0], ServicesUtils.CreateExecutedTaskViewModel).ToList();
		}

		public async Task<IReadOnlyCollection<BaseScheduledTaskVm>> GetScheduledByUserId(string userId)
		{
			var dataSet = await ExecuteStoredProcedureAsync(DatabaseConstants.SP_AUTO_TASK_BY_USER_LIST,
				new SqlParameter(DatabaseConstants.PAR_USER_ID, userId));
			if (dataSet?.Tables == null || dataSet.Tables.Count < 2)
			{
				throw new Exception("Expected two tables");
			}

			var list = ReadBasicScheduledTaskVm(dataSet.Tables[0]) ?? new List<BaseScheduledTaskVm>();
			list.AddRange(ReadTransferScheduledTaskVm(dataSet.Tables[1]));
			return list;
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

		private static List<BaseScheduledTaskVm> ReadBasicScheduledTaskVm(DataTable dataTable)
		{
			return ServicesUtils.CreateGenericList<BaseScheduledTaskVm>(dataTable, ServicesUtils.CreateBasicScheduledTaskVm).ToList();
		}

		private static IEnumerable<BaseScheduledTaskVm> ReadTransferScheduledTaskVm(DataTable dataTable)
		{
			return ServicesUtils.CreateGenericList<BaseScheduledTaskVm>(dataTable, ServicesUtils.CreateTransferScheduledTaskVm);
		}
	}
}
