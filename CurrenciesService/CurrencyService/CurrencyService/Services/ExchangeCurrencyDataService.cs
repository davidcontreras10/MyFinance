using System.Collections.Generic;
using System.Linq;
using CurrencyService.Models;
using DataAccess;
using System;
using System.Data;
using Ut = Utilities.SystemDataUtilities;
using System.Data.SqlClient;

namespace CurrencyService.Services
{
    public class ExchangeCurrencyDataService : SqlServerBaseService
    {
        #region Private Attributes

        private readonly BccrWebService _bccrWebService;

        #endregion

        #region Constructor

        public ExchangeCurrencyDataService(IConnectionConfig connectionConfig) : base(connectionConfig)
        {
            _bccrWebService = new BccrWebService();
        }

        #endregion

        #region Public Methods

        public IEnumerable<BccrVentanillaModel> GetBccrVentanillaModel(string entityName, DateTime dateTime)
        {
            return GetBccrVentanillaModelWebService(entityName, dateTime);
        }

        public EntityMethodInfo GetEntityMethodInfo(int methodId)
        {
            var methodIdParameter = new SqlParameter("@pMethodId", methodId);
            var dataSet = ExecuteStoredProcedure("EntityNameKeyByMethodIdList", methodIdParameter);
            if (dataSet == null || dataSet.Tables.Count == 0)
                return null;
            return CreateEntityMethodInfo(dataSet.Tables[0]);
        }

        #endregion

        #region Private Methods

        private Dictionary<string, string> GetBccrWebServiceExchangeCodeByEntity(string entityName)
        {
            var entityParameter = new SqlParameter("@pEntityName", entityName);
            var dataSet = ExecuteStoredProcedure("EntityIndicatorCodesByEntityNameList", entityParameter);
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count < 1)
                throw new BccrWebServiceEntityNotFoundException(entityName);
            var dataRow = dataSet.Tables[0].Rows[0];
            var dictionary = new Dictionary<string, string>
            {
                {"sell", Ut.GetString(dataRow, "SellCode")},
                {"purchase", Ut.GetString(dataRow, "PurchaseCode")}
            };
            return dictionary;
        }

        private IEnumerable<BccrVentanillaModel> GetBccrVentanillaModelWebService(string entityName, DateTime dateTime)
        {
            var initialDate = dateTime.AddMonths(-1);
            var endDate = dateTime.AddDays(1);
            var codes = GetBccrWebServiceExchangeCodeByEntity(entityName);
            var sellList = _bccrWebService.GetBccrSingleVentanillaModels(codes["sell"], initialDate, endDate);
            var purchaseList = _bccrWebService.GetBccrSingleVentanillaModels(codes["purchase"], initialDate, endDate);
            var list = CreateBccrVentanillaModel(sellList, purchaseList);
            return list;
        }

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
        private IEnumerable<BccrVentanillaModel> GetBccrVentanillaModelDatabase(string entityName, DateTime dateTime)
        {
            var entityNameParameter = new SqlParameter("@pEntityNameMatch", entityName);
            var dataSet = ExecuteStoredProcedure("SpBccrVentanillaByEntityList", entityNameParameter);
            if (dataSet == null || dataSet.Tables.Count == 0)
                return new List<BccrVentanillaModel>();
            var list = CreateGenericList(dataSet.Tables[0], CreateBccrVentanilla);
            return list;
        }

        private static EntityMethodInfo CreateEntityMethodInfo(DataTable dataTable)
        {
            return dataTable == null || dataTable.Rows.Count == 0 ? null : CreateEntityMethodInfo(dataTable.Rows[0]);
        }

        private static EntityMethodInfo CreateEntityMethodInfo(DataRow dataRow)
        {
            if (dataRow == null)
                throw new ArgumentNullException(nameof(dataRow));
            return new EntityMethodInfo
            {
                Colones = Ut.GetBool(dataRow, "Colones"),
                EntityName = Ut.GetString(dataRow, "EntityName"),
                EntitySearchKey = Ut.GetString(dataRow, "EntitySearchKey")
            };
        }

        private static BccrVentanillaModel CreateBccrVentanilla(DataRow dataRow)
        {
            if (dataRow == null)
                throw new ArgumentNullException(nameof(dataRow));
            return new BccrVentanillaModel
            {
                EntityName = Ut.GetString(dataRow, "Entity"),
                LastUpdate = Ut.GetDateTime(dataRow, "LastUpdate"),
                Purchase = Ut.GetFloat(dataRow, "Purchase"),
                Sell = Ut.GetFloat(dataRow, "Sell")
            };
        }

        private static IEnumerable<T> CreateGenericList<T>(DataTable dataTable, Func<DataRow, T> createMethod)
        {
            return dataTable?.Rows.Cast<DataRow>().Select(createMethod) ?? new List<T>();
        }

        private IEnumerable<BccrVentanillaModel> CreateBccrVentanillaModel(IEnumerable<BccrSingleVentanillaModel> sellList,
            IEnumerable<BccrSingleVentanillaModel> purchaseList)
        {
            var list = new List<BccrVentanillaModel>();
            if (sellList == null || purchaseList == null || sellList.Count() != purchaseList.Count())
                throw new ArgumentException("Lists must match or cannot be null");
            foreach (var singleSell in sellList)
            {
                var singlePurchase = purchaseList.FirstOrDefault(item => item.LastUpdate == singleSell.LastUpdate);
                if (singlePurchase == null)
                    throw new Exception("Invalid BccrSingleVentanillaModel result");
                var model = new BccrVentanillaModel
                {
                    LastUpdate = singleSell.LastUpdate,
                    Purchase = singlePurchase.Value,
                    Sell = singleSell.Value
                };
                list.Add(model);
            }
            return list;
        }
    }

    #endregion
}