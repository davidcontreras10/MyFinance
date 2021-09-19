using System.Collections.Generic;
using MyFinance.MyFinanceModel;

namespace MyFinance.Backend.Data
{
	public interface IResourceAccessRepository
	{
		IEnumerable<ResourceAccessReportRow> GetResourceAccessReport(int? applicationResourceId, int? applicationModuleId, int? resourceActionId, int? resourceAccessLevelId);
	}
}