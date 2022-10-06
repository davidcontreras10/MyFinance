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
            return View(new ScheduledTasksViewModel
            {
                HeaderModel = CreateMainHeaderModel(),
                HtmlHeaderHelper = _htmlHeaderHelper
            });
        }

        private MainHeaderModel CreateMainHeaderModel()
        {
            return PageHeaderBuilder.GetHeader(Url, PageHeaderBuilder.AppMenuItem.ScheduledTasks);
        }
    }
}