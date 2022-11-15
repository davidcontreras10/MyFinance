using MyFinanceModel.ViewModel;

namespace MyFinanceModel
{
	public class TaskExecutedResult
	{
		private TaskExecutedResult(){}

		public ExecutedTaskStatus Status { get; set; }
		public string ErrorMsg { get; set; }

		public static TaskExecutedResult Error(string errorMsg)
		{
			return new TaskExecutedResult
			{
				ErrorMsg = errorMsg,
				Status = ExecutedTaskStatus.Failed
			};
		}

		public static TaskExecutedResult Success()
		{
			return new TaskExecutedResult
			{
				Status = ExecutedTaskStatus.Succeeded
			};
		}
	}
}
