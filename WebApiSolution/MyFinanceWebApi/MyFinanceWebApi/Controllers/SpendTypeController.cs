using System.Collections.Generic;
using System.Web.Http;
using MyFinanceBackend.Services;
using MyFinanceModel.ViewModel;
using MyFinanceModel.ClientViewModel;
using System;
using MyFinanceWebApi.CustomHandlers;

namespace MyFinanceWebApi.Controllers
{
    public class SpendTypeController : BaseController
    {
        #region Attributes

        private readonly ISpendTypeService _spendTypeService;
        private const string ROOT_ROUTE = "api/SpendType/";

        #endregion

        #region Constructor

        public SpendTypeController(ISpendTypeService spendTypeService)
        {
            _spendTypeService = spendTypeService;
        }

        #endregion

        #region Routes

        [Route(ROOT_ROUTE)]
        [HttpGet]
        public IEnumerable<SpendTypeViewModel> GetSpendTypes(bool includeAll = true)
        {
            var userId = GetUserId();
            var result = _spendTypeService.GeSpendTypes(userId, includeAll);
            return result;
        }

        [Route(ROOT_ROUTE)]
        [HttpDelete]
        public IHttpActionResult DeleteSpendType([FromBody]ClientSpendTypeId clientSpendTypeId)
        {
            var userId = GetUserId();
            _spendTypeService.DeleteSpendType(userId, clientSpendTypeId.SpendTypeId);
            return Ok();
        }

        [Route(ROOT_ROUTE)]
        [HttpPost]
        public IEnumerable<int> AddSpendType(ClientAddSpendType spendType)
        {
            if (spendType == null)
            {
                throw new ArgumentNullException("spendType");
            }

            var userId = GetUserId();
            spendType.SpendTypeId = 0;
            var result = _spendTypeService.AddEditSpendTypes(userId, spendType);
            return result;
        }

        [ValidateModelState]
        [Route(ROOT_ROUTE)]
        [HttpPatch]
        public IEnumerable<int> EditSpendType(ClientEditSpendType spendType)
        {
            if (spendType == null)
            {
                throw new ArgumentNullException("spendType");
            }

            if (spendType.SpendTypeId < 1)
            {
                throw new ArgumentException("Id cannot be zero or less", "spendType");
            }

            var userId = GetUserId();
            var result = _spendTypeService.AddEditSpendTypes(userId, spendType);
            return result;
        }

        [Route(ROOT_ROUTE + "user")]
        [HttpPost]
        public IEnumerable<int> AddSpendTypeUser([FromBody] ClientSpendTypeId clientSpendTypeId)
        {
            var userId = GetUserId();
            var result = _spendTypeService.AddSpendTypeUser(userId, clientSpendTypeId.SpendTypeId);
            return result;
        }

        [Route(ROOT_ROUTE + "user")]
        [HttpDelete]
        public IEnumerable<int> DeleteSpendTypeUser([FromBody] ClientSpendTypeId clientSpendTypeId)
        {
            var userId = GetUserId();
            var result = _spendTypeService.DeleteSpendTypeUser(userId, clientSpendTypeId.SpendTypeId);
            return result;
        }

        #endregion
    }
}
