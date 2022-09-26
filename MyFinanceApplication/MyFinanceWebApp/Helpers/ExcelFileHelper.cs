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
			var title = $"Account: {accountFinanceViewModel.AccountName} -- {accountFinanceViewModel.AccountPeriodName}";
			excelWorksheet.Cells[1, 1].Value = title;
			const int startRowPosition = 2;
			const int startColPosition = 1;
			WriteSpendsList(excelWorksheet, accountFinanceViewModel.SpendViewModels, startRowPosition, startColPosition,
				accountFinanceViewModel.CurrencyId, accountFinanceViewModel.CurrencySymbol);
		}

		private static void WriteSpendsList(
			ExcelWorksheet worksheet,
			IReadOnlyCollection<SpendViewModel> spendViewModels,
			int startRow,
			int startCol,
			int accountCurrencyId,
			string accountCurrencySymbol
		)
		{
			var rowPos = startRow;
			var colPos = startCol;
			foreach (var spendViewModel in spendViewModels)
			{
				var baseCells = GetSpendViewModelLine(spendViewModel, accountCurrencyId, accountCurrencySymbol);
				foreach (var baseExcelCell in baseCells)
				{
					baseExcelCell.GenerateExcelCell(worksheet.Cells[rowPos, colPos]);
					colPos++;
				}

				rowPos++;
				colPos = startCol;
			}
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