using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfAdmin;
using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using Kendo.Mvc.Extensions;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Data;
using ExecViewHrk.Models;
using Kendo.Mvc.UI;

namespace ExecViewHrk.WebUI.Controllers
{
    public class TimeOffEmployeesAllowedTakenController : Controller
    {
        readonly ITimeOffEmployeesAllowedTakenRepository _timeOffEmployeesAllowedTakenRepo;

        public TimeOffEmployeesAllowedTakenController(ITimeOffEmployeesAllowedTakenRepository timeOffEmployeesAllowedTakenRepo)
        {

            _timeOffEmployeesAllowedTakenRepo = timeOffEmployeesAllowedTakenRepo;
        }

        public ActionResult TimeOffEmployeesAllowedTakenMatrixPartial()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TimeOffEmployeeTaken(HttpPostedFileBase postedFile)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                //  ClientDbContext clientDbContext = new ClientDbContext(connString);
                List<TimeOffEmployeesAllowedTakenVM> timeofflist = new List<TimeOffEmployeesAllowedTakenVM>();
                try
                {
                    sw.Start();
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    filePath = path + Path.GetFileName(postedFile.FileName);
                    string extension = Path.GetExtension(postedFile.FileName);
                    postedFile.SaveAs(filePath);
                    //Read the contents of CSV file.
                    string csvData = System.IO.File.ReadAllText(filePath);
                    //Execute a loop over the rows.
                    foreach (string item in csvData.Split('\r'))

                    {
                        var row = item.Trim();
                        if (!string.IsNullOrEmpty(row))
                        {
                            if (row.Split(',')[0].ToString() != "EmployeeId")
                            {
                                timeofflist.Add(new TimeOffEmployeesAllowedTakenVM
                                {
                                    // ID = customers.Count() + 1,  
                                    EmployeeId = Convert.ToInt32(row.Split(',')[0]),////////////
                                    CompanyCodeId = Convert.ToInt32(row.Split(',')[1]),////////////
                                    FileNumber = row.Split(',')[2],////////////
                                    TypeId = Convert.ToInt32(row.Split(',')[3]),/////////////
                                    Allowed = Convert.ToDecimal(row.Split(',')[4]),/////////////
                                    Taken = Convert.ToDecimal(row.Split(',')[5]),
                                    Remainder = Convert.ToDecimal(row.Split(',')[6]),///////////                                    
                                });
                            }
                        }
                    }
                    bool result = _timeOffEmployeesAllowedTakenRepo.InsertEmployeesAllowedTaken(timeofflist);
                    ViewBag.Message = "Success";
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    ViewBag.Message = "Failed to Upload Data";
                }
            }
            else
            {
                //return Json(JsonRequestBehavior.DenyGet);
            }

            sw.Stop();

            return View("TimeOffEmployeesAllowedTakenMatrixPartial");
        }


        public ActionResult GetTimeoffSummary_Read([DataSourceRequest]DataSourceRequest request)
        {
            
                var timeoffSummaryList = _timeOffEmployeesAllowedTakenRepo.GetTimeoffSummaryList();
                return Json(timeoffSummaryList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet); ;
            
        }
    }
}