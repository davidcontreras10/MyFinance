using OfficeOpenXml;

namespace MyFinanceWebApp.Models
{
	public interface IBaseExcelCell
	{
		void GenerateExcelCell(ExcelRange excelRange);
	}

	public class ExcelBasicCell : IBaseExcelCell
	{
		private readonly object _value;

		public ExcelBasicCell(object value)
		{
			_value = value;
		}

		public void GenerateExcelCell(ExcelRange excelRange)
		{
			excelRange.Value = _value;
		}
	}

	public class ExcelCurrencyNumber : IBaseExcelCell
	{
		private readonly string _currencySymbol;
		private readonly float _value;
		private string Format => $"{_currencySymbol}#,##0.00";

		public ExcelCurrencyNumber(string currencySymbol, float value)
		{
			_currencySymbol = currencySymbol;
			_value = value;
		}


		public void GenerateExcelCell(ExcelRange excelRange)
		{
			excelRange.Value = _value;
			excelRange.Style.Numberformat.Format = Format;
		}
	}
}