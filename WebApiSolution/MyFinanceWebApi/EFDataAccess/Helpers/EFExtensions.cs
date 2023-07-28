using EFDataAccess.Models;
using MyFinanceModel.ViewModel;

namespace EFDataAccess.Helpers
{
	internal static class EFExtensions
	{
		public static SpendTypeViewModel ToSpendTypeViewModel(this SpendType spendType, int? defaultSpendTypeId)
		{
			return new SpendTypeViewModel
			{
				Description = spendType.Description,
				IsDefault = defaultSpendTypeId != null && defaultSpendTypeId.Value == spendType.SpendTypeId,
				SpendTypeId = spendType.SpendTypeId,
				SpendTypeName = spendType.Name
			};
		}
	}
}
