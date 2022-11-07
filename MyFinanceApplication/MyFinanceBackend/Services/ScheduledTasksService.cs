using System.Collections.Generic;
using System.Threading.Tasks;
using MyFinanceBackend.Data;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

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
	}

	public class ScheduledTasksService : IScheduledTasksService
	{
		private readonly IAutomaticTaskRepository _automaticTaskRepository;

		public ScheduledTasksService(IAutomaticTaskRepository automaticTaskRepository)
		{
			_automaticTaskRepository = automaticTaskRepository;
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
			return await _automaticTaskRepository.GetScheduledByUserId(userId);
		}
	}
}
