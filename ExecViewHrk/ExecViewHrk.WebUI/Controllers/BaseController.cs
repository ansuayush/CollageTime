using Kendo.Mvc.UI;
using Newtonsoft.Json;
using System.Configuration;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // call the base method first
            base.OnActionExecuting(filterContext);

            // if client database connection string is not configures , force them to the login screen
            if (string.IsNullOrEmpty(User.Identity.GetClientConnectionString()))
            {
                filterContext.Result = new RedirectResult(Url.Action("Login", "Account"));
            }
        }

        public ContentResult KendoCustomResult(DataSourceResult data, JsonRequestBehavior obj)
        {
            return Content(JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss" }), "application/json");
        }
    }
}