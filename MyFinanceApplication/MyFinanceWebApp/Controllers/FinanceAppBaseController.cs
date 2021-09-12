using MyFinanceWebApp.CustomHandlers;
using MyFinanceWebApp.Helpers;
using MyFinanceWebApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Utilities;

namespace MyFinanceWebApp.Controllers
{
    public abstract class FinanceAppBaseController : Controller
    {
        #region Protecte utils

        protected ActionResult JsonCamelCaseResult(object data, JsonRequestBehavior jsonRequestBehavior = JsonRequestBehavior.AllowGet)
        {
            return new JsonCamelCaseResult(data, jsonRequestBehavior);
        }

        protected IEnumerable<int> GetIdsCollection(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return new List<int>();
            var idsArray = ids.Split(',');
            var idIntArray = idsArray.Select(StringUtilities.ConverToInt);
            return idIntArray;
        }

		protected IEnumerable<int> GetIdsCollection(IEnumerable<string> ids)
		{
			return ids?.Select(int.Parse) ?? new List<int>();
		}

        protected string GetUserToken()
        {
            var cookie = Request.Cookies["TokenAuthorization"];
            var authTokenEncrypted = cookie != null ? cookie.Values["AuthToken"] : "";
            var userId = User.Identity.Name;
            var authToken = LocalHelper.UnProtect(authTokenEncrypted, userId);
            return authToken;
        }

        protected MainHeaderModel CreateMainHeaderModel(PageHeaderBuilder.AppMenuItem appMenuItem)
        {
            return PageHeaderBuilder.GetHeader(Url, appMenuItem);
        }

        #endregion
    }
}