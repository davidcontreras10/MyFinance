using System;

namespace MyFinance.MyFinanceModel
{
	public interface IMyFinanceSettings
	{
		public string CurrencyServiceUrl { get; }
		public TimeSpan ResetPasswordTokenExpireTime { get; }
		public string SavingAmountTypeName { get; }
		public string SpendAmountTypeName { get; }
	}
}
