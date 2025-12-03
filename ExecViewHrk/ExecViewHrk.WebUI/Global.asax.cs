using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using ExecViewHrk.WebUI.App_Start;
using ExecViewHrk.Domain.Helper;
using System.Text;
using ExecViewHrk.EfAdmin;
using System.Configuration;
using System.Globalization;
using System.Threading;

namespace ExecViewHrk.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfig.RegisterMappings();
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //odelBinders.Binders[typeof(Employee)] = new EmployeeBinder1();
            //ModelBinders.Binders[typeof(ExecViewHrk.EfClient.Employee)] = new ExecViewHrk.WebUI.Models.ErrorCustomModelBinder();
            //ModelBinderProviders.BinderProviders.Clear();
            // ModelBinderProviders.BinderProviders.Add(new ErrorCustomModelBinderProvider());
            // ModelBinders.Binders.Add(typeof(HomePageModels), new HomeCustomBinder());
        }
               
        protected void Application_Error(Object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            if (exception == null)
                return;
            
            string errMessage = Context.Error.Message ?? null;   // ExceptionMessage
            string errSource = Context.Error.Source ?? null;
            string errStackTrace = Context.Error.StackTrace ?? null;    // - ExceptionStackTrace

            string userName = Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : null; //((Context.User).Identity).Name; 
            
            string url = Context.Request.Url == null ? "" : Context.Request.Url.ToString();
            string urlReferrer = Context.Request.UrlReferrer == null ? "" : Context.Request.UrlReferrer.ToString();
            string userAgent = Context.Request.UserAgent ?? null;
            string physicalPath = Context.Request.PhysicalPath ?? null;

            bool? isLocale = Context.Request.IsLocal;
            string remoteAddress = Context.Request.ServerVariables["REMOTE_ADDR"] ?? null;
            bool? cookiesEnabled = Context.Request.Browser.Cookies;
            bool? javascriptEnabled = Context.Request.Browser.JavaScript;
            string browserName = Context.Request.Browser.Browser ?? null;
            int? browserVersion = Context.Request.Browser.MajorVersion;
            bool? isAuthenticated = Context.Request.IsAuthenticated;
            string _targetSite = Context.Error.TargetSite == null ? "" : Context.Error.TargetSite.ToString();
         
            if (Context.Error.TargetSite.ToString() == "System.Web.Mvc.ActionResult Error()" 
                || errMessage.ToLower().Contains("the layout page"))
                return;

            string controllerName = Context.Request.RequestContext.RouteData.Values["controller"] == null ?
                null : Context.Request.RequestContext.RouteData.Values["controller"].ToString();

            AdminDbContext adminDbContext = new AdminDbContext();
            ErrorLog errorLog = new ErrorLog
            {
                Browser = browserName,
                BrowserVersion = browserVersion,
                CookiesEnabled = cookiesEnabled,
                ExceptionMessage = errMessage,
                ExceptionSource = errSource,
                ExceptionStackTrace = errStackTrace,
                ExceptionTargetSite = _targetSite,
                IsAuthenticated = isAuthenticated,
                IsLocale = isLocale,
                JavaScriptEnabled = javascriptEnabled,
                LogDate = DateTime.Now,
                RequestUrl = url,
                RequestPhysicalPath = physicalPath,
                RequestUrlReferrer = urlReferrer,
                RequestUserAgent = userAgent,
                ServerVariablesRemoteAddress = remoteAddress,
                UserName = userName
            };

            adminDbContext.ErrorLogs.Add(errorLog);

            try
            {
                adminDbContext.SaveChanges();
            }
            catch  { }

            EmailProcessor emailProcessor = new EmailProcessor();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            //sb.AppendLine("Browser Name - " + browserName);
            //sb.AppendLine("Browser Version - " + browserVersion.ToString());
            //sb.AppendLine("Cookies Enabled - " + cookiesEnabled.ToString());
            //sb.AppendLine();
            sb.AppendLine("Error Message - " + errMessage); sb.AppendLine();

            sb.AppendLine("Error Source - " + errSource); sb.AppendLine();

            sb.AppendLine("Stack Trace - " + errStackTrace); sb.AppendLine();

            sb.AppendLine("Target Site - " + _targetSite); sb.AppendLine();

            sb.AppendLine("Is Authenticated - " + isAuthenticated.ToString()); sb.AppendLine();
            sb.AppendLine("Is Locale - " + isLocale.ToString()); sb.AppendLine();
            sb.AppendLine("Is JavaScript Enabled - " + javascriptEnabled.ToString()); sb.AppendLine();
            sb.AppendLine("DateTime - " + DateTime.Now.ToLongDateString()); sb.AppendLine();
            sb.AppendLine("URL - " + url); sb.AppendLine();
            sb.AppendLine("Physical Path - " + physicalPath); sb.AppendLine();
            sb.AppendLine("URL Referrer - " + urlReferrer); sb.AppendLine();
            sb.AppendLine("User Agent - " + userAgent); sb.AppendLine();
            sb.AppendLine("Remote Address - " + remoteAddress); sb.AppendLine();
            sb.AppendLine("UserName - " + userName); sb.AppendLine();
            //emailProcessor.Send("smtp.gmail.com", "anitaz.tomar@gmail.com", "ExecView Exception"
            //    , "An Exception has occurred and has been logged."
            //    + "\n\n"
            //    + sb.ToString());
            string FromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddressTraining"].ToString();
            //emailProcessor.Send("smtp.gmail.com", FromEmailAddress, "ExecView Exception"           
            //    , "An Exception has occurred and has been logged."
            //    + "\n\n"
            //    + sb.ToString());

            bool bIsSessionNew = Session.IsNewSession;

            string _errorString  = sb.ToString();
            Session["APPLICATION_ERROR"] = _errorString;
            ///////Response.Clear();

            // Clear the error
            ///////Server.ClearError();

            // Redirect to a landing page
            //Response.Redirect("home/landing");

            //string baseUrl = Request.Url.ToString().Replace(Request.Url.AbsolutePath, "");

           Response.Redirect("~/Common/Error");


        }     

        //protected void Application_BeginRequest(Object sender, EventArgs e)
        //{
        //    CultureInfo newCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        //    newCulture.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";
        //    newCulture.DateTimeFormat.DateSeparator = "/";
        //    Thread.CurrentThread.CurrentCulture = newCulture;
        //}

    }
}

        //protected void Application_BeginRequest()
        //{
        //    var xxx =  System.Web.HttpContext.Current.Session;
        //}

        //protected void Application_AuthenticateRequest()
        //{
        //    var xxx =  System.Web.HttpContext.Current.Session;
        //}

        //protected void Application_PostAuthenticateRequest()
        //{
        //    var xxx =  System.Web.HttpContext.Current.Session;
        //}

        //protected void Application_AuthorizeRequest()
        //{
        //    var xxx =  System.Web.HttpContext.Current.Session;
        //}

        //protected void Application_PostAcquireRequestState()
        //{
        //    var xxx =  System.Web.HttpContext.Current.Session;
        //}

         
         
        
