//using ExecViewHrk.EfClient;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.EfAdmin;
using ExecViewHrk.EfClient;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Net;
using ExecViewHrk.WebUI.Models;
using System.Data.Entity.Validation;
using System.Globalization;
using ExecViewHrk.WebUI.Helpers;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Collections.ObjectModel;

namespace ExecViewHrk.WebUI.Helpers
{
    public class AccessEmployeeDetails
    {
        internal static Employee EmpDetails(ClientDbContext clientDbContext, string email)
        {
            var aspNetUsersEmail = clientDbContext.AspNetUsers.Where(x => x.UserName == email).Select(x => x.Email).FirstOrDefault();
            Employee empDetails = clientDbContext.Employees
                            .Include("Person")
                            .Where(x => x.Person.eMail == aspNetUsersEmail)
                            .OrderByDescending(x => x.EmploymentNumber).FirstOrDefault();
            return empDetails;
        }

        // Get Employee Datails based on Company Code
        internal static Employee EmpDetails(ClientDbContext clientDbContext, string email, int companyCodeId)
        {
            var aspNetUsersEmail = clientDbContext.AspNetUsers.Where(x => x.UserName == email).Select(x => x.Email).FirstOrDefault();
            Employee empDetails = clientDbContext.Employees
                            .Include("Person")
                            .Where(x => x.Person.eMail == aspNetUsersEmail && x.CompanyCodeId == companyCodeId)
                            .OrderByDescending(x => x.EmploymentNumber).FirstOrDefault();
            return empDetails;
        }
    }
}