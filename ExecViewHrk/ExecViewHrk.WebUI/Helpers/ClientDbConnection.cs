using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExecViewHrk.SqlData;
using ExecViewHrk.WebUI.Infrastructure;

namespace ExecViewHrk.WebUI.Helpers
{
    public class ClientDbConnection
    {
        //public static string clientDbConn = System.Configuration.ConfigurationManager.ConnectionStrings["HrkClientDb"].ConnectionString;
        public static string clientDbConn = HttpContext.Current.User.Identity.GetClientConnectionString();
            //System.Configuration.ConfigurationManager.ConnectionStrings["execView1"].ConnectionString;  
    }
}