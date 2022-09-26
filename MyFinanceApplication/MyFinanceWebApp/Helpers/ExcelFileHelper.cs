using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyFinanceModel.ViewModel;
using MyFinanceWebApp.Models;
using OfficeOpenXml;

namespace MyFinanceWebApp.Helpers
{
	public class ExcelFileHelper
	{
		public static byte[] GenerateFile(IReadOnlyCollection<AccountFinanceViewModel> accounts)
		{
			using (var package = new ExcelPackage())
			{
				foreach (var accountFinanceViewModel in accounts)
				{
					var sheet = package.Workbook.Worksheets.Add(accountFinanceViewModel.AccountName);
					WriteAccountWorksheet(sheet, accountFinanceViewModel);
				}

				return package.GetAsByteArray();
			}
		}

		private static void WriteAccountWorksheet(
			ExcelWorksheet excelWorksheet,
			AccountFinanceViewModel accountFinanceViewModel
		)
		{
			excelWorksheet.Cells[1, 1].Value = accountFinanceViewModel.AccountPeriodName;
		}

		private static IBaseExcelCell[] GetSpendViewModelLine(
			SpendViewModel spendViewModel,
			int accountCurrencyId,
			string accountCurrencySymbol
		)
		{
			var originalAmount = new ExcelCurrencyNumber(spendViewModel.CurrencySymbol, spendViewModel.OriginalAmount);
			var accountAmount = new ExcelCurrencyNumber(accountCurrencySymbol, spendViewModel.ConvertedAmount);
			return new IBaseExcelCell[]
			{
				originalAmount,
				accountAmount,
				new ExcelBasicCell(spendViewModel.Description)
			};
		}
	}
}