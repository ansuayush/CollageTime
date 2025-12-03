using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Filters;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    [NoCache]
    public class EmployeeHomeController : Controller
    {
        // GET: EmployeeHome
        public ActionResult Index()
        {
            EmployeeHome emp_Home = new EmployeeHome();
            emp_Home.TimeCardTypeCode = Emp_TimeCardsType();
            return View(emp_Home);
        }

        //TimeCard Fast In Out Fetch records from database
        private string Emp_TimeCardsType()
        {
            var emp_TimecardTypeId = "NS";      //Not set       
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (User.IsInRole("ClientEmployees"))
                {
                    try
                    {
                        var empDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);
                        if (empDetails != null)
                        { emp_TimecardTypeId = clientDbContext.DdlTimeCardTypes.Where(x => x.TimeCardTypeId == empDetails.TimeCardTypeId).Select(x => x.TimeCardTypeCode).SingleOrDefault(); }
                    }
                    catch// (Exception err)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }

                }
            }

            return (emp_TimecardTypeId);
        }
    }
}