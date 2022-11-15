using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFinanceBackend.Data;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;
using Serilog;

namespace MyFinanceBackend.Services
{
	public interface IScheduledTasksService
	{
		Task CreateBasicTrxAsync(
			string userId,
			ClientScheduledTask.Basic clientScheduledTask
		);

		Task CreateTransferTrxAsync(
			string userId,
			ClientScheduledTask.Transfer clientScheduledTask
		);

		Task<IReadOnlyCollection<BaseScheduledTaskVm>> GetScheduledTasksByUserIdAsync(string userId);
		Task<IReadOnlyCollection<ExecutedTaskViewModel>> GetExecutedTasksByTaskIdAsync(string taskId);
		Task DeleteByIdAsync(string taskId);

		Task<TaskExecutedResult> ExecuteScheduledTaskAsync(
			string taskId,
			DateTime dateTime,
			ExecuteTaskRequestType requestType,
			string userId
		);
	}

	public class ScheduledTasksService : IScheduledTasksService
	{
		private readonly IAutomaticTaskRepository _automaticTaskRepository;
		private readonly IAccountRepository _accountRepository;
		private readonly ISpendsService _spendsService;
		private readonly ITransferService _transferService;
		private readonly ILogger _logger;

		public ScheduledTasksService(
			IAutomaticTaskRepository automaticTaskRepository,
			IAccountRepository accountRepository,
			ISpendsService spendsService,
			ITransferService transferService,
			ILogger logger
		)
		{
			_automaticTaskRepository = automaticTaskRepository;
			_accountRepository = accountRepository;
			_spendsService = spendsService;
			_transferService = transferService;
			_logger = logger;
		}

		public async Task<TaskExecutedResult> ExecuteScheduledTaskAsync(
			string taskId,
			DateTime dateTime,
			ExecuteTaskRequestType requestType,
			string userId
		)
		{
			var scheduledTasks = await _automaticTaskRepository.GetScheduledByTaskIdAsync(taskId);
			if (scheduledTasks == null || !scheduledTasks.Any())
			{
				return TaskExecutedResult.Error($"TaskId: {taskId} does not exist");
			}

			try
			{
				TaskExecutedResult taskExecutedResult;
				var scheduledTask = scheduledTasks.First();
				//_automaticTaskRepository.BeginTransaction();
				switch (scheduledTask)
				{
					case BasicScheduledTaskVm baseScheduledTaskVm:
						taskExecutedResult =
							await ExecuteBasicScheduledTaskAsync(baseScheduledTaskVm, dateTime, requestType);
						break;
					case TransferScheduledTaskVm transferScheduledTaskVm:
						taskExecutedResult =
							await ExecuteTransferScheduledTaskAsync(transferScheduledTaskVm, dateTime, requestType);
						break;
					default:
						taskExecutedResult = TaskExecutedResult.Error("Task not supported");
						break;
				}

				await RecordExecutedTaskAsync(scheduledTask, taskExecutedResult, dateTime, userId);
				//_automaticTaskRepository.Commit();
				return taskExecutedResult;
			}
			catch (Exception e)
			{
				_logger.Error(e.ToString());
				//_automaticTaskRepository.RollbackTransaction();
				throw;
			}
		}

		public async Task CreateBasicTrxAsync(
			string userId,
			ClientScheduledTask.Basic clientScheduledTask
		)
		{
			await _automaticTaskRepository.InsertBasicTrxAsync(userId, clientScheduledTask);
		}

		public async Task CreateTransferTrxAsync(
			string userId,
			ClientScheduledTask.Transfer clientScheduledTask
		)
		{
			await _automaticTaskRepository.InsertTransferTrxAsync(userId, clientScheduledTask);
		}

		public async Task<IReadOnlyCollection<BaseScheduledTaskVm>> GetScheduledTasksByUserIdAsync(string userId)
		{
			return await _automaticTaskRepository.GetScheduledByUserIdAsync(userId);
		}

		public async Task<IReadOnlyCollection<ExecutedTaskViewModel>> GetExecutedTasksByTaskIdAsync(string taskId)
		{
			return await _automaticTaskRepository.GetExecutedTasksByTaskIdAsync(taskId);
		}

		public async Task DeleteByIdAsync(string taskId)
		{
			await _automaticTaskRepository.DeleteByIdAsync(taskId);
		}

		private async Task RecordExecutedTaskAsync(
			BaseScheduledTaskVm scheduledTaskVm,
			TaskExecutedResult taskExecutedResult,
			DateTime dateTime,
			string userId
		)
		{
			if (scheduledTaskVm == null)
			{
				throw new ArgumentNullException(nameof(scheduledTaskVm));
			}

			if (taskExecutedResult == null)
			{
				throw new ArgumentNullException(nameof(taskExecutedResult));
			}

			var clientExecutedTask = new ClientExecutedTask
			{
				AutomaticTaskId = scheduledTaskVm.Id.ToString(),
				ExecuteDatetime = dateTime,
				ExecutionStatus = taskExecutedResult.Status,
				ExecutionMsg = taskExecutedResult.ErrorMsg,
				ExecutedByUserId = userId
			};

			await _automaticTaskRepository.RecordClientExecutedTaskAsync(clientExecutedTask);
		}

		private async Task<TaskExecutedResult> ExecuteTransferScheduledTaskAsync(
			TransferScheduledTaskVm transferScheduledTask,
			DateTime dateTime,
			ExecuteTaskRequestType requestType
		)
		{
			if (transferScheduledTask == null)
			{
				throw new ArgumentNullException(nameof(transferScheduledTask));
			}

			if (transferScheduledTask.AccountId == 0)
			{
				return TaskExecutedResult.Error("Invalid accountId");
			}

			if (transferScheduledTask.ToAccountId == 0)
			{
				return TaskExecutedResult.Error("Invalid to accountId");
			}

			var currentAccountPeriod =
				await _accountRepository.GetAccountPeriodInfoByAccountIdDateTimeAsync(transferScheduledTask.AccountId,
					dateTime);
			var transferRequest = new TransferClientViewModel
			{
				SpendDate = dateTime,
				Description = GetExecuteTaskDescription(transferScheduledTask.Description, requestType,
					transferScheduledTask.TaskType),
				AccountPeriodId = currentAccountPeriod.AccountPeriodId,
				SpendTypeId = transferScheduledTask.SpendTypeId,
				CurrencyId = transferScheduledTask.CurrencyId,
				IsPending = false,
				Amount = transferScheduledTask.Amount,
				UserId = currentAccountPeriod.UserId,
				AmountTypeId = TransactionTypeIds.Ignore,
				BalanceType = BalanceTypes.Custom,
				DestinationAccount = transferScheduledTask.ToAccountId
			};

			_transferService.SubmitTransfer(transferRequest);
			return TaskExecutedResult.Success();
		}

		private async Task<TaskExecutedResult> ExecuteBasicScheduledTaskAsync(
			BasicScheduledTaskVm basicScheduledTaskVm,
			DateTime dateTime,
			ExecuteTaskRequestType requestType
		)
		{
			if (basicScheduledTaskVm == null)
			{
				throw new ArgumentNullException(nameof(basicScheduledTaskVm));
			}

			if (basicScheduledTaskVm.AccountId == 0)
			{
				return TaskExecutedResult.Error("Invalid accountId");
			}

			var currentAccountPeriod =
				await _accountRepository.GetAccountPeriodInfoByAccountIdDateTimeAsync(basicScheduledTaskVm.AccountId,
					dateTime);
			if (currentAccountPeriod == null || currentAccountPeriod.AccountPeriodId == 0)
			{
				return TaskExecutedResult.Error("No current period");
			}

			var basicTrxCreate = new ClientBasicTrxByPeriod
			{
				AmountTypeId = basicScheduledTaskVm.IsSpend ? TransactionTypeIds.Spend : TransactionTypeIds.Saving,
				SpendDate = dateTime,
				Description = GetExecuteTaskDescription(basicScheduledTaskVm.Description, requestType,
					basicScheduledTaskVm.TaskType),
				AccountPeriodId = currentAccountPeriod.AccountPeriodId,
				SpendTypeId = basicScheduledTaskVm.SpendTypeId,
				CurrencyId = basicScheduledTaskVm.CurrencyId,
				IsPending = false,
				Amount = basicScheduledTaskVm.Amount,
				UserId = currentAccountPeriod.UserId
			};

			_spendsService.AddBasicTransaction(basicTrxCreate, basicTrxCreate.AmountTypeId);
			return TaskExecutedResult.Success();
		}

		private static string GetExecuteTaskDescription(
			string baseDescription,
			ExecuteTaskRequestType requestType,
			ScheduledTaskType scheduledTaskType
		)
		{
			baseDescription = string.IsNullOrWhiteSpace(baseDescription) ? "No Desc" : baseDescription;
			return $"AUTO_TASK_{scheduledTaskType}_{baseDescription}_{requestType}";
		}
	}
}
