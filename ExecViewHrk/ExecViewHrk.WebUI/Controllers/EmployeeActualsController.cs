using ExecViewHrk.Domain.Helper;
using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class EmployeeActualsController : Controller
    {

        public ActionResult Index()
        {
            ViewData["errormessage"] = TempData["Feedback"];
            EmployeeActualsVM model = EmployeeActualsListImport();
            return View(model);
        }

        public EmployeeActualsVM EmployeeActualsListImport()
        {
            EmployeeActualsVM employeeActuals = new EmployeeActualsVM();
            return employeeActuals;
        }

        // GET: EmployeeActuals
        public ActionResult EmployeeActuals()
        {
            var viewName = "EmployeeActuals";
            EmployeeActualsVM employeeActuals = new EmployeeActualsVM();
            return View(viewName, employeeActuals);
        }

        public ActionResult EmployeeActualsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            try
            {
                var EmployeeActualsList = clientDbContext.Database.SqlQuery<EmployeeActualsVM>("GetEmployeeActuals").ToList();
                return Json(EmployeeActualsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region EmployeeActualsImport
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
                DateTime payPeriodDate = DateTime.MaxValue;
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
                                if (row[0].ToLower() != "payroll company code")
                                {
                                    sbErrors.AppendLine("Column A is named: " + row[0] + " and should be named: Payroll Company Code.");
                                    bErrorFound = true;
                                }
                                if (row[1].ToLower() != "file number")
                                {
                                    sbErrors.AppendLine("Column B is named: " + row[1] + " and should be named: File Number.");
                                    bErrorFound = true;
                                }
                                if (row[2].ToLower() != "period end date (pay statements)")
                                {
                                    sbErrors.AppendLine("Column C is named: " + row[2] + " and should be named: Period End Date (Pay Statements).");
                                    bErrorFound = true;
                                }
                                if (row[3].ToLower() != "worked in cost number")
                                {
                                    sbErrors.AppendLine("Column D is named: " + row[3] + " and should be named: Worked in cost number.");
                                    bErrorFound = true;
                                }
                                if (row[4].ToLower() != "worked in cost number description")
                                {
                                    sbErrors.AppendLine("Column E is named: " + row[4] + " and should be named: Worked in Cost Number Description.");
                                    bErrorFound = true;
                                }
                                if (row[5].ToLower() != "gross pay (pay statements)")
                                {
                                    sbErrors.AppendLine("Column F is named: " + row[5] + " and should be named: Gross Pay (Pay Statements).");
                                    bErrorFound = true;
                                }
                                if (row[6].ToLower() != "total hours (pay statements)")
                                {
                                    sbErrors.AppendLine("Column G is named: " + row[6] + " and should be named: Total Hours (Pay Statements).");
                                    bErrorFound = true;
                                }
                                if (bErrorFound) break;
                            }
                            else
                            {
                                string strCompanyCode = row[0] == null ? "" : row[0].ToString();
                                if (strCompanyCode.Length == 0)
                                {
                                    sbErrors.AppendLine(Environment.NewLine + "Payroll Company Code is blank.  Record skipped. Record number = " + nLoopCount);
                                    continue;
                                }

                                string strFileNumber = row[1] == null ? "" : row[1].ToString();
                                strFileNumber = strFileNumber.Trim();
                                if (strFileNumber.Length == 0)
                                {
                                    sbErrors.AppendLine(Environment.NewLine + "File Number is blank.  Record skipped. Record number = " + nLoopCount);
                                    continue;
                                }
                                strFileNumber = strFileNumber.PadLeft(6, '0');

                                string strPayPeriodEndDate = row[2] == null ? "" : row[2].ToString();
                                DateTime PayEndDate = Convert.ToDateTime(strPayPeriodEndDate);
                                DateTime? payPeriodEndDate = ConvertToDateTime(strPayPeriodEndDate);
                                if (payPeriodEndDate == null && strPayPeriodEndDate.Length > 0)
                                {
                                    sbErrors.Append(Environment.NewLine + "Period End Date is not in the correct format or is empty.");
                                    continue;
                                }
                                DateTime? PayDate = payPeriodEndDate;

                                string strGrossWages = row[5] == null ? "" : row[5].ToString();
                                strGrossWages = strGrossWages.Replace("$", "");
                                strGrossWages = strGrossWages.Replace("'", "");
                                Decimal? dGrossWages = ConvertToDecimal(strGrossWages);
                                if (dGrossWages == null)
                                {
                                    sbErrors.Append(Environment.NewLine + "Gross Pay is not in the correct format or is blank. Record skipped. Record number = " + nLoopCount + ". ");
                                    continue;
                                }

                                var dtEmployees = clientDbContext.Database.SqlQuery<EmployeeActualsCCFNVM>(
                                                          "exec dbo.[GetAllForCC_FN] @CompanyCode,@FileNumber",
                                                          new SqlParameter("@CompanyCode", strCompanyCode),
                                                          new SqlParameter("@FileNumber", strFileNumber)
                                                        ).ToList();
                                if (dtEmployees.Count > 1)
                                {
                                    sbErrors.AppendLine(Environment.NewLine + "More than one employee record found for " + strCompanyCode + ", " + strFileNumber + ".  Record skipped. Record number = " + nLoopCount);
                                    continue;
                                }
                                else if (dtEmployees.Count == 0)
                                {
                                    sbErrors.AppendLine(Environment.NewLine + "No Employee record found for " + strCompanyCode + ", " + strFileNumber + ".  Record skipped. Record number = " + nLoopCount + ". ");
                                    continue;
                                }
                                else
                                {
                                    if (payPeriodDate == DateTime.MaxValue && payPeriodEndDate != null)
                                    {
                                        payPeriodDate = (DateTime)payPeriodEndDate;
                                        DateTime PayDt = (DateTime)payPeriodEndDate;
                                        var deleteAllDate = (from c in clientDbContext.EmployeeActuals where c.PayPeriodDate == PayDt select c).ToList();
                                        clientDbContext.EmployeeActuals.RemoveRange(deleteAllDate);
                                        clientDbContext.SaveChanges();
                                    }

                                    try
                                    {
                                        var dtEPositions = clientDbContext.Database.SqlQuery<EPositionsUsingCCFNVM>(
                                                          "exec dbo.[GetAllEmployeePositionsUsingCompanyCodeFileNumber] @CompanyCode,@FileNumber",
                                                          new SqlParameter("@CompanyCode", strCompanyCode),
                                                          new SqlParameter("@FileNumber", strFileNumber)
                                                         ).ToList();

                                        if (dtEPositions.Count == 0)
                                        {
                                            sbErrors.AppendLine(Environment.NewLine + "No Employee position for " + strCompanyCode + ", " + strFileNumber + ".  Record skipped. Record number = " + nLoopCount + ". ");
                                            continue;
                                        }
                                        else if (dtEPositions.Count > 1)
                                        {
                                            sbErrors.AppendLine(Environment.NewLine + "More than one Employee position for " + strCompanyCode + ", " + strFileNumber + ".  Record skipped. Record number = " + nLoopCount + ". ");
                                            continue;
                                        }
                                        else
                                        {
                                            int nEPositionId = Convert.ToInt32(dtEPositions.FirstOrDefault().ePosId);
                                            int nPositionId = Convert.ToInt32(dtEPositions.FirstOrDefault().PosId);
                                            int nEmployeeId = Convert.ToInt32(dtEPositions.FirstOrDefault().empId);

                                            var dtcheckEmployeeActuals = clientDbContext.Database.SqlQuery<EmployeeExistingActualsVM>(
                                                                          "exec dbo.[GetAllEmployee_ExistingActaulsForPinehurst] @FileNumber,@CompanyCode,@EmployeeID,@PositionID,@PayPeriodDate",
                                                                          new SqlParameter("@FileNumber", strFileNumber),
                                                                          new SqlParameter("@CompanyCode", strCompanyCode),
                                                                          new SqlParameter("@EmployeeID", nEmployeeId),
                                                                          new SqlParameter("@PositionID", nPositionId),
                                                                          new SqlParameter("@PayPeriodDate", payPeriodEndDate)
                                                                         ).ToList();

                                            if (dtcheckEmployeeActuals.Count == 0)
                                            {
                                                var employeeActuals = new EmployeeActuals();
                                                employeeActuals.FileNumber = strFileNumber;
                                                employeeActuals.CompanyCode = strCompanyCode;
                                                employeeActuals.EmployeeID = nEmployeeId;
                                                employeeActuals.PositionID = nPositionId;
                                                employeeActuals.PayPeriodDate = PayEndDate;
                                                employeeActuals.ActualPay = (Decimal)dGrossWages;
                                                employeeActuals.PayCheckEndDate = null;
                                                clientDbContext.EmployeeActuals.Add(employeeActuals);
                                                clientDbContext.SaveChanges();
                                            }
                                            else if (dtcheckEmployeeActuals.Count == 1)
                                            {
                                                var employeeActuals = (from x in clientDbContext.EmployeeActuals where x.FileNumber == strFileNumber && x.EmployeeID == nEmployeeId && x.PositionID == nPositionId && x.CompanyCode == strCompanyCode && x.PayPeriodDate == (DateTime)payPeriodEndDate select x).FirstOrDefault();
                                                employeeActuals.ActualPay = (Decimal)dGrossWages;
                                                clientDbContext.SaveChanges();
                                            }
                                            else if (dtEPositions.Count > 1)
                                            {
                                                sbErrors.AppendLine(Environment.NewLine + "More than one Employee position for " + strCompanyCode + ", " + strFileNumber + ".  Record skipped. Record number = " + nLoopCount + ". ");
                                                continue;
                                            }
                                        }
                                    }

                                    catch (Exception err1)
                                    {
                                        sbErrors.AppendLine(Environment.NewLine + "Error trying to insert record for " + strCompanyCode + ", " + strFileNumber + ". Record skipped. Record number = " + nLoopCount + ". ");
                                        sbErrors.AppendLine(err1.Message + System.Environment.NewLine + err1.StackTrace);
                                    }
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
            EmailProcessorCommunity.Send("", strFrom, strTo, " Employee Actuals Import Results", "This is an automated message from the Employee Actuals Import Task. Results of import are as follows: "
                + Environment.NewLine + Environment.NewLine
                + strResults
                + Environment.NewLine + Environment.NewLine
                + "Number Of Attempts = " + numberOfAttempts.ToString(), false);
            string strToSecond = ConfigurationManager.AppSettings["toAddressTwo"].ToString();
            EmailProcessorCommunity.Send("", strFrom, strToSecond, " Employee Actuals Import Results", "This is an automated message from the Employee Actuals Import Task. Results of import are as follows: "
                + Environment.NewLine + Environment.NewLine
                + strResults
                + Environment.NewLine + Environment.NewLine
                + "Number Of Attempts = " + numberOfAttempts.ToString(), false);
        }

        private DateTime? ConvertToDateTime(string strDateIn)
        {
            DateTime tempDate = DateTime.MaxValue;
            if (DateTime.TryParse(strDateIn, out tempDate))
                return tempDate;
            else
                return null;
        }

        private Decimal? ConvertToDecimal(string strDecimalIn)
        {

            bool bIsNegative = false;
            strDecimalIn = strDecimalIn.Trim();
            if (strDecimalIn.Contains("(")) bIsNegative = true;
            strDecimalIn = strDecimalIn.Replace("(", "");
            strDecimalIn = strDecimalIn.Replace(")", "");

            Decimal dTemp = -1111.456m;
            if (Decimal.TryParse(strDecimalIn, out dTemp))
                return bIsNegative ? dTemp * (-1) : dTemp;
            else
                return null;
        }

        #endregion
    }
}