using System.Web.Mvc;
using MyFinanceWebApp.Helpers;
using MyFinanceWebApp.Models;

namespace MyFinanceWebApp.Controllers
{
    public class ScheduledTasksController : Controller
    {
        private readonly IHtmlHeaderHelper _htmlHeaderHelper;

        public ScheduledTasksController(IHtmlHeaderHelper htmlHeaderHelper)
        {
            _htmlHeaderHelper = htmlHeaderHelper;
        }
        

        [AllowAnonymous]
        // GET: ScheduledTasks
        public ActionResult Index()
        {
            var model = TempData.ContainsKey("model")
                ? TempData["model"] as ScheduledTasksViewModel
                : new ScheduledTasksViewModel
                {
                    HeaderModel = CreateMainHeaderModel(),
                    HtmlHeaderHelper = _htmlHeaderHelper,
                    RequestType = ScheduleTaskRequestType.View
                };
            return View(model);
        }

        [AllowAnonymous]
        // GET: ScheduledTasks
        public ActionResult New(int? accountId = null)
        {
            TempData["model"] = new ScheduledTasksViewModel
            {
                HeaderModel = CreateMainHeaderModel(),
                HtmlHeaderHelper = _htmlHeaderHelper,
                RequestType = ScheduleTaskRequestType.New
            };

            return RedirectToAction("Index");
        }

        private MainHeaderModel CreateMainHeaderModel()
        {
            return PageHeaderBuilder.GetHeader(Url, PageHeaderBuilder.AppMenuItem.ScheduledTasks);
        }
    }
}