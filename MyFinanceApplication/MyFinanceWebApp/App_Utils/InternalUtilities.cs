using System;
using System.Collections.Generic;
using System.Linq;
using MyFinanceModel.ViewModel;
using MyFinanceWebApp.Helpers;
using MyFinanceWebApp.Models;

namespace MyFinanceWebApp.App_Utils
{
	internal class InternalUtilities
	{
		public static void AddBasicTableProperty(IEnumerable<AccountFinanceViewModel> accountFinanceViewModels)
		{
			if(accountFinanceViewModels== null)
				return;
			foreach (var accountFinanceViewModel in accountFinanceViewModels)
			{
				accountFinanceViewModel.SpendTable =
					CreateBasicTable(accountFinanceViewModel.SpendViewModels, accountFinanceViewModel.AccountId,
									 accountFinanceViewModel.CurrencySymbol);
			}
		}

		private static BasicTable CreateBasicTable(IEnumerable<SpendViewModel> spendViewModels, int accountId,
			string currencySymbol)
		{
			var table = new BasicTable
			{
				Title = new BasicTableCell {Value = "Details"},
				Header = new BasicTableRow
				{
					Cells = new List<BasicTableCell>
					{
						new BasicTableCell {Value = "Date"},
						new BasicTableCell {Value = "Amount"},
						new BasicTableCell {Value = "Type"}
					}
				}
			};

			if (spendViewModels == null || !spendViewModels.Any())
				return table;
			foreach (var row in spendViewModels.Select(spend => new BasicTableRow
			{
				Cells = new List<BasicTableCell>
				{
					new BasicTableCell {Value = spend.SpendDate.ToShortDateString()},
					new BasicTableCell {Value = GetSpendHtml(accountId, currencySymbol, spend)},
					new BasicTableCell {Value = spend.SpendTypeName}
				},
				RowId = spend.SpendId,
				InLineAttributes = spend.IsPending ? "style='background: lavenderblush;'" : null
			}))
			{
				table.Rows.Add(row);
			}
			return table;
		}

		public static DateTime FixEndDate(DateTime endDate)
		{
			var dateTime = new DateTime(endDate.Year, endDate.Month, endDate.Day);
			dateTime = dateTime.AddDays(1).AddSeconds(-1);
			return dateTime;
		}

		#region Private Attributes

		private static string GetSpendHtml(int accountId, string currencySymbol, SpendViewModel spendViewModel)
		{
			if (spendViewModel == null)
				return "";
			if (spendViewModel.IsOriginalAmount())
			{
				return spendViewModel.GetHtmlString(currencySymbol);
			}

			return GetSpendConvertedAmountHtml(accountId, spendViewModel.SpendId,
				spendViewModel.GetHtmlString(spendViewModel.CurrencySymbol),
				spendViewModel.GetHtmlStringConverted(currencySymbol));
		}

		private static string GetSpendConvertedAmountHtml(int accountId, int spendId, string original, string converted)
		{
			var span1Id = "spend-converted-value-" + spendId + "-" + accountId;
			const string span1Class = "spend-converted-amount spend-amount-visible";
			var span1 = $"<span id='{span1Id}' class='{span1Class}'>" + converted + "</span>";

			var span2Id = "spend-original-value-" + spendId + "-" + accountId;
			const string span2Class = "spend-original-amount spend-amount-hidden";
			var span2 = $"<span id='{span2Id}' class='{span2Class}'>" + original + "</span>";

			var btnId = "spend-switch-action-" + spendId + "-" + accountId;
			const string btnClass = "spend-action-amount";
			const string btnValue = "";
			var btn =
				$"<input type='button' id='{btnId}' class='{btnClass}' value='{btnValue}' onclick=\"switchSpendValues('{span1Id}','{span2Id}');\"/>";
			var fullHtml = span1 + span2 + btn;
			return fullHtml;
		}

		#endregion
	}
}