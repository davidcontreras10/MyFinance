using System.ComponentModel.DataAnnotations;

namespace MyFinance.MyFinanceModel.ClientViewModel
{
	public class ClientBasicTrxByPeriod : ClientBasicAddSpend
	{
		[Range(1, int.MaxValue)]
		[Required]
		public int AccountPeriodId { get; set; }
	}
}
