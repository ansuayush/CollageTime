using ExecViewHrk.WebUI.Helpers;
using System;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomErrorHandler()); 
            //{ View = "Error" });
        }
    }
}