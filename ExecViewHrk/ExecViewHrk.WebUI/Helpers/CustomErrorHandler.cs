using ExecViewHrk.EfAdmin;
using System;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Helpers
{
    public class CustomErrorHandler : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            AdminDbContext adminDbContext = new AdminDbContext();
            ErrorLog errorLog = new ErrorLog
            {
                ExceptionMessage = filterContext.Exception.Message.ToString(),
                //ExceptionSource = errSource,
                //ExceptionStackTrace = errStackTrace,
                //ExceptionTargetSite = errTragetSite,
                //IsAuthenticated = isAuthenticated,
                //IsLocale = isLocale,
                //JavaScriptEnabled = javascriptEnabled,
                LogDate = DateTime.Now,
                RequestUrl = filterContext.HttpContext.Request.Url.ToString(),
                //RequestPhysicalPath = physicalPath,
                //RequestUrlReferrer = urlReferrer,
                //RequestUserAgent = userAgent,
                //ServerVariablesRemoteAddress = remoteAddress,
                UserName = filterContext.HttpContext.User.Identity.Name
            };

            adminDbContext.ErrorLogs.Add(errorLog);

            try
            {
                adminDbContext.SaveChanges();
                //filterContext.ExceptionHandled = true;
                //filterContext.HttpContext.Response.Clear();
                //filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch { }
        }
    }
}