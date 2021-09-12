using System.Collections.Generic;
using MyFinanceModel;

namespace MyFinanceBackend.Data
{
	public interface IResourceAccessRepository
	{
		IEnumerable<ResourceAccessReportRow> GetResourceAccessReport(int? applicationResourceId, int? applicationModuleId, int? resourceActionId, int? resourceAccessLevelId);
	}
}