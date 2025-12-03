using ExecViewHrk.Domain.Helper;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Domain.Repositories;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Infrastructure;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity.SqlServer;
using System.Configuration;
using Newtonsoft.Json;

namespace ExecViewHrk.WebUI.Controllers
{

    public class RetroHoursController : BaseController
    {
        readonly IEmployeeRetroHours _EmployeeRetroHours;
        readonly private IEPositionRepository _positionRepo;


        public RetroHoursController(
            IEPositionRepository positionRepo
           )
        {
            _positionRepo = positionRepo;

        }
        // GET: RetroHours
        RetrohoursVm retrohours = new RetrohoursVm();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RetroHoursMatrixPartial()
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            RetrohoursVm retrohoursvm = new RetrohoursVm();
            int id = Convert.ToInt32(TempData["EditId"]);
            if (id != 0)
            {
                var retrohoursDB = clientDbContext.EmployeeRetroHours.Where(x => x.Id == id).FirstOrDefault();
                int CompanyCodeId = retrohoursDB.CompanyCodeId;
                string CompanyCodeIds = Convert.ToString(CompanyCodeId);
                var CCcode = clientDbContext.CompanyCodes.Where(r => r.CompanyCodeId == retrohoursDB.CompanyCodeId).Select(s => s.CompanyCodeCode).FirstOrDefault();
                var posId = clientDbContext.E_Positions.Where(r => r.E_PositionId == retrohoursDB.EPositionId).Select(s => s.PositionId).FirstOrDefault();
                var Posdescrption = clientDbContext.Positions.Where(r => r.PositionId == posId).Select(s => s.PositionDescription).FirstOrDefault();
                var PersonId = clientDbContext.Employees.Where(r => r.EmployeeId == retrohoursDB.EmployeeId).Select(s => s.PersonId).FirstOrDefault();
                var employeeName = clientDbContext.Persons.Where(r => r.PersonId == PersonId).Select(s => s.Lastname + ", " + s.Firstname).FirstOrDefault();
                var payperiods = clientDbContext.PayPeriods.Where(r => r.PayPeriodId == retrohoursDB.PayperiodId).Select(s => s.StartDate + "-" + s.EndDate).FirstOrDefault();
                var RetroCodeDEC = clientDbContext.HoursCodes.Where(r => r.HoursCodeId == retrohoursDB.HoursCodeId).Select(s => s.HoursCodeCode).FirstOrDefault();
                //EmployeeRetroHours retroHours = new EmployeeRetroHours();
                retrohoursvm.HourCodeList = GetHoursCodeList(CompanyCodeId);// JsonConvert.DeserializeObject<List<DropDownModel>>();
                if (retrohoursDB != null)
                {
                    retrohoursvm.Id = retrohoursDB.Id;
                    retrohoursvm.Retrohours = retrohoursDB.RetroHours;
                    retrohoursvm.RetroHoursDate = retrohoursDB.RetroHoursDate;
                    retrohoursvm.EmployeeFullName = employeeName;
                    retrohoursvm.CompanyCodeDescription = CCcode;
                    retrohoursvm.CompanyCodeId = retrohoursDB.CompanyCodeId;
                    retrohoursvm.PositionDescription = Posdescrption;
                    retrohoursvm.EmployeeId = retrohoursDB.EmployeeId;
                    retrohoursvm.PositionId = posId;
                    retrohoursvm.HoursCodeCode = RetroCodeDEC;
                    retrohoursvm.PayPeriod = payperiods;
                }
                return View(retrohoursvm);
            }

            else
            {
                return View(retrohours);
            }

        }
        public ActionResult RetroHours_Read([DataSourceRequest]DataSourceRequest request, int EmployeeId, int CompanyCodeId, int PositionId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            List<RetrohoursVm> RetroHourList = new List<RetrohoursVm>();
            if (User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
            {
                RetroHourList = (from eo in clientDbContext.EmployeeRetroHours
                                 join Epos in clientDbContext.E_Positions on
                                 eo.EPositionId equals Epos.E_PositionId
                                 join Pos in clientDbContext.Positions on
                                 Epos.PositionId equals Pos.PositionId
                                 join emp in clientDbContext.Employees on
                                      eo.EmployeeId equals emp.EmployeeId
                                 join per in clientDbContext.Persons
                                      on emp.PersonId equals per.PersonId
                                 join cmp in clientDbContext.CompanyCodes
                                      on eo.CompanyCodeId equals cmp.CompanyCodeId
                                 join pay in clientDbContext.PayPeriods
                                      on eo.PayperiodId equals pay.PayPeriodId
                                 join HC in clientDbContext.HoursCodes
                                       on eo.HoursCodeId equals HC.HoursCodeId
                                 select new RetrohoursVm
                                 {
                                     Id = eo.Id,
                                     PositionId = Pos.PositionId,
                                     CompanyCodeId = cmp.CompanyCodeId,
                                     EmployeeId = emp.EmployeeId,
                                     CompanyCodeDescription = cmp.CompanyCodeDescription,
                                     EmployeeFullName = per.Lastname + ", " + per.Firstname,
                                     PositionCode = Pos.PositionDescription,
                                     Retrohours = eo.RetroHours,
                                     strRatroHourDate = SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", eo.RetroHoursDate)).Trim() + "/" + SqlFunctions.DateName("DAY", eo.RetroHoursDate) + "/" + SqlFunctions.DateName("YYYY", eo.RetroHoursDate),
                                     PayperiodDate = SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", pay.StartDate)).Trim() + "/" + SqlFunctions.DateName("DAY", pay.StartDate) + "/" + SqlFunctions.DateName("YYYY", pay.StartDate)
                                                     + " - " + SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", pay.EndDate)).Trim() + "/" + SqlFunctions.DateName("DAY", pay.EndDate) + "/" + SqlFunctions.DateName("YYYY", pay.EndDate),
                                     HoursCodedesc = HC.HoursCodeCode + "-" + HC.HoursCodeDescription,
                                     Username = eo.EnteredBy
                                 }).ToList();
            }
            else if (User.IsInRole("ClientManagers"))
            {

                string useridentityname = User.Identity.Name;
                var aspNetUsersEmail = clientDbContext.AspNetUsers.Where(x => x.UserName == useridentityname).Select(x => x.Email).FirstOrDefault();
                var ReportsToID = clientDbContext.Persons.Where(x => x.eMail == aspNetUsersEmail).Select(x => x.PersonId).FirstOrDefault();
                if (ReportsToID == 0)
                {
                    ReportsToID = 0;
                }
                RetroHourList = (from eo in clientDbContext.EmployeeRetroHours
                                 join Epos in clientDbContext.E_Positions on
                                 eo.EPositionId equals Epos.E_PositionId
                                 join Pos in clientDbContext.Positions on
                                 Epos.PositionId equals Pos.PositionId
                                 join emp in clientDbContext.Employees on
                                      eo.EmployeeId equals emp.EmployeeId
                                 join per in clientDbContext.Persons
                                      on emp.PersonId equals per.PersonId
                                 join cmp in clientDbContext.CompanyCodes
                                      on eo.CompanyCodeId equals cmp.CompanyCodeId
                                 join pay in clientDbContext.PayPeriods
                                      on eo.PayperiodId equals pay.PayPeriodId
                                 join HC in clientDbContext.HoursCodes
                                    on eo.HoursCodeId equals HC.HoursCodeId
                                 where (Epos.ReportsToID == ReportsToID)
                                 select new RetrohoursVm
                                 {
                                     Id = eo.Id,
                                     PositionId = Pos.PositionId,
                                     CompanyCodeId = cmp.CompanyCodeId,
                                     EmployeeId = emp.EmployeeId,
                                     CompanyCodeDescription = cmp.CompanyCodeDescription,
                                     EmployeeFullName = per.Lastname + ", " + per.Firstname,
                                     PositionCode = Pos.PositionDescription,
                                     Retrohours = eo.RetroHours,
                                     strRatroHourDate = SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", eo.RetroHoursDate)).Trim() + "/" + SqlFunctions.DateName("DAY", eo.RetroHoursDate) + "/" + SqlFunctions.DateName("YYYY", eo.RetroHoursDate),
                                     PayPeriodId = eo.PayperiodId,
                                     PayperiodDate = SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", pay.StartDate)).Trim() + "/" + SqlFunctions.DateName("DAY", pay.StartDate) + "/" + SqlFunctions.DateName("YYYY", pay.StartDate)
                                                     + " - " + SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", pay.EndDate)).Trim() + "/" + SqlFunctions.DateName("DAY", pay.EndDate) + "/" + SqlFunctions.DateName("YYYY", pay.EndDate),
                                     HoursCodedesc = HC.HoursCodeCode + "-" + HC.HoursCodeDescription,
                                     Username = eo.EnteredBy,
                                     RetroHoursDate = eo.RetroHoursDate,
                                 }).ToList();

            }

            return KendoCustomResult(RetroHourList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        public ActionResult RetroHoursSaveAjx(RetrohoursVm retrohoursVm)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string requestType = User.Identity.GetRequestType();
            //DateTime rateHourdate = retrohoursVm.RatroHourDate;
            var RateHourdate = retrohoursVm.RetroHoursDate;
            var PositionDescription = clientDbContext.Positions.Where(r => r.PositionId == retrohoursVm.PositionId).Select(s => s.PositionDescription).FirstOrDefault();
            var CmpanyCode = clientDbContext.CompanyCodes.Where(r => r.CompanyCodeId == retrohoursVm.CompanyCodeId).Select(s => s.CompanyCodeDescription).FirstOrDefault();
            var Personid = clientDbContext.Employees.Where(r => r.EmployeeId == retrohoursVm.EmployeeId).Select(s => s.PersonId).FirstOrDefault();
            var Employee = clientDbContext.Persons.Where(x => x.PersonId == Personid).Select(r => r.Lastname + "," + r.Firstname).FirstOrDefault();

            //var E_PositionId = clientDbContext.E_Positions.Where(r => r.PositionId == retrohoursVm.PositionId).Select(s => s.E_PositionId).FirstOrDefault();

            var E_PositionId = retrohoursVm.PositionId;
            //clientDbContext.E_Positions.Where(r => r.EmployeeId == retrohoursVm.EmployeeId).Select(s => s.E_PositionId).FirstOrDefault();       

            if(retrohoursVm.PositionId == 0)
            {
                 E_PositionId = retrohoursVm.E_PositionId;
            }        
            var EpositionSalid = getPositionSalaryHistoryId(retrohoursVm.RetroHoursDate, E_PositionId); //clientDbContext.E_PositionSalaryHistories.Where(r => r.E_PositionId == E_PositionId && r.EffectiveDate != null).OrderByDescending(x => x.E_PositionSalaryHistoryId).Select(s => s.E_PositionSalaryHistoryId).FirstOrDefault();
            var Newrate = clientDbContext.E_PositionSalaryHistories.Where(r => r.E_PositionSalaryHistoryId == EpositionSalid).Select(s => s.PayRate).FirstOrDefault();
            EmployeeRetroHours EmployeeRetroHours = new EmployeeRetroHours();
            var edittbl = clientDbContext.EmployeeRetroHours.Where(x => x.Id == retrohoursVm.Id).FirstOrDefault();
            if (EpositionSalid != 0 && E_PositionId != 0)
            {
            if (retrohoursVm.Id == 0)
            {
                EmployeeRetroHours.EmployeeId = retrohoursVm.EmployeeId;
                EmployeeRetroHours.CompanyCodeId = retrohoursVm.CompanyCodeId;
                EmployeeRetroHours.EPositionId = E_PositionId;
                EmployeeRetroHours.RetroHours = Convert.ToDecimal(retrohoursVm.Retrohours);
                EmployeeRetroHours.RetroHoursDate = retrohoursVm.RetroHoursDate;
                EmployeeRetroHours.PositionSalaryHistoryId = EpositionSalid;
                EmployeeRetroHours.ModifiedBy = User.Identity.Name;
                EmployeeRetroHours.ModifiedDate = DateTime.UtcNow;
                EmployeeRetroHours.EnteredBy = User.Identity.Name;
                EmployeeRetroHours.EnteredDate = DateTime.UtcNow;
                EmployeeRetroHours.HoursCodeId = retrohoursVm.HoursCodeId;
                EmployeeRetroHours.PayperiodId = retrohoursVm.PayPeriodId;
                clientDbContext.EmployeeRetroHours.Add(EmployeeRetroHours);
            }
            else
            {
                edittbl.EmployeeId = retrohoursVm.EmployeeId;
                edittbl.CompanyCodeId = retrohoursVm.CompanyCodeId;
                edittbl.EPositionId = E_PositionId;
                edittbl.RetroHours = Convert.ToDecimal(retrohoursVm.Retrohours);
                edittbl.RetroHoursDate = retrohoursVm.RetroHoursDate;
                edittbl.PositionSalaryHistoryId = EpositionSalid;
                edittbl.ModifiedBy = User.Identity.Name;
                edittbl.ModifiedDate = DateTime.UtcNow;
                edittbl.EnteredBy = User.Identity.Name;
                edittbl.EnteredDate = DateTime.UtcNow;
                edittbl.HoursCodeId = retrohoursVm.HoursCodeId;
                edittbl.PayperiodId = retrohoursVm.PayPeriodId;
            }

        }
        else
        {
           ModelState.AddModelError("", "Invalid Position Id");
         }

         try
         {
             clientDbContext.SaveChanges();

          }
            catch (Exception ex)
            {
                return RedirectToAction("RetroHoursMatrixPartial");
            }

            return RedirectToAction("RetroHoursMatrixPartial");
        }
        public ActionResult DeleteRetrohour(int RetroHourId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.EmployeeRetroHours.Where(x => x.Id == RetroHourId).SingleOrDefault();
            if (dbRecord != null)
            {
                var obj = new EmployeeRetroHoursRepository();
                obj.RetroDelete(RetroHourId);

                try
                {
                    clientDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { Message = ex.Message, succeed = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Message = "Record does not exist.", succeed = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeePositionList(int companyCodeId, int EmployeeId, DateTime RetrohourDate)
        {
            string connString = User.Identity.GetClientConnectionString();
            List<E_PositioVm> PositionList = new List<E_PositioVm>();
            if (!String.IsNullOrEmpty(connString))
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    if (User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
                    {
                        PositionList = GetActiveE_Positions(_positionRepo.GetEPositionListbyRetrohourDate(EmployeeId, companyCodeId), RetrohourDate);

                        retrohours.PositionList = PositionList;
                        ViewData["PositionList"] = PositionList;
                    }
                    if (User.IsInRole("ClientManagers"))
                    {
                        string useridentityname = User.Identity.Name;
                        PositionList = GetActiveE_Positions(_positionRepo.GetEPositionListbyManagerId(EmployeeId, companyCodeId, useridentityname), RetrohourDate);
                        retrohours.PositionList = PositionList;
                        ViewData["PositionList"] = PositionList;
                    }

                };
            return Json(PositionList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Common function used to get employees active positions for the current pay period and pay group - Filter method
        /// </summary>
        /// <param name="posList">List of all employee positions</param>
        /// <returns>Active position list</returns>
        private List<E_PositioVm> GetActiveE_Positions(List<E_PositioVm> posList, DateTime RetrohourDate)
        {
            var activePositions = new List<E_PositioVm>();
            foreach (E_PositioVm pos in posList)
            {
                bool flag = false;
                if (pos.StartDate.HasValue && pos.actualEndDate.HasValue)
                {
                    if ((DateTime.Compare(RetrohourDate.Date, pos.StartDate.Value) >= 0) && (DateTime.Compare(RetrohourDate.Date, pos.actualEndDate.Value) <= 0))
                    {
                        // Position Dates are in between Retrohour Date
                        flag = true;

                    }


                }
                else if (pos.StartDate.HasValue && !pos.actualEndDate.HasValue)
                {
                    // If Position has Start Date but NO End Date                 
                    if (DateTime.Compare(RetrohourDate.Date, pos.StartDate.Value) >= 0)
                    {
                        flag = true;
                    }
                }
                if (flag)
                    activePositions.Add(pos);
            }
            return activePositions;
        }

        public ActionResult RetroHourEdit(int Id)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            RetrohoursVm retrohours = new RetrohoursVm();
            List<E_PositioVm> PositionList = new List<E_PositioVm>();
            var tbl = clientDbContext.EmployeeRetroHours.Where(x => x.Id == Id).FirstOrDefault();
            var HoursCode = clientDbContext.HoursCodes.Where(r => r.HoursCodeId == tbl.HoursCodeId).Select(s => s.HoursCodeCode).FirstOrDefault();
            int CompanyCodeId = tbl.CompanyCodeId;
            var CompanyCode = clientDbContext.CompanyCodes.Where(r => r.CompanyCodeId == tbl.CompanyCodeId).Select(s => s.CompanyCodeDescription).FirstOrDefault();
            var empid = tbl.EmployeeId;
            var Personid = clientDbContext.Employees.Where(r => r.EmployeeId == tbl.EmployeeId).Select(s => s.PersonId).FirstOrDefault();
            var Employee = clientDbContext.Persons.Where(x => x.PersonId == Personid).Select(r => r.Lastname + "," + r.Firstname).FirstOrDefault();

            ViewData["HoursCodeList"] = GetHoursCodeList(CompanyCodeId);
            if (User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
            {
                PositionList = GetActiveE_Positions(_positionRepo.GetEPositionListbyRetrohourDate(tbl.EmployeeId, tbl.CompanyCodeId), tbl.RetroHoursDate.Value);

                retrohours.PositionList = PositionList;
            }
            if (User.IsInRole("ClientManagers"))
            {
                string useridentityname = User.Identity.Name;
                PositionList = GetActiveE_Positions(_positionRepo.GetEPositionListbyManagerId(tbl.EmployeeId, tbl.CompanyCodeId, useridentityname), tbl.RetroHoursDate.Value);
                retrohours.PositionList = PositionList;
            }
            ViewData["PositionList"] = PositionList;
            ViewData["companyCodesList"] = GetCompanyCodes();
            ViewData["EmployeeList"] = GetPersonsList();
            
            ViewData["PayPeriodsList"] = GetHourPayPeriods(empid); 

            retrohours.Id = tbl.Id;
            retrohours.EmployeeId = tbl.EmployeeId;
            retrohours.PersonId = Personid;
            retrohours.E_PositionId = tbl.EPositionId;
            retrohours.CompanyCodeId = tbl.CompanyCodeId;
            retrohours.HoursCodeId = tbl.HoursCodeId;
            retrohours.Retrohours = tbl.RetroHours;
            retrohours.RetroHoursDate = tbl.RetroHoursDate;
            retrohours.HoursCodeCode = HoursCode;
            retrohours.CompanyCodeDescription = CompanyCode;
            retrohours.PayPeriodId = tbl.PayperiodId;
            return View(retrohours);
        }

        public int getPositionSalaryHistoryId(DateTime? RetrohourDate, int EpositionId)
        {

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var activeSalaryRate = new List<E_PositionSalaryHistorVm>();
            var result = 0;
            var listPositionSalaryHis = _positionRepo.GetSalaryHistroybyEpositionId(EpositionId);
            if (listPositionSalaryHis != null)
            {
                foreach (E_PositionSalaryHistorVm esh in listPositionSalaryHis)
                {
                    bool flag = false;
                    if (esh.EffectiveDate.HasValue && esh.EndDate.HasValue)
                    {
                        if ((DateTime.Compare(RetrohourDate.Value, esh.EffectiveDate.Value) >= 0) && (DateTime.Compare(RetrohourDate.Value, esh.EndDate.Value) <= 0))
                        {
                            // Rate Dates are in between Retrohour Date
                            flag = true;

                        }
                    }
                    else if (esh.EffectiveDate.HasValue && !esh.EndDate.HasValue)
                    {
                        // If Rate has Start Date but NO End Date                 
                        if (DateTime.Compare(RetrohourDate.Value, esh.EffectiveDate.Value) >= 0)
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                        activeSalaryRate.Add(esh);
                }

                result = activeSalaryRate.Where(e => e.E_PositionId == EpositionId).Select(sr => sr.E_PositionSalaryHistoryId).FirstOrDefault();
            }
            return result;
        }

        public List<DropDownModel> GetHoursCodeList(int CompanyCodeId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var HoursCodeLst = clientDbContext.HoursCodes.Where(r => r.IsRetro == true && r.StartDate == null && r.CompanyCodeId == CompanyCodeId)
                .Select(T => new DropDownModel
                {
                    HoursCodeId = T.HoursCodeId.ToString(),
                    HoursCodeCode = T.HoursCodeCode
                })
                 .OrderBy(x => x.HoursCodeCode)
                .ToList();
            return HoursCodeLst;
        }

        public List<CompanyCodeVM> GetCompanyCodes()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            List<CompanyCodeVM> companyCodeslist = clientDbContext.CompanyCodes
                   .Where(x => x.IsCompanyCodeActive == true)
                   .Select(m => new CompanyCodeVM
                   {
                       CompanyCodeId = m.CompanyCodeId,
                       CompanyCodeDescription = m.CompanyCodeDescription
                   }).OrderBy(x => x.CompanyCodeDescription).ToList();
            return companyCodeslist;
        }

        public List<DropDownModel> GetPersonsList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var personsList = clientDbContext.Persons
                .Select(m => new DropDownModel { keyvalue = m.PersonId.ToString(), keydescription = m.Lastname + " " + m.Firstname })
                .OrderBy(x => x.keydescription)
                .ToList();
            return personsList;
        }
        public List<PayPeriodVM> GetHourPayPeriods(int? Employeeid)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var payGroupId = clientDbContext.E_Positions.Where(x => x.EmployeeId == Employeeid).Select(X => X.PayGroupId).First();

            List<PayPeriodVM> payPeriodsList = new List<PayPeriodVM>();
            int? payfrequencyid = clientDbContext.Employees.Where(x => x.EmployeeId == Employeeid).Select(x => x.PayFrequencyId).FirstOrDefault();
            if (payfrequencyid != null)
            {
                payPeriodsList = clientDbContext.PayPeriods
                 .Where(m => m.PayFrequencyId == payfrequencyid && m.StartDate <= System.DateTime.UtcNow
                                                                && m.IsPayPeriodActive == true //SHOULD IMPORT 1 TO THIS COLUMN DURING MAIN EMPLOYEE IMPORT
                                                                && m.PayGroupCode == payGroupId
                                                                && m.IsDeleted == false
                                                                )
                  .Select(m => new PayPeriodVM
                  {
                      PayPeriodId = m.PayPeriodId,
                      EndDate = m.EndDate,
                      PayGroupCode = m.PayGroupCode,
                      PayPeriod = SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.StartDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.StartDate) + "/" + SqlFunctions.DateName("YYYY", m.StartDate)
                                 + " - " + SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.EndDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.EndDate) + "/" + SqlFunctions.DateName("YYYY", m.EndDate)
                  }).OrderByDescending(m => m.EndDate).ToList();//.Take(6).ToList();
            }
            return payPeriodsList;
        }

    }
}

       