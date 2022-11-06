using System.Threading.Tasks;
using MyFinanceBackend.Data;
using MyFinanceModel.ClientViewModel;

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
	}
}
