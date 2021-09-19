using System.Collections.Generic;
using System.Data.SqlClient;
using MyFinance.Backend.Constants;
using MyFinance.Backend.Services;
using MyFinance.DataAccess;
using MyFinance.MyFinanceModel;

namespace MyFinance.Backend.Data
{
	public class ResourceAccessRepository : SqlServerBaseService, IResourceAccessRepository
	{
		#region Constructor

		public ResourceAccessRepository(IConnectionConfig config) : base(config)
		{
		}

		#endregion

		#region Public Methods

		public IEnumerable<ResourceAccessReportRow> GetResourceAccessReport(int? applicationResourceId, int? applicationModuleId, 
			int? resourceActionId, int? resourceAccessLevelId)
		{
			var parameters = new List<SqlParameter>();
			if(applicationResourceId != null)
			{
				parameters.Add(new SqlParameter(DatabaseConstants.PAR_APPLICATION_RESOURCE_ID, applicationResourceId));
			}

			if (applicationModuleId != null)
			{
				parameters.Add(new SqlParameter(DatabaseConstants.PAR_APPLICATION_MODULE_ID, applicationModuleId));
			}

			if (resourceActionId != null)
			{
				parameters.Add(new SqlParameter(DatabaseConstants.PAR_RESOURCE_ACTION_ID, resourceActionId));
			}

			if (resourceAccessLevelId != null)
			{
				parameters.Add(new SqlParameter(DatabaseConstants.PAR_RESOURCE_ACCESS_LEVEL_ID, resourceAccessLevelId));
			}

			var dataSet = ExecuteStoredProcedure(DatabaseConstants.SP_RESOURCE_ACCESS_REPORT, parameters);
			if(dataSet == null || dataSet.Tables.Count == 0)
			{
				return new ResourceAccessReportRow[0];
			}

			var result = ServicesUtils.CreateGenericList(dataSet.Tables[0], ServicesUtils.CreateResourceAccessReportRow);
			return result;
		}

		#endregion
	}
}
