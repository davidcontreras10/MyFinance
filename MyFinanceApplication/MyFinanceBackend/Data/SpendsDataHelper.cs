using MyFinanceBackend.Models;
using MyFinanceModel;
using MyFinanceModel.ClientViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyFinanceBackend.Data
{
	public static class SpendsDataHelper
	{
		public static ClientAddSpendCurrencyData CreateClientAddSpendCurrencyData(ClientAddSpendAccount clientAddSpendAccount, int amountCurrencyId)
		{
			return new ClientAddSpendCurrencyData
			{
				AmountCurrencyId = amountCurrencyId,
				AccountId = clientAddSpendAccount.AccountId,
				CurrencyConverterMethodId = clientAddSpendAccount.ConvertionMethodId
			};
		}

		public static AccountCurrencyConverterData CreateAccountCurrencyConverterData(
			ClientAddSpendValidationResultSet clientAddSpendValidationResultSet)
		{
			if (clientAddSpendValidationResultSet == null)
				throw new ArgumentNullException(nameof(clientAddSpendValidationResultSet));
			return new AccountCurrencyConverterData
			{
				AccountId = clientAddSpendValidationResultSet.AccountId,
				AccountCurrencyId = clientAddSpendValidationResultSet.AmountCurrencyId,
				AmountCurrencyId = clientAddSpendValidationResultSet.AmountCurrencyId,
				AccountName = clientAddSpendValidationResultSet.AccountName
			};
		}
	}
}
