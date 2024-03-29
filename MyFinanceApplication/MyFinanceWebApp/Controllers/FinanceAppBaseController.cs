﻿using MyFinanceWebApp.CustomHandlers;
using MyFinanceWebApp.Helpers;
using MyFinanceWebApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DContre.MyFinance.StUtilities;

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
            var cookie = Request.Cookies[Constants.AuthorizationCookieName];
            var authToken = cookie != null ? cookie.Values[Constants.AuthTokenCookieName] : string.Empty;
            return authToken;
        }

        protected MainHeaderModel CreateMainHeaderModel(PageHeaderBuilder.AppMenuItem appMenuItem)
        {
            return PageHeaderBuilder.GetHeader(Url, appMenuItem);
        }

        #endregion
    }
}