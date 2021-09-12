using System.Web.Mvc;
using MyFinanceModel.ClientViewModel;
using MyFinanceWebApp.CustomHandlers;
using MyFinanceWebApp.Helpers;
using MyFinanceWebApp.Models;
using MyFinanceWebApp.Services;

namespace MyFinanceWebApp.Controllers
{
	[HandleTokenError]
    public class SpendTypeController : Controller
    {
        #region Attributes

        private readonly ISpendTypeService _spendTypeService;
        private readonly IHtmlHeaderHelper _htmlHeaderHelper;

        #endregion

        #region Constructor

        public SpendTypeController(ISpendTypeService spendTypeService, IHtmlHeaderHelper htmlHeaderHelper)
        {
            _htmlHeaderHelper = htmlHeaderHelper;
            _spendTypeService = spendTypeService;
        }

        #endregion 

        #region Submit

        
        public ActionResult Index()
        {
            var model = new MainSpendTypePageModel
            {
                HeaderModel = CreateMainHeaderModel(),
                HtmlHeaderHelper = _htmlHeaderHelper
            };

            return View("Index", model);
        }

        #endregion

		#region Json

	    [HttpGet]
	    public ActionResult GetAllData()
	    {
			var authToken = GetUserToken();
            var allSpendTypes = _spendTypeService.GetAllSpendTypes(authToken);
            var userSpendTypes = _spendTypeService.GetUserSpendTypes(authToken);
		    var model = new SpendTypesAllUser
		    {
			    AllSpendTypes = allSpendTypes,
			    UserSpendTypes = userSpendTypes
		    };

		    return Json(model, JsonRequestBehavior.AllowGet);
	    }

		[HttpPost]
		public ActionResult AddSpendType(ClientSpendType clientSpendType)
		{
			var authToken = GetUserToken();
            var result = _spendTypeService.AddSpendTypes(authToken, clientSpendType);
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		
		[HttpPost]
	    public ActionResult EditSpendType(ClientSpendType clientSpendType)
	    {
			var authToken = GetUserToken();
            var result = _spendTypeService.EditSpendTypes(authToken, clientSpendType);
		    return Json(result, JsonRequestBehavior.AllowGet);
	    }

	    [HttpPost]
	    public ActionResult AddSpendTypeUser(ClientSpendTypeId clientSpendTypeId)
	    {
			var authToken = GetUserToken();
            var result = _spendTypeService.AddSpendTypeUser(authToken, clientSpendTypeId.SpendTypeId);
			return Json(result, JsonRequestBehavior.AllowGet);
	    }

		[HttpPost]
		public ActionResult DeleteSpendTypeUser(ClientSpendTypeId clientSpendTypeId)
		{
			var authToken = GetUserToken();
            var result = _spendTypeService.DeleteSpendTypeUser(authToken, clientSpendTypeId.SpendTypeId);
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult DeleteSpendType(ClientSpendTypeId clientSpendTypeId)
		{
			var authToken = GetUserToken();
            _spendTypeService.DeleteSpendType(authToken, clientSpendTypeId.SpendTypeId);
			return Json(new[] {clientSpendTypeId.SpendTypeId}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Privates

		private string GetUserToken()
		{
			var cookie = Request.Cookies["TokenAuthorization"];
			var authTokenEncrypted = cookie != null ? cookie.Values["AuthToken"] : "";
			var userId = User.Identity.Name;
			var authToken = LocalHelper.UnProtect(authTokenEncrypted, userId);
			return authToken;
		}

		private MainHeaderModel CreateMainHeaderModel()
        {
            return PageHeaderBuilder.GetHeader(Url, PageHeaderBuilder.AppMenuItem.SpendType);
        }

        #endregion
    }
}