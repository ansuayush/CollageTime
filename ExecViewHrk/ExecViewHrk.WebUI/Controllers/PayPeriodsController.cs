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
using ExecViewHrk.WebUI.Helpers;
using System.IO;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using Newtonsoft.Json;
using ExecViewHrk.SqlData;
using ExecViewHrk.Domain.Repositories;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using ExecViewHrk.Domain.Helper;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PayPeriodsController : Controller
    {
        IPayPeriodRepository _payperiodrepository;
        // GET: PayPeriods

        public PayPeriodsController(IPayPeriodRepository payperiodrepository)
        {
            _payperiodrepository = payperiodrepository;
        }

        public ActionResult PayPeriodsMatrixPartial()
        {
            var model = new PayPeriodVM();
            //PopulateCompanyCodes();
            //PopulatePayFrequencies();
            return View(model);
        }

        public ActionResult PayPeriodsList_Read([DataSourceRequest]DataSourceRequest request,int CompanyCodeId)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var payperiodlist = _payperiodrepository.GetPayPeriodList();
                return Json(payperiodlist.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PayPeriodDetails(int payPeriodId)
        {
            return View(GetPayPeriodRecord(payPeriodId));
        }

        public PayPeriodVM GetPayPeriodRecord(int payPeriodId)
        {
            PayPeriodVM payPeriodVm = new PayPeriodVM();
            payPeriodVm = _payperiodrepository.GetPayPeriodDetail(payPeriodId);
            return payPeriodVm;
        }

        
        public ActionResult Index()
        {
            PayPeriodVM model = PayPeriodListimport();            
            var payperiodlist = _payperiodrepository.GetPayPeriodList();
            return View("PayPeriodsMatrixPartial", model);
        }

        public ActionResult IndexMain()
        {
            PayPeriodVM model = PayPeriodListimport();          
            var payperiodlist = _payperiodrepository.GetPayPeriodList();
            return View("Index", model);
        }

        public PayPeriodVM PayPeriodListimport()
        {
            PayPeriodVM PayPeriod = new PayPeriodVM();
            return PayPeriod;
        }



        /// <summary>
        /// Saving the PayPeriod Table
        /// </summary>
        /// <param name="payperiodVm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(PayPeriodVM payperiodVm)
        {
            //Duplicate Validation...
            int ppExists = 0;
            var ppitem = new PayPeriodVM();
            var payperioddetails = new PayPeriodVM();
                if (payperiodVm != null)
              {
                if (payperiodVm.PayPeriodId == 0)
                {
                    ppExists = _payperiodrepository.GetPayPeriodList().Where(x => x.PayFrequencyId == payperiodVm.PayFrequencyId && x.CompanyCodeId.Equals(payperiodVm.CompanyCodeId)
                                                                          && x.StartDate.Equals(payperiodVm.StartDate)).Count();
                }
                else
                {
                    ppExists = _payperiodrepository.GetPayPeriodList().Where(x => x.PayPeriodId != payperiodVm.PayPeriodId && x.PayFrequencyId == payperiodVm.PayFrequencyId && x.CompanyCodeId.Equals(payperiodVm.CompanyCodeId)
                                                                               && x.StartDate.Equals(payperiodVm.StartDate)).Count();
                }

                var ppdates = _payperiodrepository.GetPayPeriodList();
                if (payperiodVm.PayPeriodId > 0)
                {
                    ppdates = ppdates.Where(x => x.PayFrequencyId == payperiodVm.PayFrequencyId && x.CompanyCodeId.Equals(payperiodVm.CompanyCodeId) && x.PayPeriodId != payperiodVm.PayPeriodId).ToList();
                }
                else
                {
                    ppdates = ppdates.Where(x => x.PayFrequencyId == payperiodVm.PayFrequencyId && x.CompanyCodeId == payperiodVm.CompanyCodeId).ToList();
                }
                if (payperiodVm.PayPeriodId == 0)
                {
                    ppitem = _payperiodrepository.GetPayPeriodList().Where(x => x.PayFrequencyId == payperiodVm.PayFrequencyId && x.CompanyCodeId.Equals(payperiodVm.CompanyCodeId)
                                                                           && x.StartDate.Equals(payperiodVm.StartDate)).SingleOrDefault();
                }
                else
                {
                    ppitem = _payperiodrepository.GetPayPeriodList().Where(x => x.PayPeriodId != payperiodVm.PayPeriodId && x.PayFrequencyId == payperiodVm.PayFrequencyId && x.CompanyCodeId.Equals(payperiodVm.CompanyCodeId)
                                                                           && x.StartDate.Equals(payperiodVm.StartDate)).SingleOrDefault();
                }
                if (payperiodVm.PayPeriodId > 0)
                {
                    if (ppitem != null)
                    {

                        //NEED TO SKIP THE LOADED RECORD IF NO MODIFICATION IS DONE
                        if (ppitem.PayFrequencyId == payperiodVm.PayFrequencyId && ppitem.StartDate.Equals(payperiodVm.StartDate) && ppitem.PayGroupCode == payperiodVm.PayGroupCode)
                        {
                            if (ppitem.PayPeriodId != payperiodVm.PayPeriodId)
                                return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "Start Date with Pay Frequency") }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (ppExists > 0)
                        {
                            if (ppitem.PayFrequencyId == payperiodVm.PayFrequencyId && ppitem.StartDate.Equals(payperiodVm.StartDate) && ppitem.PayGroupCode==payperiodVm.PayGroupCode)
                            {
                                return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "Start Date with Pay Frequency and Pay Group Code") }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                else
                {
                    if (ppExists > 0)
                    {
                        return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "Start Date with Pay Frequency and Pay Group Code") }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (ppdates != null && ppdates.Count > 0)
                {
                    foreach (var items in ppdates)
                    {

                        if ((items.CompanyCodeId == payperiodVm.CompanyCodeId) && (payperiodVm.StartDate >= items.StartDate && payperiodVm.StartDate <= items.EndDate) && (items.PayGroupCode==payperiodVm.PayGroupCode))
                        {
                            return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "Start Date is Overlap with existing dates") }, JsonRequestBehavior.AllowGet);
                        }
                        if ((items.CompanyCodeId == payperiodVm.CompanyCodeId) && (payperiodVm.EndDate >= items.StartDate && payperiodVm.EndDate <= items.EndDate) && (items.PayGroupCode == payperiodVm.PayGroupCode))
                        {
                            return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "End Date is Overlap with existing dates") }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                string list = payperiodVm.managerslockedlist;
                List<ManagerLockoutsVM> model = new List<ManagerLockoutsVM>();
                if (list != null)
                {
                    string[] arraylist = list.Split(';');
                    if (arraylist.Length != 0)
                    {
                        foreach (var item in arraylist)
                        {
                            var mnaagerslockout = new ManagerLockoutsVM
                            {
                                ManagerUserName = item
                            };
                            model.Add(mnaagerslockout);
                        }
                    }
                }
                payperiodVm.ManagersList = model;
                if (ModelState.IsValid)
                {
                    payperioddetails = _payperiodrepository.savePayPeriod(payperiodVm,User.Identity.Name);
                }
            }
            return Json(new { payperioddetails, succeed = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //public ActionResult PayPeriodDelete(int payPeriodId)
        //{
        //    try
        //    {
        //        _payperiodrepository.DeletePayPeriod(payPeriodId);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Message = ex.Message, succeed = false }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult PayPeriodDelete(int payPeriodId, string userId)
        {
            try
            {
                //Check Existing Records with PayPeriodId
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                if (clientDbContext.ManagerLockouts.Any(x => x.PayPeriodId == payPeriodId))
                {
                    return Json(new { Message = CustomErrorMessages.ERROR_DELETING_RECORD_ALREADY_IN_USE, succeed = false }, JsonRequestBehavior.AllowGet);
                }
                if (clientDbContext.TimeCardApprovals.Any(x => x.PayPeriodId == payPeriodId))
                {
                    return Json(new { Message = CustomErrorMessages.ERROR_DELETING_RECORD_ALREADY_IN_USE, succeed = false }, JsonRequestBehavior.AllowGet);
                }


                _payperiodrepository.DeletePayPeriod(payPeriodId, User.Identity.Name);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, succeed = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PayPeriodsList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.PayPeriod payPeriod)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (payPeriod != null && ModelState.IsValid && (payPeriod.EndDate.Date > payPeriod.StartDate.Date) && payPeriod.CompanyCodeId != null)
                {

                    var newPayPeriod = new PayPeriod
                    {
                        CompanyCodeId = payPeriod.CompanyCodeId,
                        StartDate = payPeriod.StartDate.Date,
                        EndDate = payPeriod.EndDate.Date,
                        PayFrequencyId = payPeriod.PayFrequencyId,
                        LockoutEmployees = payPeriod.LockoutEmployees,
                        LockoutManagers = payPeriod.LockoutManagers,
                        IsArchived = payPeriod.IsArchived,
                        IsPayPeriodActive = true
                    };

                    clientDbContext.PayPeriods.Add(newPayPeriod);
                    clientDbContext.SaveChanges();
                    payPeriod.PayPeriodId = newPayPeriod.PayPeriodId;

                    var AutoFillTimecardInDb = clientDbContext.TimeCardDisplayColumns
                        .Where(x => (x.TimeCardTypeId == 1 && x.AutoFill == true) || (x.TimeCardTypeId == 2 && x.AutoFill == true)).Any();

                    if (AutoFillTimecardInDb)
                    {
                        //IEnumerable <int> employeeIdCollections = clientDbContext.Employees.Where(x => x.CompanyCodeId == payPeriod.CompanyCodeId && x.EmployeeId == 1).Select(x=>x.EmployeeId);

                        //IEnumerable<int> employeeIdCollections = clientDbContext.Employees.GroupBy(x => x.EmploymentNumber)
                        //    .Select(m => m.OrderByDescending(x => x.EmployeeId).FirstOrDefault())
                        //    .Where(x => x.CompanyCodeId == payPeriod.CompanyCodeId)
                        //    .Select(x => x.EmployeeId);

                        //Employee has only one timecardtype, this determine that timecardtype is autofill or not
                        IEnumerable<EmployeeTimeCardAutoFill> employeeIdCollections = clientDbContext.Employees
                        .Include("DdlTimeCardTypes.TimeCardDisplayColumns")
                        .Include("DdlEmploymentStatuses")
                        .GroupBy(x => x.PersonId)
                        .Select(m => m.OrderByDescending(x => x.EmploymentNumber).FirstOrDefault())
                        .Where(x => x.CompanyCodeId == payPeriod.CompanyCodeId && x.DdlTimeCardType.TimeCardDisplayColumn.AutoFill != null
                        && x.DdlEmploymentStatus.Code == "A")
                        .Select(x => new EmployeeTimeCardAutoFill
                        {
                            EmployeeId = x.EmployeeId,
                            AutoFill = x.DdlTimeCardType.TimeCardDisplayColumn.AutoFill
                        });

                        if (employeeIdCollections.Any())
                        {
                            int days = (payPeriod.EndDate.Date - payPeriod.StartDate.Date).Days;

                            foreach (EmployeeTimeCardAutoFill empId in employeeIdCollections)
                            {
                                if (empId.AutoFill)
                                {
                                    for (int i = 0; i <= days; i++)
                                    {
                                        //var checkDay = payPeriod.StartDate.Date.AddDays(i);
                                        //if (empId.AutoFill)
                                        //{
                                        var newTimeCardRecord = new TimeCard
                                        {
                                            CompanyCodeId = payPeriod.CompanyCodeId.Value,
                                            EmployeeId = empId.EmployeeId,
                                            ActualDate = payPeriod.StartDate.Date.AddDays(i),
                                            ProjectNumber = 1,
                                        };

                                        clientDbContext.TimeCards.Add(newTimeCardRecord);
                                        //}
                                    }
                                }
                            }
                            clientDbContext.SaveChanges();
                        }

                    }

                }

                return Json(new[] { payPeriod }.ToDataSourceResult(request, ModelState));
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PayPeriodsList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.PayPeriod payPeriod)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (payPeriod != null && ModelState.IsValid)
                {
                    var payPeriodInDb = clientDbContext.PayPeriods
                        .Where(x => x.PayPeriodId == payPeriod.PayPeriodId)
                        .SingleOrDefault();

                    if (payPeriodInDb != null)
                    {
                        payPeriodInDb.PayPeriodId = payPeriod.PayPeriodId;
                        payPeriodInDb.CompanyCodeId = payPeriod.CompanyCodeId;
                        payPeriodInDb.StartDate = payPeriod.StartDate.Date;
                        payPeriodInDb.EndDate = payPeriod.EndDate.Date;
                        payPeriodInDb.PayFrequencyId = payPeriod.PayFrequencyId;
                        payPeriodInDb.LockoutEmployees = payPeriod.LockoutEmployees;
                        payPeriodInDb.LockoutManagers = payPeriod.LockoutManagers;
                        payPeriodInDb.IsArchived = payPeriod.IsArchived;
                        payPeriodInDb.IsPayPeriodActive = payPeriod.IsPayPeriodActive;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { payPeriod }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PayPeriodsList_Destroy([DataSourceRequest] DataSourceRequest request
            , PayPeriod payPeriod)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (payPeriod != null)
                {
                    PayPeriod payPeriodInDb = clientDbContext.PayPeriods
                        .Where(x => x.PayPeriodId == payPeriod.PayPeriodId).SingleOrDefault();

                    if (payPeriodInDb != null)
                    {
                        clientDbContext.PayPeriods.Remove(payPeriodInDb);

                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch// (Exception err)
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                        }

                    }
                }

                return Json(new[] { payPeriod }.ToDataSourceResult(request, ModelState));
            }
        }

        //public JsonResult GetPayPeriods(string text)
        //{
        //    string connString = User.Identity.GetClientConnectionString();

        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {

        //        var payPeriods = clientDbContext.PayPeriods
        //            .Where(x => x.IsPayPeriodActive == true)
        //            .Select(m => new
        //            {
        //                PayPeriodId = m.PayPeriodId,
        //                PayPeriodDescription = m.PayPeriodDescription
        //            }).OrderBy(x => x.PayPeriodDescription).ToList();

        //        return Json(payPeriods, JsonRequestBehavior.AllowGet);
        //    }

        //}

        private void PopulateCompanyCodes()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var companyCodesList = new ClientDbContext(connString).CompanyCodes
                        .Select(c => new CompanyCodeVm
                        {
                            CompanyCodeId = c.CompanyCodeId,
                            CompanyCodeDescription = c.CompanyCodeDescription //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(s => s.CompanyCodeDescription).ToList();

                //companyCodesList.Insert(0, new CompanyCodeVm { CompanyCodeId = 0, CompanyCodeDescription = "--select one--" });

                ViewData["companyCodesList"] = companyCodesList;
                //ViewData["defaultCompanyCode"] = companyCodesList.First();
            }
        }


        private void PopulatePayFrequencies()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var payFrequenciesList = new ClientDbContext(connString).DdlPayFrequencies
                        .Select(c => new
                        {
                            PayFrequencyId = c.PayFrequencyId,
                            PayFrequencyDescription = "Hours " + c.Description //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(s => s.PayFrequencyDescription).ToList();

                //payFrequenciesList.Insert(0, new { 0,  "--select one--" });

                ViewData["payFrequenciesList"] = payFrequenciesList;
                //ViewData["defaultPayFrequency"] = payFrequenciesList.First();
            }
        }


        [HttpPost]
        public JsonResult ExportEmployeeTimeCardDetails_Ajax(int companyCodeId, int payPeriodId)
        {
            List<TimeCardExportCollectionVM> exportTimeCardList = new List<TimeCardExportCollectionVM>();
            string connString = User.Identity.GetClientConnectionString();
            System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/TimeCardExport"));
            //System.IO.Directory.CreateDirectory(System.Configuration.ConfigurationManager.AppSettings["appRoot"] + "TimeCardExport");
            string filename = "TimeCard" + DateTime.Now.ToString("_MM-dd-yyyy_HH-mm-ss-fff") + ".csv";
            string FilePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TimeCardExport"), filename);
            try
            {
                exportTimeCardList = _payperiodrepository.Export_Employee_TimecardDetails(companyCodeId, payPeriodId);
                if (exportTimeCardList.Count() > 0)
                {
                    List<String> listExportTC = new List<string>();
                    listExportTC.Add("CompanyCode,BatchId,FileNumber,Reg Hours,OverTimeHours,Hours3Code,Hours3Amount,"
                            + "Hours4Code,Hours4Amount,Hours5Code,Hours5Amount,Earnings3Code,Earnings3Amount,"
                            + "Earnings4Code, Earnings4Amount, Earnings5Code, Earnings5Amount");
                    foreach (var exportTC in exportTimeCardList)
                    {
                        listExportTC.Add(exportTC.ToString());   //override ToString() in TimeCardExportCollection
                    }
                    System.IO.File.WriteAllLines(FilePath, listExportTC);
                }
                else
                {
                    filename = string.Empty;
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                filename = string.Empty;
            }
            return Json(new { fileName = filename, exportTimeCardList.Count }, JsonRequestBehavior.AllowGet);
        }
        public string PayRateExport(string startDate, string endDate, int? payGroupId, int? payPeriodNumber, int CompanyCode)
        {
            PayRateExport2(startDate, endDate, payGroupId, payPeriodNumber, CompanyCode);
            return "ok";
        }
        public void PayRateExport2(string startDate, string endDate, int? payGroupId, int? payPeriodNumber,int CompanyCode)
        {
           
            string mailFrom = ConfigurationManager.AppSettings["fromAddress"].ToString();
            string toAddress = ConfigurationManager.AppSettings["toAddress"].ToString();
            string toAddressTwo = ConfigurationManager.AppSettings["toAddressTwo"].ToString();                           
            var connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString,0);

            ConvertStudenttoTemparyemployees(startDate, endDate, payPeriodNumber, false, false, CompanyCode);
           
             var empTimeCardList = clientDbContext.Database.SqlQuery<ExportPayPeriodVM>(
                                                          "exec dbo.spGetExportPayperiod @PayPeriodNumber,@PayGroupId,@StartDate,@EndDate,@CompanyCode",
                                                          new SqlParameter("@PayPeriodNumber", payPeriodNumber),
                                                          new SqlParameter("@PayGroupId", payGroupId),
                                                          new SqlParameter("@StartDate", startDate),
                                                          new SqlParameter("@EndDate", endDate),
                                                          new SqlParameter("@CompanyCode", CompanyCode)
                                                        ).ToList();
            
            if (empTimeCardList != null && empTimeCardList.Count >= 0)
            {
                StringBuilder sbAllResults = new StringBuilder();
                string fileName = string.Empty;
                var sw = new StringWriter();
                sw.WriteLine("\"Co Code\",\"Batch ID\",\"File #\",\"Cancel Pay\",\"Pay #\",\"Reg Hours\",\"O/T Hours\",\"Hours 3 Code\",\"Hours 3 Amount\",\"Hours 4 Code\",\"Hours 4 Amount\",\"Reg Earnings\",\"O/T Earnings\",\"Earnings 3 Code\",\"Earnings 3 Amount\",\"Earnings 4 Code\",\"Earnings 4 Amount\",\"Earnings 5 Code\",\"Earnings 5 Amount\",\"Temp Cost Number\",\"Temp Department\",\"Temp Rate\",\"Memo Code\",\"Memo Amount\",\"Memo Code\",\"Memo Amount\"");
                Response.ClearContent();
                if (payPeriodNumber >= 1 && payPeriodNumber <= 19)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=EPIELLCT 1-19.csv");
                    fileName = "EPIELLCT 1-19.csv";
                }
                else if(payPeriodNumber >= 20 && payPeriodNumber <= 26)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=EPIELLCT 20-26.csv");
                    fileName = "EPIELLCT 20-26.csv";
                }
                Response.ContentType = "text/csv";
                foreach (var item in empTimeCardList)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\",\"{21}\",\"{22}\",\"{23}\",\"{24}\",\"{25}\"",
                                      item.CoCode,
                                      item.BatchID,
                                      item.FileNumber,
                                      item.CancelPay,
                                      item.PayNumber,
                                      item.RegHours,
                                      item.OTHours,
                                      item.Hours3Code,
                                      item.Hours3Amount,
                                      item.Hours4Code,
                                      item.Hours4Amount,
                                      item.RegEarnings,
                                      item.OTEarnings,
                                      item.Earnings3Code,
                                      item.Earnings3Amount,
                                      item.Earnings4Code,
                                      item.Earnings4Amount,
                                      item.Earnings5Code,
                                      item.Earnings5Amount,
                                      item.CostNumber,
                                      item.Department,
                                      item.TempRate,
                                      item.MemoCode1,
                                      item.MemoAmount1,
                                      item.MemoCode2,
                                      item.MemoAmount2
                                      ));
                }
                Response.Write(sw.ToString());                
                Response.End();
                if (sw.ToString() !="")
                {                   
                    System.IO.MemoryStream str = new MemoryStream();
                    System.Text.Encoding Enc = System.Text.Encoding.Default;
                    byte[] mBArray = Enc.GetBytes(sw.ToString());
                    str = new MemoryStream(mBArray, false);
                 
                    var companyCodesList = clientDbContext.CompanyCodes.Where(x => x.CompanyCodeId == CompanyCode).Select(x => x.CompanyCodeDescription).FirstOrDefault();
                    var companyCodeDesc = string.IsNullOrEmpty(companyCodesList) ? "" : companyCodesList;
           
                    var sdate = DateTime.ParseExact(startDate, "yyyyMMdd", null).ToShortDateString();
                    var edate = DateTime.ParseExact(endDate, "yyyyMMdd", null).ToShortDateString();
                    var emailsubject = "EPIP: " + companyCodeDesc + " (" + sdate + "  To  " +  edate + ")";

                    sbAllResults.AppendLine(Environment.NewLine + "Company Code: " + companyCodeDesc);
                    sbAllResults.AppendLine(Environment.NewLine + "Pay Period Start Date: " + sdate);
                    sbAllResults.AppendLine(Environment.NewLine + "Pay Period End Date: " + edate);
                    sbAllResults.AppendLine(Environment.NewLine + "Record Count: " + empTimeCardList.Count);
                    string strResults = sbAllResults.ToString();
                
                    EmailProcessorCommunity.SendEmailSingleFileAttachment(mailFrom, toAddress, toAddressTwo, emailsubject, "This is an automated message from the Drew University Web Time Pay Period Export."
                     + Environment.NewLine + Environment.NewLine
                     + strResults
                     + Environment.NewLine + Environment.NewLine, false, str, fileName);
                    //Drew-Student Treaty Limit Exceeded

                    string emailBody = _payperiodrepository.AutoSendEmailTreatyLimit(CompanyCode, payGroupId);

                    if (emailBody != null)
                    {
                        EmailProcessorCommunity.SendEmailTreatyLimitExceeded(mailFrom, toAddress,toAddressTwo, "Drew-Student Treaty Limit Exceeded", emailBody, true);
                    }
                }
                
                InsertPayRateExportLog(startDate, endDate, payGroupId, payPeriodNumber, CompanyCode);
                
                //Convert Student to employees ....

                ConvertStudenttoTemparyemployees(startDate, endDate, payPeriodNumber, true, true, CompanyCode);


            }
        }
        public void PayRateExportEposition(string startDate, string endDate, int? payGroupId, int? payPeriodNumber, int CompanyCode)
        {
            var connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString);
            var empTimeCardList = clientDbContext.Database.SqlQuery<ExportPayPeriodVM>(
                                                          "exec dbo.spGetExportPayperiodbyEPosition @PayPeriodNumber,@PayGroupId,@StartDate,@EndDate,@CompanyCode",
                                                          new SqlParameter("@PayPeriodNumber", payPeriodNumber),
                                                          new SqlParameter("@PayGroupId", payGroupId),
                                                          new SqlParameter("@StartDate", startDate),
                                                          new SqlParameter("@EndDate", endDate),
                                                          new SqlParameter("@CompanyCode", CompanyCode)
                                                        ).ToList();
            if (empTimeCardList != null && empTimeCardList.Count >= 0)
            {
                var sw = new StringWriter();
                sw.WriteLine("\"Co Code\",\"Batch ID\",\"File #\",\"Cancel Pay\",\"Pay #\",\"Reg Hours\",\"O/T Hours\",\"Hours 3 Code\",\"Hours 3 Amount\",\"Hours 4 Code\",\"Hours 4 Amount\",\"Reg Earnings\",\"O/T Earnings\",\"Earnings 3 Code\",\"Earnings 3 Amount\",\"Earnings 4 Code\",\"Earnings 4 Amount\",\"Earnings 5 Code\",\"Earnings 5 Amount\",\"Temp Cost Number\",\"Temp Department\",\"Temp Rate\",\"Memo Code\",\"Memo Amount\",\"Memo Code\",\"Memo Amount\"");
                Response.ClearContent();
                if (payPeriodNumber >= 1 && payPeriodNumber <= 19)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=EPIELLCT 1-19.csv");
                }
                else if (payPeriodNumber >= 20 && payPeriodNumber <= 26)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=EPIELLCT 20-26.csv");
                }
                Response.ContentType = "text/csv";
                foreach (var item in empTimeCardList)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\",\"{21}\",\"{22}\",\"{23}\",\"{24}\",\"{25}\"",
                                      item.CoCode,
                                      item.BatchID,
                                      item.FileNumber,
                                      item.CancelPay,
                                      item.PayNumber,
                                      item.RegHours,
                                      item.OTHours,
                                      item.Hours3Code,
                                      item.Hours3Amount,
                                      item.Hours4Code,
                                      item.Hours4Amount,
                                      item.RegEarnings,
                                      item.OTEarnings,
                                      item.Earnings3Code,
                                      item.Earnings3Amount,
                                      item.Earnings4Code,
                                      item.Earnings4Amount,
                                      item.Earnings5Code,
                                      item.Earnings5Amount,
                                      item.CostNumber,
                                      item.Department,
                                      item.TempRate,
                                      item.MemoCode1,
                                      item.MemoAmount1,
                                      item.MemoCode2,
                                      item.MemoAmount2
                                      ));
                }
                Response.Write(sw.ToString());
                Response.End();

                //Inserts the Exported information in the corresponding Log Table.
                //TempData["exportedLog"] = empTimeCardList;
                //TempData.Keep();
                InsertPayRateExportLog(startDate, endDate, payGroupId, payPeriodNumber, CompanyCode);
            }
        }

        public void ConvertStudenttoTemparyemployees(string startDate, string endDate, int? payPeriodNumber,  bool? Isexported, bool? IsStudent, int CompanyCode)
        {
            var connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString, 0);

            var temporaryemployeeList = clientDbContext.Database.SqlQuery<ExportPayPeriodVM>(
                                                          "exec dbo.GetConvertedStudenttotemporaryemployeeinbtweenpayperiod @PayPeriodNumber,@StartDate,@EndDate,@Isexported,@IsStudent,@CompanyCode",
                                                          new SqlParameter("@PayPeriodNumber", payPeriodNumber),
                                                          new SqlParameter("@StartDate", startDate),
                                                          new SqlParameter("@EndDate", endDate),
                                                          new SqlParameter("@Isexported", Isexported),
                                                          new SqlParameter("@IsStudent", IsStudent),
                                                          new SqlParameter("@CompanyCode", CompanyCode)
                                                        ).ToList();
        }

        public ActionResult LockOutEmployeeUpdate(int payPeriodId, bool lockoutemployee)
        {
            try
            {
                _payperiodrepository.LockoutemployeeUpdate(payPeriodId, lockoutemployee);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, succeed = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LockOutManagersUpdate(int payPeriodId, bool lockoutManagers)
        {
            try
            {
                _payperiodrepository.LockoutManagersUpdate(payPeriodId, lockoutManagers);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, succeed = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportPreviewPayPeriod(int companyCodeId, int payPeriodId)
        {
            List<TimeCardExportCollectionVM> exportTimeCardList = new List<TimeCardExportCollectionVM>();
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                try
                {
                    //var exportTimeCardList = new ExecViewHrk.SqlData.SqlTimeCards(connString)
                    //         .Export_Employee_TimecardDetails(companyCodeId, payPeriodId).ToList();
                    exportTimeCardList = _payperiodrepository.Export_Employee_TimecardDetails(companyCodeId, payPeriodId);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            return View("PreviewPayPeriod", exportTimeCardList);
        }
        public ActionResult ExportPreviewPayPeriod1(string startDate, string endDate, int? payGroupId, int? payPeriodNumber)
        {
            List<ExportPayPeriodVM> exportTimeCardList = new List<ExportPayPeriodVM>();
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                try
                {
                    exportTimeCardList = clientDbContext.Database.SqlQuery<ExportPayPeriodVM>(
                                                          "exec dbo.spGetExportPayperiod @PayPeriodNumber,@PayGroupId,@StartDate,@EndDate",
                                                          new SqlParameter("@PayPeriodNumber", payPeriodNumber),
                                                          new SqlParameter("@PayGroupId", payGroupId),
                                                          new SqlParameter("@StartDate", startDate),
                                                          new SqlParameter("@EndDate", endDate)
                                                        ).ToList();

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            return View("PreviewPayPeriod1", exportTimeCardList);
        }

        [HttpGet]
        public FileContentResult Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception(String.Format("File not exist"));

            byte[] filedata = null;
            string contentType = string.Empty;
            try
            {
                string fullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TimeCardExport"), fileName);
                if (System.IO.File.Exists(fullPath))
                {
                    filedata = System.IO.File.ReadAllBytes(fullPath);
                    contentType = MimeMapping.GetMimeMapping(fullPath);
                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = fileName,
                        Inline = true,
                    };
                    Response.AppendHeader("Content-Disposition", cd.ToString());
                }
                else
                {
                    throw new Exception(String.Format("File not exist", fileName));
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return File(filedata, contentType);
        }


        [HttpPost]
        public JsonResult BeforeArchive(short companyCodeId, int payPeriodId)
        {
            var payPerioddata = _payperiodrepository.GetPayPeriodDetail(payPeriodId);
            payPerioddata.TimecardUnApprovedCount = _payperiodrepository.GetUnApprovedTimecards(companyCodeId, payPerioddata.StartDate, payPerioddata.EndDate);
            payPerioddata.TimecardCount = _payperiodrepository.GetTimecardCount(companyCodeId, payPerioddata.StartDate, payPerioddata.EndDate);
            payPerioddata.IsEndDate = payPerioddata.EndDate > DateTime.Now;
            return Json(payPerioddata, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ArchiveEmployeesTimeCard_Ajax(short companyCodeId, int payPeriodId)
        {
            var payperioddetails = _payperiodrepository.GetPayPeriodDetail(payPeriodId);

            int payfrequencyid = payperioddetails.PayFrequencyId;
            DateTime startdate = payperioddetails.StartDate;
            DateTime enddate = payperioddetails.EndDate;
            bool result = _payperiodrepository.ArchivePayperiod(companyCodeId, payfrequencyid, startdate, enddate, payPeriodId);
            //if (result == "false")
            //{
            //    ModelState.AddModelError("", "The payperiod not exist for company");
            //}
            //bool timeCardStatus = false;
            //string connString = User.Identity.GetClientConnectionString();
            //using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            //{ 
            //    if (companyCodeId != null && payPeriodId > 0 && ModelState.IsValid)
            //    {
            //        var payPeriodInDb = clientDbContext.PayPeriods
            //               .Where(x => x.PayPeriodId == payPeriodId && x.CompanyCodeId == companyCodeId)
            //               .SingleOrDefault();

            //        if (payPeriodInDb != null)
            //        {
            //                payPeriodInDb.IsArchived = true;                           
            //                clientDbContext.SaveChanges();                       
            //        }
            //        else
            //        {
            //            ModelState.AddModelError("", "The payperiod not exist for company");
            //        }
            //    }
            //}

            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }

        #region PayPeriod Exported Log
        /// <summary>
        /// Inserts the Exported Information in a Log Table. Main Fields FileNumber,PayNum,EmployeeType,PayGroupId
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="payGroupId"></param>
        /// <param name="payPeriodNumber"></param>
        public void InsertPayRateExportLog(string startDate, string endDate, int? payGroupId, int? payPeriodNumber,int? CompanyCode)
        {
            var connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString, 0);
            List<PayPeriodsExportedLogVM> _payPeriodsExportedLog = new List<PayPeriodsExportedLogVM>();            
            //var exportedLog = TempData["exportedLog"];
            var empTimeCardList = clientDbContext.Database.SqlQuery<ExportPayPeriodVM>(
                                                         "exec dbo.spGetExportPayperiod @PayPeriodNumber,@PayGroupId,@StartDate,@EndDate,@CompanyCode",
                                                         new SqlParameter("@PayPeriodNumber", payPeriodNumber),
                                                         new SqlParameter("@PayGroupId", payGroupId),
                                                         new SqlParameter("@StartDate", startDate),
                                                         new SqlParameter("@EndDate", endDate),
                                                         new SqlParameter("@CompanyCode", CompanyCode)                                                         
                                                       ).ToList();
            try
            {
                foreach (var item in empTimeCardList)
                {
                    var exportedPayPeriodLogItem = new PayPeriodsExportedLogVM
                    {
                        ExportedLogId = _payPeriodsExportedLog.Count + 1,
                        CoCode = item.CoCode,
                        BatchID = item.BatchID,
                        FileNumber = item.FileNumber,
                        PayNumber = payPeriodNumber.ToString(),
                        RegHours = item.RegHours,
                        OTHours = item.OTHours,
                        Hours3Code = item.Hours3Code,
                        Hours3Amount = item.Hours3Amount,
                        Hours4Code = item.Hours4Code,
                        Hours4Amount = item.Hours4Amount,
                        RegEarnings = item.RegEarnings,
                        OTEarnings = item.OTEarnings,
                        Earnings3Code = item.Earnings3Code,
                        Earnings3Amount = item.Earnings3Amount,
                        Earnings4Code = item.Earnings4Code,
                        Earnings4Amount = item.Earnings4Amount,
                        Earnings5Code = item.Earnings5Code,
                        Earnings5Amount = item.Earnings5Amount,
                        CostNumber = item.CostNumber,
                        Department = item.Department,
                        TempRate = item.TempRate,
                        MemoCode1 = item.MemoCode1,
                        MemoAmount1 = item.MemoAmount1,
                        MemoCode2 = item.MemoCode2,
                        CancelPay = item.CancelPay,
                        MemoAmount2 = item.MemoAmount2,
                        PayGroupId = payGroupId.ToString(),
                        StartDate = startDate,
                        EndDate = endDate
                    };
                    /*
                    if (payPeriodNumber >= 1 && payPeriodNumber <= 18)
                    {
                        exportedPayPeriodLogItem.EmployeeType = "9 x 12";
                        if (string.IsNullOrEmpty(exportedPayPeriodLogItem.MemoAmount1))
                            exportedPayPeriodLogItem.MemoAmount1 = "0";
                        if (string.IsNullOrEmpty(exportedPayPeriodLogItem.MemoAmount2))
                            exportedPayPeriodLogItem.MemoAmount2 = "0";                       
                        decimal rate = Convert.ToDecimal(exportedPayPeriodLogItem.MemoAmount2) - Convert.ToDecimal(exportedPayPeriodLogItem.MemoAmount1);
                        exportedPayPeriodLogItem.SalaryHistoryRate = rate.ToString();                                                                                      
                    }
                    if (payPeriodNumber >= 19 && payPeriodNumber <= 26)
                    {
                        exportedPayPeriodLogItem.EmployeeType = "9 x 9";                        
                    }
                    */
                    if (string.IsNullOrEmpty(exportedPayPeriodLogItem.MemoAmount1))
                        exportedPayPeriodLogItem.MemoAmount1 = "0";
                    if (string.IsNullOrEmpty(exportedPayPeriodLogItem.MemoAmount2))
                        exportedPayPeriodLogItem.MemoAmount2 = "0";
                    decimal rate = Convert.ToDecimal(exportedPayPeriodLogItem.MemoAmount2) - Convert.ToDecimal(exportedPayPeriodLogItem.MemoAmount1);
                    exportedPayPeriodLogItem.SalaryHistoryRate = rate.ToString();
                    _payPeriodsExportedLog.Add(exportedPayPeriodLogItem);
                }
            }
            catch (Exception ex)
            {

            }
            BulkCopyData(User.Identity.GetClientConnectionString(), "PayPeriodsExportedLogs", _payPeriodsExportedLog.AsDataTable(), payPeriodNumber.ToString());
        }

        /// <summary>
        /// Performs Bulk Insert for the specified Table Object.
        /// </summary>
        /// <param name="_connectionString"></param>
        /// <param name="_destinationTableName"></param>
        /// <param name="_dt"></param>
        /// <param name="payPeriodNumber"></param>
        public void BulkCopyData(string _connectionString, string _destinationTableName, DataTable _dt,string payPeriodNumber)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null))
                {
                    using (SqlCommand cmdTruncate = new SqlCommand("Delete from PayPeriodsExportedLogs where PayNumber = " + payPeriodNumber + "", connection))
                    {
                        connection.Open();
                        cmdTruncate.CommandTimeout = 300;
                        cmdTruncate.ExecuteNonQuery();

                        bulkCopy.DestinationTableName = _destinationTableName;
                        bulkCopy.BulkCopyTimeout = 300;
                        bulkCopy.BatchSize = 10000;
                        try
                        {
                            bulkCopy.WriteToServer(_dt);
                            connection.Close();
                        }
                        catch(Exception ex)
                        {

                        }
                    }
                }
            }
        }
        #endregion

        #region Global Archive for all Companies        
        public JsonResult BeforeGlobalArchive(int payPeriodId)
        {
            var payPerioddata = _payperiodrepository.GetPayPeriodDetail(payPeriodId);
            payPerioddata.TimecardUnApprovedCount = _payperiodrepository.GetGlobalUnApprovedTimecards(payPerioddata.StartDate, payPerioddata.EndDate);
            payPerioddata.TimecardCount = _payperiodrepository.GetGlobalTimecardCount(payPerioddata.StartDate, payPerioddata.EndDate);
            payPerioddata.IsEndDate = payPerioddata.EndDate > DateTime.Now;
            return Json(payPerioddata, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ArchiveEmployeesGlobalTimeCard_Ajax(int payPeriodId)
        {
            var payperioddetails = _payperiodrepository.GetPayPeriodDetail(payPeriodId);
            int payfrequencyid = payperioddetails.PayFrequencyId;
            DateTime startdate = payperioddetails.StartDate;
            DateTime enddate = payperioddetails.EndDate;
            bool result = _payperiodrepository.ArchiveGlobalPayperiod(payfrequencyid, startdate, enddate, payPeriodId);

            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }




        #region payperiodtimport
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
                        //string connString = User.Identity.GetClientConnectionString();
                        string strConnection = ConfigurationManager.ConnectionStrings["execView1"].ConnectionString;
                        //var sqlConnection = new SqlConnection(strConnection);
                        ClientDbContext clientDbContext = new ClientDbContext(strConnection);
                        foreach (var row in sheet)
                        {
                            nLoopCount++;
                            if (nLoopCount == 1)
                            {
                                if (row[0].ToLower() != "paytype")
                                {
                                    sbErrors.AppendLine("Column A is named: " + row[0] + " and should be named: Pay Type.");
                                    bErrorFound = true;
                                }
                                if (row[1].ToLower() != "startdate")
                                {
                                    sbErrors.AppendLine("Column B is named: " + row[1] + " and should be named: Start Date.");
                                    bErrorFound = true;
                                }
                                if (row[2].ToLower() != "enddate")
                                {
                                    sbErrors.AppendLine("Column C is named: " + row[2] + " and should be named: End Date.");
                                    bErrorFound = true;
                                }
                                if (row[3].ToLower() != "paygroupcode")
                                {
                                    sbErrors.AppendLine("Column D is named: " + row[3] + " and should be named: Pay Group Code.");
                                    bErrorFound = true;
                                }
                                if (row[4].ToLower() != "payperiodnumber")
                                {
                                    sbErrors.AppendLine("Column E is named: " + row[4] + " and should be named: Pay Period Number.");
                                    bErrorFound = true;
                                }                               
                                if (bErrorFound) break;
                            }
                            else
                            {
                                string strPayType = row[0] == null ? "" : row[0].ToString();
                                if (strPayType.Length == 0)
                                {
                                    sbErrors.AppendLine(Environment.NewLine + "Pay Type is blank.  Record skipped. Record number = " + nLoopCount);
                                    continue;
                                }
                                string strStartDate = row[1] == null ? "" : row[1].ToString();
                                if (strStartDate.Length == 0)
                                {
                                    sbErrors.AppendLine(Environment.NewLine + "Start Date is blank.  Record skipped. Record number = " + nLoopCount);
                                    continue;
                                }
                                string strEndDate = row[2] == null ? "" : row[2].ToString();
                                if (strEndDate.Length == 0)
                                {
                                    sbErrors.AppendLine(Environment.NewLine + "End Date is blank.  Record skipped. Record number = " + nLoopCount);
                                    continue;
                                }
                                string strPayGroupCode = row[3] == null ? "" : row[3].ToString();
                                if (strPayGroupCode.Length == 0)
                                {
                                    sbErrors.AppendLine(Environment.NewLine + "Pay Group Code is not in the correct format. Record skipped. Record number =" + nLoopCount);
                                    continue;
                                }
                                string strPayPeriodnumber = row[4] == null ? "" : row[4].ToString();
                                if (strPayPeriodnumber.Length == 0)
                                {
                                    sbErrors.AppendLine(Environment.NewLine + "Pay Period Number is blank. Record skipped. Record number =" + nLoopCount);
                                    continue;
                                }

                                int ? paygroupid = Convert.ToInt32(strPayGroupCode);
                                int? PayPeriodnumber = Convert.ToInt32(strPayPeriodnumber);
                                int? PayFrequencyId = 2;
                                DateTime StartDate = Convert.ToDateTime(strStartDate);
                                DateTime EndDate = Convert.ToDateTime(strEndDate);
                                var PayPeriodscount = clientDbContext.PayPeriods.Where(x => x.PayFrequencyId == PayFrequencyId && x.StartDate == StartDate && x.EndDate == EndDate && x.PayGroupCode == paygroupid && x.PayPeriodNumber == PayPeriodnumber).ToList();
                                if (PayPeriodscount.Count == 0)
                                {
                                    var ddPayPeriod = new PayPeriod();
                                    ddPayPeriod.StartDate = StartDate;
                                    ddPayPeriod.EndDate = EndDate;
                                    ddPayPeriod.PayFrequencyId = PayFrequencyId.Value;
                                    ddPayPeriod.CompanyCodeId = 1;
                                    ddPayPeriod.IsPayPeriodActive = true;
                                    ddPayPeriod.IsArchived = false;
                                    ddPayPeriod.LockoutEmployees = false;
                                    ddPayPeriod.LockoutManagers = false;
                                    ddPayPeriod.PayGroupCode = paygroupid;
                                    ddPayPeriod.PayPeriodNumber = PayPeriodnumber;
                                    ddPayPeriod.PayDate = null;
                                    ddPayPeriod.IsDeleted = false;
                                    ddPayPeriod.UserId = "dan@hrknowledge.com";
                                    ddPayPeriod.LastModifiedDate = null;
                                    clientDbContext.PayPeriods.Add(ddPayPeriod);
                                    clientDbContext.SaveChanges();
                                }
                                else
                                {
                                    PayPeriod objPayPeriod = clientDbContext.PayPeriods.Where(x => x.PayFrequencyId == PayFrequencyId && x.StartDate == StartDate && x.EndDate == EndDate && x.PayGroupCode == paygroupid && x.PayPeriodNumber == PayPeriodnumber).FirstOrDefault();//clientDbContext.PayPeriods.Where(x => x.PayPeriodId == PayPeriodId && x.StartDate == Convert.ToDateTime(strStartDate)).FirstOrDefault();
                                    objPayPeriod.PayFrequencyId = Convert.ToInt32(PayFrequencyId);
                                    objPayPeriod.StartDate = Convert.ToDateTime(strStartDate);
                                    objPayPeriod.EndDate = Convert.ToDateTime(strEndDate);
                                    objPayPeriod.PayGroupCode = paygroupid;
                                    objPayPeriod.PayPeriodNumber = PayPeriodnumber;
                                    objPayPeriod.IsDeleted = false;
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

            return RedirectToAction("IndexMain");
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
            EmailProcessorCommunity.Send("", strFrom, strTo, " Pay Period Import Results", "This is an automated message from the Pay Period  Import Task. Results of import are as follows: "
                + Environment.NewLine + Environment.NewLine
                + strResults
                + Environment.NewLine + Environment.NewLine
                + "Number Of Attempts = " + numberOfAttempts.ToString(), false);
            string strToSecond = ConfigurationManager.AppSettings["toAddressTwo"].ToString();
            EmailProcessorCommunity.Send("", strFrom, strToSecond, " Pay Period Import Results", "This is an automated message from the Pay Period  Import Task. Results of import are as follows: "
                + Environment.NewLine + Environment.NewLine
                + strResults
                + Environment.NewLine + Environment.NewLine
                + "Number Of Attempts = " + numberOfAttempts.ToString(), false);
        }


        #endregion

        #endregion        

    }
}