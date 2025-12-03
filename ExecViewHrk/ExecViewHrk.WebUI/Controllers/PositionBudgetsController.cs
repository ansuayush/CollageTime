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
using ExecViewHrk.WebUI.Services;
using ExecViewHrk.WebUI.Helpers;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Validation;
using ExecViewHrk.Models;
using System.Text;
using System.IO;
using System.Configuration;
using ExecViewHrk.Domain.Helper;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PositionBudgetsController : Controller
    {
        // GET: PositionBudgets
        public ActionResult Index()
        {
            PositionBudgetsVM model = PositionBudgetListimport();
            return View(model);
        }

        public PositionBudgetsVM PositionBudgetListimport()
        {
            PositionBudgetsVM positionBudget = new PositionBudgetsVM();
            return positionBudget;
        }

        public ActionResult PositionBudgetList()
        {
            var viewName = "PositionBudgets";
            PositionBudgetsVM positionBudget = new PositionBudgetsVM();

            return View(viewName, positionBudget);
        }

        public ActionResult PositionBudgetsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            {
                var PositionBudgetList = (from pb in clientDbContext.PositionsBudgets
                                          join p in clientDbContext.Positions on pb.PositionID equals p.PositionId
                                          where pb.PositionID == p.PositionId
                                          select new PositionBudgetsVM
                                          {
                                              ID = pb.ID,
                                              PositionID = pb.PositionID,
                                              BudgetYear = pb.BudgetYear,
                                              BudgetMonth = pb.BudgetMonth,
                                              BudgetAmount = pb.BudgetAmount,
                                              FTE = pb.FTE,
                                              PositionTitle = p.Title,
                                              BudgetMonthText = (pb.BudgetMonth == 1 ? "Jan" :
                                                                  pb.BudgetMonth == 2 ? "Feb" :
                                                                  pb.BudgetMonth == 3 ? "March " :
                                                                  pb.BudgetMonth == 4 ? "April" :
                                                                  pb.BudgetMonth == 5 ? "May" :
                                                                  pb.BudgetMonth == 6 ? "June" :
                                                                  pb.BudgetMonth == 7 ? "July" :
                                                                  pb.BudgetMonth == 8 ? "Aug" :
                                                                  pb.BudgetMonth == 9 ? "Sep" :
                                                                  pb.BudgetMonth == 10 ? "Oct" :
                                                                  pb.BudgetMonth == 11 ? "Nov" :
                                                                  pb.BudgetMonth == 12 ? "Dec" : "")
                                          }).ToList();

                return Json(PositionBudgetList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PositionBudgetsList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.PositionBudgets e_PositionSalaryHistory)
        {


            return Json("");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PositionBudgetsList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.PositionBudgets PositionBudget)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (PositionBudget != null && ModelState.IsValid)
                {
                    var PositionBudgetDb = clientDbContext.PositionsBudgets
                        .Where(x => x.PositionID == PositionBudget.PositionID)
                        .SingleOrDefault();

                    if (PositionBudgetDb != null)
                    {
                        PositionBudgetDb.PositionID = PositionBudget.PositionID;
                        PositionBudgetDb.BudgetYear = PositionBudget.BudgetYear;
                        PositionBudgetDb.BudgetMonth = PositionBudget.BudgetMonth;
                        PositionBudgetDb.BudgetAmount = PositionBudget.BudgetAmount;
                        PositionBudgetDb.FTE = PositionBudget.FTE;
                        clientDbContext.SaveChanges();
                    }
                }
                return Json(new[] { PositionBudget }.ToDataSourceResult(request, ModelState));
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PositionBudgetDelete(int ID)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.PositionsBudgets.Where(x => x.ID == ID).SingleOrDefault();
            if (dbRecord != null)
            {
                clientDbContext.PositionsBudgets.Remove(dbRecord);
                try
                {
                    clientDbContext.SaveChanges();
                }
                catch// (Exception ex)
                {
                    return Json(new { Message = CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE, succeed = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Message = "Record does not exist.", succeed = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult PositionBudgetsDetail(int Id)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PositionBudgetsVM PositionBudgetVM = new PositionBudgetsVM();
            PositionBudgetVM = (from pb in clientDbContext.PositionsBudgets
                                join p in clientDbContext.Positions on pb.PositionID equals p.PositionId
                                where pb.PositionID == p.PositionId && pb.ID == Id
                                select new PositionBudgetsVM
                                {
                                    ID = pb.ID,
                                    PositionID = pb.PositionID,
                                    BudgetYear = pb.BudgetYear,
                                    BudgetMonth = pb.BudgetMonth,
                                    BudgetAmount = pb.BudgetAmount,
                                    FTE = pb.FTE,
                                    PositionTitle = p.Title,
                                    BudgetMonthText = (pb.BudgetMonth == 1 ? "Jan" :
                                                             pb.BudgetMonth == 2 ? "Feb" :
                                                             pb.BudgetMonth == 3 ? "March " :
                                                             pb.BudgetMonth == 4 ? "April" :
                                                             pb.BudgetMonth == 5 ? "May" :
                                                             pb.BudgetMonth == 6 ? "June" :
                                                             pb.BudgetMonth == 7 ? "July" :
                                                             pb.BudgetMonth == 8 ? "Aug" :
                                                             pb.BudgetMonth == 9 ? "Sep" :
                                                             pb.BudgetMonth == 10 ? "Oct" :
                                                             pb.BudgetMonth == 11 ? "Nov" :
                                                             pb.BudgetMonth == 12 ? "Dec" : "")
                                }).FirstOrDefault();

            PositionBudgetVM.lstMonths = getMonths();
            PositionBudgetVM.lstyears = getYears();
            return View(PositionBudgetVM);
        }

        [HttpPost]
        public ActionResult PositionBudgetSaveAjax(PositionBudgetsVM positionBudgetVm)
        {
            bool recordIsNew = false;
            int positionID = positionBudgetVm.PositionID;
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PositionBudgets positionBudgts = clientDbContext.PositionsBudgets.Where(x => x.ID == positionBudgetVm.ID).SingleOrDefault();
            PositionBudgetsVM oPositionBudgets = new PositionBudgetsVM();
            PositionBudgetService budgetService = new PositionBudgetService();
            if (ModelState.IsValid)
            {

                try
                {
                    var budget = budgetService.saveBudgetEntity(positionBudgetVm);
                    ViewBag.AlertMessage = recordIsNew == true ? "New Job Position Budget  Added" : "Position Budget Saved";
                }
                catch (Exception err)
                {
                    string _message = "";
                    IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                    if (errors.Count() == 0)
                    {
                        _message += err.InnerException.InnerException.Message;
                        if (_message.Contains(" Cannot insert duplicate key"))
                        {
                            return Json(new { Message = CustomErrorMessages.ERROR_DUPLICATE_RECORD, succeed = false }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        foreach (DbEntityValidationResult error in errors)
                        {
                            foreach (var valError in error.ValidationErrors)
                            {
                                if (_message != "") _message += "<br />";
                                _message += valError.ErrorMessage;
                            }
                        }
                    }
                    return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                string _message = "";
                foreach (var item in modelStateErrors)
                {
                    if (_message != "") _message += "<br />";
                    _message += item.ErrorMessage;
                }
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            }

            //return Json("");
            return Json(new { positionBudgetVm, succeed = true, recordIsNew }, JsonRequestBehavior.AllowGet);
        }

        public List<PositionBudgetsVM> GetAllForPosition(int PositionId)
        {
            List<PositionBudgetsVM> lstPositionBudget = new List<PositionBudgetsVM>();
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            {
                lstPositionBudget = (from pb in clientDbContext.PositionsBudgets
                                     join p in clientDbContext.Positions on pb.PositionID equals p.PositionId
                                     where pb.PositionID == PositionId
                                     select new PositionBudgetsVM
                                     {
                                         PositionID = pb.PositionID,
                                         BudgetYear = pb.BudgetYear,
                                         BudgetMonth = pb.BudgetMonth,
                                         BudgetAmount = pb.BudgetAmount,
                                         FTE = pb.FTE,
                                         BudgetMonthText = (pb.BudgetMonth == 1 ? "Jan" :
                                                             pb.BudgetMonth == 2 ? "Feb" :
                                                             pb.BudgetMonth == 3 ? "March " :
                                                             pb.BudgetMonth == 4 ? "April" :
                                                             pb.BudgetMonth == 5 ? "May" :
                                                             pb.BudgetMonth == 6 ? "June" :
                                                             pb.BudgetMonth == 7 ? "July" :
                                                             pb.BudgetMonth == 8 ? "Aug" :
                                                             pb.BudgetMonth == 9 ? "Sep" :
                                                             pb.BudgetMonth == 10 ? "Oct" :
                                                             pb.BudgetMonth == 11 ? "Nov" :
                                                             pb.BudgetMonth == 12 ? "Dec" : "")
                                     }).ToList();

                return lstPositionBudget;
            }
        }

        public ActionResult AddEditPositionBudgets(int ID)
        {
            bool isNewRecord = false;
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PositionBudgetsVM PositionBudgetVM = new PositionBudgetsVM();
            if (ID == 0)
            {
                isNewRecord = true;
                PositionBudgetVM.PositionID = 0;
                PositionBudgetVM.isNewRecord = isNewRecord;
            }
            else
            {
                PositionBudgetVM = clientDbContext.PositionsBudgets
                         .Where(x => x.ID == ID)
                         .Select(e => new PositionBudgetsVM
                         {
                             ID = e.ID,
                             PositionID = e.PositionID,
                             BudgetYear = e.BudgetYear,
                             BudgetMonth = e.BudgetMonth,
                             BudgetAmount = e.BudgetAmount,
                             FTE = e.FTE
                         }).FirstOrDefault();
            }
            PositionBudgetVM.lstMonths = getMonths().CleanUp();
            PositionBudgetVM.lstyears = getYears().CleanUp();

            List<DropDownModel> lstPositionsID = new List<DropDownModel>();

            var PositionsTitle = clientDbContext.Positions.OrderBy(x => x.PositionId).ToList();
            foreach (var item in PositionsTitle)
            {
                lstPositionsID.Add(new DropDownModel { keyvalue = item.PositionId.ToString(), keydescription = item.Title == null ? item.PositionCode : item.Title });
            }
            PositionBudgetVM.lstPositionIDs = lstPositionsID.CleanUp();

            return View("_PositionBudgetAddEdit", PositionBudgetVM);
        }

        public List<DropDownModel> getMonths()
        {
            List<DropDownModel> lstDropDownMonths = new List<DropDownModel>();
            lstDropDownMonths.Add(new DropDownModel { keyvalue = "1", keydescription = "January" });
            lstDropDownMonths.Add(new DropDownModel { keyvalue = "2", keydescription = "February" });
            lstDropDownMonths.Add(new DropDownModel { keyvalue = "3", keydescription = "March" });
            lstDropDownMonths.Add(new DropDownModel { keyvalue = "4", keydescription = "April" });
            lstDropDownMonths.Add(new DropDownModel { keyvalue = "5", keydescription = "May" });
            lstDropDownMonths.Add(new DropDownModel { keyvalue = "6", keydescription = "June" });
            lstDropDownMonths.Add(new DropDownModel { keyvalue = "7", keydescription = "July" });
            lstDropDownMonths.Add(new DropDownModel { keyvalue = "8", keydescription = "August" });
            lstDropDownMonths.Add(new DropDownModel { keyvalue = "9", keydescription = "September" });
            lstDropDownMonths.Add(new DropDownModel { keyvalue = "10", keydescription = "October" });
            lstDropDownMonths.Add(new DropDownModel { keyvalue = "11", keydescription = "November" });
            lstDropDownMonths.Add(new DropDownModel { keyvalue = "12", keydescription = "December" });
            return lstDropDownMonths;
        }
        public List<DropDownModel> getYears()
        {

            List<DropDownModel> lstDropDownYears = new List<DropDownModel>();
            lstDropDownYears.Add(new DropDownModel { keyvalue = "2011", keydescription = "2011" });
            lstDropDownYears.Add(new DropDownModel { keyvalue = "2012", keydescription = "2012" });
            lstDropDownYears.Add(new DropDownModel { keyvalue = "2013", keydescription = "2013" });
            lstDropDownYears.Add(new DropDownModel { keyvalue = "2014", keydescription = "2014" });
            lstDropDownYears.Add(new DropDownModel { keyvalue = "2015", keydescription = "2015" });
            lstDropDownYears.Add(new DropDownModel { keyvalue = "2016", keydescription = "2016" });
            lstDropDownYears.Add(new DropDownModel { keyvalue = "2017", keydescription = "2017" });
            lstDropDownYears.Add(new DropDownModel { keyvalue = "2018", keydescription = "2018" });
            lstDropDownYears.Add(new DropDownModel { keyvalue = "2019", keydescription = "2019" });
            lstDropDownYears.Add(new DropDownModel { keyvalue = "2020", keydescription = "2020" });
            return lstDropDownYears;
        }

        #region positionbudgetimport
        [HttpPost]
        public ActionResult ImportData(HttpPostedFileBase postedFile)
        {
            if (postedFile == null)
            {
                TempData["Feedback"] = "Please select a file";
            }
            else
            {
                StringBuilder sbAllResults = new StringBuilder();
                StringBuilder sbErrors = new StringBuilder();
                int nLoopCount = 0;
                string path = string.Empty;
                if (postedFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(postedFile.FileName);
                    string[] fileNameArray = fileName.Split('.');
                    fileName = fileNameArray[0] + "_" + DateTime.Now.ToShortDateString() + "-" + DateTime.Now.Ticks + "." + fileNameArray[1];
                    fileName = fileName.Replace("/", "-");
                    fileName = fileName.Replace(":", "-");
                    path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                    bool bErrorFound = false;
                    try
                    {
                        postedFile.SaveAs(path);
                        nLoopCount = 0;
                        var inputFile = new FileInfo(path);
                        var sheet = TakeIo.Spreadsheet.Spreadsheet.Read(inputFile);
                        string connString = User.Identity.GetClientConnectionString();
                        ClientDbContext clientDbContext = new ClientDbContext(connString);
                        foreach (var row in sheet)
                        {
                            nLoopCount++;
                            if (nLoopCount == 1)
                            {
                                if (row[0].ToLower() != "position id")
                                {
                                    sbErrors.AppendLine("Column A is named: " + row[0] + " and should be named: Position ID.");
                                    bErrorFound = true;
                                }
                                if (row[1].ToLower() != "position code")
                                {
                                    sbErrors.AppendLine("Column B is named: " + row[1] + " and should be named: Position Code.");
                                    bErrorFound = true;
                                }
                                if (row[2].ToLower() != "position title")
                                {
                                    sbErrors.AppendLine("Column C is named: " + row[2] + " and should be named: Position Title.");
                                    bErrorFound = true;
                                }
                                if (row[3].ToLower() != "budget amount")
                                {
                                    sbErrors.AppendLine("Column D is named: " + row[3] + " and should be named: Budget Amount.");
                                    bErrorFound = true;
                                }
                                if (row[4].ToLower() != "fte")
                                {
                                    sbErrors.AppendLine("Column E is named: " + row[4] + " and should be named: FTE.");
                                    bErrorFound = true;
                                }
                                if (row[5].ToLower() != "slots")
                                {
                                    sbErrors.AppendLine("Column F is named: " + row[5] + " and should be named: Slots.");
                                    bErrorFound = true;
                                }
                                if (row[6].ToLower() != "budget year")
                                {
                                    sbErrors.AppendLine("Column G is named: " + row[6] + " and should be named: Budget Year.");
                                    bErrorFound = true;
                                }
                                if (bErrorFound) break;
                            }
                            else
                            {
                                string strPositionId = row[0] == null ? "" : row[0].ToString();
                                if (strPositionId.Length == 0)
                                {
                                    sbErrors.AppendLine(Environment.NewLine + "Position ID is blank.  Record skipped. Record number = " + nLoopCount);
                                    continue;
                                }
                                string strPositionCode = row[1] == null ? "" : row[1].ToString();
                                if (strPositionCode.Length == 0)
                                {
                                    sbErrors.AppendLine(Environment.NewLine + "Position Code is blank.  Record skipped. Record number = " + nLoopCount);
                                    continue;
                                }
                                string strPositionTitle = row[2] == null ? "" : row[2].ToString();
                                if (strPositionTitle.Length == 0)
                                {
                                    sbErrors.AppendLine(Environment.NewLine + "Position Title is blank.  Record skipped. Record number = " + nLoopCount);
                                    continue;
                                }
                                string strBudgetAmount = row[3] == null ? "" : row[3].ToString();
                                if (strBudgetAmount.Length == 0)
                                {
                                    sbErrors.AppendLine(Environment.NewLine + "Budget Amount is not in the correct format. Record skipped. Record number =" + nLoopCount);
                                    continue;
                                }
                                string strFTE = row[4] == null ? "" : row[4].ToString();
                                if (strFTE.Length == 0)
                                {
                                    sbErrors.AppendLine(Environment.NewLine + "FTE is blank. Record skipped. Record number =" + nLoopCount);
                                    continue;
                                }
                                string strSlots = row[5] == null ? "" : row[5].ToString();
                                if (strSlots.Length == 0)
                                {
                                    sbErrors.AppendLine(Environment.NewLine + "Slots is blank.  Record skipped. Record number = " + nLoopCount);
                                    continue;
                                }
                                string strBudgetYear = row[6] == null ? "" : row[6].ToString();
                                if (strBudgetYear.Length == 0)
                                {
                                    sbErrors.AppendLine(Environment.NewLine + "Budget Year is blank.  Record skipped. Record number = " + nLoopCount);
                                    continue;
                                }
                                int budgetyear = Convert.ToInt32(strBudgetYear);
                                int strpositionid = Convert.ToInt32(strPositionId);
                                var positionbudgets = clientDbContext.PositionsBudgets.Where(x => x.PositionID == strpositionid && x.BudgetYear == budgetyear).ToList();
                                if (positionbudgets.Count == 0)
                                {
                                    var ddpositionbudgets = new PositionBudgets();
                                    ddpositionbudgets.PositionID = Convert.ToInt32(strPositionId);
                                    ddpositionbudgets.BudgetYear = budgetyear;
                                    ddpositionbudgets.BudgetAmount = Convert.ToDecimal(strBudgetAmount);
                                    ddpositionbudgets.FTE = Convert.ToDecimal(strFTE);
                                    clientDbContext.PositionsBudgets.Add(ddpositionbudgets);
                                    clientDbContext.SaveChanges();
                                }
                                else
                                {
                                    PositionBudgets objbudget = clientDbContext.PositionsBudgets.Where(x => x.PositionID == strpositionid && x.BudgetYear == budgetyear).FirstOrDefault();
                                    objbudget.BudgetAmount = Convert.ToDecimal(strBudgetAmount);
                                    objbudget.FTE = Convert.ToDecimal(strFTE);
                                    clientDbContext.SaveChanges();
                                }

                                var positioncode = clientDbContext.Positions.Where(x => x.PositionCode == strPositionCode).ToList();
                                if (positioncode.Count == 0)
                                {
                                    var ddpositions = new Position();
                                    ddpositions.PositionCode = strPositionCode;
                                    ddpositions.PositionDescription = strPositionTitle;
                                    ddpositions.Title = strPositionTitle;
                                    ddpositions.TotalSlots = Convert.ToInt32(strSlots);
                                    ddpositions.FTE = Convert.ToInt32(strFTE);
                                    clientDbContext.Positions.Add(ddpositions);
                                    clientDbContext.SaveChanges();
                                }
                                else
                                {
                                    Position objpositions = clientDbContext.Positions.Where(x => x.PositionCode == strPositionCode).FirstOrDefault();
                                    objpositions.PositionDescription = strPositionTitle;
                                    objpositions.Title = strPositionTitle;
                                    objpositions.TotalSlots = Convert.ToInt32(strSlots);
                                    objpositions.FTE = Convert.ToInt32(strFTE);
                                    clientDbContext.SaveChanges();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewData["Feedback"] = ex.Message;
                        sbErrors.AppendLine(ex.Message + System.Environment.NewLine + ex.StackTrace);
                    }
                }
                else
                {
                    ViewData["Feedback"] = "Please select a file";
                    sbErrors.AppendLine(Environment.NewLine + "Please select a file");
                }
                sbAllResults.AppendLine("");
                sbAllResults.AppendLine("*******************************************************************************************************");
                sbAllResults.AppendLine(sbErrors.ToString());
                sbAllResults.AppendLine("");
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                string strFileOut = string.Empty;
                string strResults = sbAllResults.ToString();
                strResults = strResults.Replace("<br />", "");
                writeResultsToFile(strResults, out strFileOut);
                EmailResults(strResults, 1);
            }

            return RedirectToAction("Index");
        }
        private static void writeResultsToFile(string strResults, out string strFileName)
        {
            DirectoryInfo di = null;
            FileStream fsOut = null;
            StreamWriter sw = null;
            string strMapPath = "";
            strFileName = "";
            string strPath = "";
            string strPathAndFile = "";
            try
            {
                string strClientId = System.Configuration.ConfigurationManager.AppSettings["ClientID"].ToString();
                strMapPath = System.Configuration.ConfigurationManager.AppSettings["appRoot"].ToString();
                strPath = strMapPath + "\\ResultsFiles";
                if (!Directory.Exists(strPath))
                    di = Directory.CreateDirectory(strPath);
                strFileName = strClientId + "Import" + DateTime.Now.Ticks + ".txt";
                strPathAndFile = strMapPath + "\\ResultsFiles\\" + strFileName;
                if (System.IO.File.Exists(strPathAndFile))
                    System.IO.File.Delete(strPathAndFile);
                fsOut = System.IO.File.OpenWrite(strPathAndFile);
                sw = new StreamWriter(fsOut);
                sw.WriteLine("Results of import - " + DateTime.Now);
                sw.WriteLine();
                sw.WriteLine(strResults);
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                    sw = null;
                }
                if (fsOut != null)
                {
                    fsOut.Close();
                    fsOut = null;
                }
            }
            catch (Exception err)
            {
            }
            finally
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                }
                if (fsOut != null)
                    fsOut.Close();
            }
        }
        private static void EmailResults(string strResults, int numberOfAttempts)
        {
            string strFrom = ConfigurationManager.AppSettings["fromAddress"].ToString();
            string strTo = ConfigurationManager.AppSettings["toAddress"].ToString();
            EmailProcessorCommunity.Send("", strFrom, strTo, " Pinnacle Position Budgets Import Results", "This is an automated message from the  Pinnacle Position Budgets  Import Task. Results of import are as follows: "
                + Environment.NewLine + Environment.NewLine
                + strResults
                + Environment.NewLine + Environment.NewLine
                + "Number Of Attempts = " + numberOfAttempts.ToString(), false);
            string strToSecond = ConfigurationManager.AppSettings["toAddressTwo"].ToString();
            EmailProcessorCommunity.Send("", strFrom, strToSecond, " Pinnacle Position Budgets  Import Results", "This is an automated message from the  Pinnacle Position Budgets  Import Task. Results of import are as follows: "
                + Environment.NewLine + Environment.NewLine
                + strResults
                + Environment.NewLine + Environment.NewLine
                + "Number Of Attempts = " + numberOfAttempts.ToString(), false);
        }

        #endregion

    }

}