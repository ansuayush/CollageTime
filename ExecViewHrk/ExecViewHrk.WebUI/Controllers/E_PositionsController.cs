using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.EfClient;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Helpers;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using Newtonsoft.Json;
using ExecViewHrk.WebUI.Services;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.WebUI.Models;
using System.Configuration;

namespace ExecViewHrk.WebUI.Controllers
{
    public class E_PositionsController : BaseController
    {
        IEPositionRepository _epositionRepository;
        public E_PositionsController(IEPositionRepository epositionRepository)
        {
            _epositionRepository = epositionRepository;
        }
        readonly PositionService positionService = new PositionService();
        // GET: E_Positions
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ePositionList_Read([DataSourceRequest]DataSourceRequest request,int? empno)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string connString = User.Identity.GetClientConnectionString();
            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees")) throw new Exception("Client Employee trying to access NSS.");

            if (requestType != "IsSelfService") SessionStateHelper.CheckForEmployeeSelectedValue();

            int personId = User.Identity.GetRequestType() == "IsSelfService" ? clientDbContext.Employees.Where(x => x.Person.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID));
            var EmpId = empno == null ? clientDbContext.Employees.Where(x => x.Person.PersonId == personId).Select(x => x.EmployeeId).FirstOrDefault() :
                                    clientDbContext.Employees.Where(x => x.Person.PersonId == personId && x.EmploymentNumber == empno).Select(x => x.EmployeeId).FirstOrDefault();
            
            /*var EmpId = Convert.ToInt32(TempData["EmpId"]);
            TempData.Keep("EmpId");
            if (EmpId == 0)
            {
                EmpId = empno == null ? clientDbContext.Employees.Where(x => x.Person.PersonId == personId).Select(x => x.EmployeeId).FirstOrDefault() :
                                    clientDbContext.Employees.Where(x => x.Person.PersonId == personId && x.EmploymentNumber == empno).Select(x => x.EmployeeId).FirstOrDefault();
            }*/

            var FileNumber = clientDbContext.Employees.Where(x => x.PersonId == personId).Select(x => x.FileNumber).FirstOrDefault();
            Session["FileNumber"] = FileNumber;
            var ePositionList = _epositionRepository.GetEPositionList(personId, EmpId);   
            // return Json(ePositionList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            return KendoCustomResult(ePositionList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult EPositionDelete(int e_PositionId)
        {
            try
            {
                _epositionRepository.DeleteEPosition(e_PositionId);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, succeed = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult E_PositionsDetail(int ePositionId, int EmployeeId, int personId, int EmpNumber)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var employeeId = clientDbContext.Employees.Where(m => (m.EmploymentNumber == EmployeeId && m.PersonId == personId)).Select(m => m.EmployeeId).FirstOrDefault();
            TempData["EmployeeId"] = employeeId;
            Session["EmplNo"] = EmployeeId;
            Session["IsnewRecords"] = true;
            Session["EmpNumber"] = EmpNumber;
            return View(GetE_PositionsRecord(ePositionId, personId));
        }

        public PartialViewResult E_PositionsMatrixPartial(int? employeeId, bool isSelectedIndex = false, int e_PositionIdParam = 0)
        {
            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees"))
                throw new Exception("Client Employee trying to access NSS.");


            if (requestType != "IsSelfService")
                SessionStateHelper.CheckForEmployeeSelectedValue();

            string connString = User.Identity.GetClientConnectionString();

            ClientDbContext clientDbContext = new ClientDbContext(connString);

            int personId = User.Identity.GetRequestType() == "IsSelfService" ? clientDbContext.Employees.Where(x => x.Person.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                 : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID));

            if ((personId == 0) && (requestType == "IsSelfService"))
                throw new Exception("Self service person id is 0 - Personal record cannot be displayed. It doesn't exist.");

            E_PositioVm e_PositionVm;
            var employeeList = clientDbContext.Employees.ToList().Where(m => m.PersonId == personId);
            e_PositionVm = new E_PositioVm();
            if (employeeId == null)
            {
                var empId = (from cc in clientDbContext.CompanyCodes
                                   join e in clientDbContext.Employees on cc.CompanyCodeId equals e.CompanyCodeId
                                   join ep in clientDbContext.E_Positions on e.EmployeeId equals ep.EmployeeId
                                   where e.PersonId == personId && (ep.actualEndDate > DateTime.Now || ep.actualEndDate == null)
                                   select new E_PositioVm
                                   {
                                       EmployeeId=e.EmployeeId
                                   }).FirstOrDefault();
                if (empId == null)
                {
                    var empId1 = clientDbContext.Employees.Where(p => p.PersonId == personId).OrderByDescending(x => x.EmployeeId).Select(eid => eid.EmployeeId).FirstOrDefault();
                    var emplno1 = clientDbContext.Employees.Where(p => p.EmployeeId == empId1).OrderByDescending(x => x.EmployeeId).Select(p => p.EmploymentNumber).FirstOrDefault();
                    Session["EmpId"] = empId1;
                    Session["Emplno"] = emplno1;
                }
                else
                {
                    Session["EmpId"] = empId;
                    var emplno = clientDbContext.Employees.Where(p => p.EmployeeId == empId.EmployeeId).Select(p => p.EmploymentNumber).FirstOrDefault();
                    Session["Emplno"] = emplno;
                }
                var employmentNumberList = clientDbContext.Employees.Where(p => p.PersonId == personId).Select(eid => eid.EmployeeId).ToList();
                if(employmentNumberList.Count == 1)
                {
                    Session["EmpId"] = employmentNumberList[0];
                }
                else
                {
                    foreach (var empNo in employmentNumberList)
                    {
                        var enddatelist = clientDbContext.E_Positions.Where(x => x.EmployeeId == empNo).Select(ed => ed.actualEndDate).ToList();
                        foreach (var enddate in enddatelist)
                        {
                            if (enddate < DateTime.Now)
                            {
                                break;
                            }
                            else
                            {
                                //TempData["EmpId"] = empNo;
                                Session["EmpId"] = empNo;
                            }
                        }
                    }
                }
                //var empId = clientDbContext.Employees.Where(p => p.PersonId == personId).OrderByDescending(x => x.EmployeeId).Select(eid => eid.EmployeeId).FirstOrDefault();
                //var emplno = clientDbContext.Employees.Where(p => p.EmployeeId == empId).OrderByDescending(x => x.EmployeeId).Select(p => p.EmploymentNumber).FirstOrDefault();
                // TempData["EmpId"] = empId;
                //TempData.Keep();
                //TempData["EmplNo"] = emplno;
            }
            else
            {
                //TempData["EmpId"] = employeeId;
                Session["EmpId"] = employeeId;
            }
            var EmpIs = Session["Emplno"];
            TempData["EmplNo"] = EmpIs;
            TempData.Keep();
            e_PositionVm.EmployeeIdDropDownList = (from obj in employeeList
                                                   select new SelectListItem
                                                   {
                                                       Selected = (obj.EmploymentNumber.ToString() == EmpIs.ToString()),
                                                       Value = obj.EmploymentNumber.ToString(),
                                                       Text = obj.EmploymentNumber.ToString()
                                                   }).OrderByDescending(x=>x.Selected).ToList();
            ViewData["EmployeeId"] = e_PositionVm.EmployeeIdDropDownList;
            if (isSelectedIndex && e_PositionIdParam > 0)
            {
                e_PositionVm = GetE_PositionsRecord(e_PositionIdParam, personId);
            }
            else
            {
                int e_PositionId = clientDbContext.E_Positions
                    .Include("Employee.Person")
                    .Where(x => x.Employee.PersonId == personId).Select(x => x.E_PositionId).FirstOrDefault();

                if (e_PositionId == 0)
                {
                    e_PositionVm = new E_PositioVm();

                    e_PositionVm.PersonId = personId;

                    e_PositionVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();


                }
                else
                    e_PositionVm = GetE_PositionsRecord(e_PositionId, personId);
            }
            
            return PartialView(e_PositionVm);
        }

        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedE_PositionsAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonAda() action; drop down on top left of employment screen
            E_PositioVm e_PositionVm;
            int e_PositionId = clientDbContext.E_Positions
                   .Include("Employees.Person")
                   .Where(x => x.Employee.Person.PersonId == personId)
                   .OrderByDescending(x => x.Employee.EmploymentNumber)
                   .Select(m => m.E_PositionId).FirstOrDefault();
            if (e_PositionId == 0)
            {
                e_PositionVm = new E_PositioVm();
                e_PositionVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                e_PositionVm.PersonId = personId;
            }
            else
                e_PositionVm = GetE_PositionsRecord(e_PositionId, personId);

            SessionStateHelper.Set(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();
            ModelState.Clear();
            return Json(e_PositionVm, JsonRequestBehavior.AllowGet);
        }

        public E_PositioVm GetE_PositionsRecord(int e_PositionId, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            E_PositioVm e_PositionVm = new E_PositioVm();
            int PositionId = clientDbContext.E_Positions.Where(x => x.E_PositionId == e_PositionId).Select(m => m.PositionId).FirstOrDefault();
            var positiondesc = clientDbContext.Positions.Where(m => m.PositionId == PositionId).Select(m => m.PositionDescription).FirstOrDefault();
            var PositionCode = clientDbContext.Positions.Where(m => m.PositionId == PositionId && m.PositionDescription == positiondesc).Select(m => m.PositionCode).FirstOrDefault();
            var businesslevelNbr = clientDbContext.Positions.Where(m => m.PositionId == PositionId).Select(m => m.BusinessLevelNbr).FirstOrDefault();
            var businesslevel = clientDbContext.PositionBusinessLevels.Where(n => n.BusinessLevelNbr == businesslevelNbr).Select(s => s.BusinessLevelTitle).FirstOrDefault();
            var departmentId = clientDbContext.E_Positions.Where(x => x.E_PositionId == e_PositionId).Select(x => x.DepartmentId).FirstOrDefault();
            var departMent = clientDbContext.Departments.Where(x => x.DepartmentId == departmentId).Select(x => x.DepartmentDescription).FirstOrDefault();
            var jobid = clientDbContext.Positions.Where(n => n.PositionId == PositionId).Select(m => m.JobId).FirstOrDefault();
            var job = clientDbContext.Jobs.Where(n => n.JobId == jobid).Select(s => s.JobDescription).FirstOrDefault();
            var primaryposition = clientDbContext.E_Positions.Where(m => m.PositionId == PositionId).Select(s => s.PrimaryPosition).FirstOrDefault();
            var positiongradeid = clientDbContext.E_Positions.Where(n => n.PositionId == PositionId).Select(m => m.PositionGradeID).FirstOrDefault();
            var payRate = clientDbContext.E_PositionSalaryHistories.Where(m => m.E_PositionId == e_PositionId).Select(m => m.PayRate).FirstOrDefault();
            var hoursPerPayPeriod = clientDbContext.E_PositionSalaryHistories.Where(m => m.E_PositionId == e_PositionId).Select(m => m.HoursPerPayPeriod).FirstOrDefault();
            var employeeId = clientDbContext.E_Positions.Where(x => x.E_PositionId == e_PositionId).Select(m => m.EmployeeId).FirstOrDefault();
            var rateValue = clientDbContext.Employees.Where(x => x.EmployeeId == employeeId).Select(m => m.Rate).FirstOrDefault();
            var paygroupId = clientDbContext.E_Positions.Where(x => x.E_PositionId == e_PositionId).Select(m => m.PayGroupId).FirstOrDefault();
            var compycodeid = clientDbContext.Employees.Where(x => x.EmployeeId == employeeId).Select(x => x.CompanyCodeId).FirstOrDefault();
            //Kusha: Ticket No:  Returns the Actual Date
            var actenddate = clientDbContext.E_Positions.Where(x => x.E_PositionId == e_PositionId).Select(m => m.actualEndDate).FirstOrDefault();
            if (actenddate != null)
            {
                string acDate = Convert.ToString(actenddate);
                DateTime formatDate = Convert.ToDateTime(acDate);
                var formatDate12 = formatDate.ToShortDateString();
                ViewBag.formatDate = formatDate12;
            }

            Session["compycodeid"] = compycodeid;
            Session["ePositionId"] = e_PositionId;
            Session["paygroupId"] = paygroupId;
            ViewBag.RegPayRateAmt = rateValue;
            if (e_PositionId == 0)
            {
                e_PositionVm.PayFrequencyId = 2;
            }
            if (e_PositionId != 0)
            {
                e_PositionVm = clientDbContext.E_Positions
                   .Include("Employee.Person")
                   .Include("Position.PositionDescription")
                   .Include("DdlPayFrequencies.Description")
                   .Include("DdlRateTypes.Description")
                   .Include("DdlPositionCategory.Description")
                   .Include("DdlPositionGrade.Description")
                   .Include("DdlPositionTypes.Description")
                   .Include("E_PositionSalaryHistory.PayRate")
                   .Include("E_PositionSalaryHistory.HoursPerPayPeriod")
                   .Include("E_PositionSalaryHistory.EffectiveDate")
                   .Include("E_PositionSalaryHistory.HoursPerPayPeriod")
                   .Include("DdlEmployeeTypes.Description")
                   .Include("DdlPaygroups.PayGroupId")
                   .Where(x => x.E_PositionId == e_PositionId)
                   .Select(x => new E_PositioVm
                   {
                       E_PositionId = x.E_PositionId,
                       PositionId = x.PositionId,
                       PositionDescription = x.Position.PositionDescription,
                       PositionCode = PositionCode,
                       EmployeeId = x.Employee.EmployeeId,
                       PersonId = x.Employee.Person.PersonId,
                       PersonName = x.Employee.Person.Lastname + ", " + x.Employee.Person.Firstname,
                       PayFrequencyId = x.PayFrequencyId,
                       PayFrequencyDescription = x.DdlPayFrequency.Description,
                       RateTypeId = x.RateTypeId,
                       EmployeeTypeId = x.EmployeeTypeId,
                       PayGroupId = x.PayGroupId,
                       RateTypeDescription = x.DdlRateType.Description,
                       BusinessLevel = businesslevel,
                       Job = job,
                       FTE = x.FTE,
                       PositionCategoryID = x.PositionCategoryID,
                       PositionCategoryDesc = x.DdlPositionCategory.description,
                       PositionTypeID = x.PositionTypeID,
                       PositionTypeDesc = x.DdlPositionTypes.description,
                       PositionGradeID = x.PositionGradeID,
                       PositionGradeDesc = x.DdlPositionGrade.Description,
                       PrimaryPosition = x.PrimaryPosition,
                       StartDate = x.StartDate,
                       actualEndDate = x.actualEndDate,
                       EndDate = x.EndDate,
                       projectedEndDate = x.ProjectedEndDate,
                       E_PositionSalaryHistoryId = clientDbContext.E_PositionSalaryHistories.Where(m => m.E_PositionId == e_PositionId).Select(m => m.E_PositionSalaryHistoryId).FirstOrDefault(),
                       PayRate = payRate,
                       HoursPerPayPeriod = hoursPerPayPeriod,
                       EffectiveDate = clientDbContext.E_PositionSalaryHistories.Where(m => m.E_PositionId == e_PositionId).Select(m => m.EffectiveDate).FirstOrDefault(),
                       EnteredBy = x.EnteredBy,
                       EnteredDate = x.EnteredDate,
                       ModifiedBy = x.ModifiedBy,
                       ModifiedDate = x.ModifiedDate,
                       Notes = x.Notes,
                       ReportsToID = x.ReportsToID,
                       AdpYear = x.AdpYear,
                       AdpWSLimit = x.AdpWSLimit,
                       CostNumber = x.CostNumber,
                       CostNumberEffectiveDate = x.CostNumberEffectiveDate,
                       CostNumber1Percent = x.CostNumber1Percent,
                       CostNumber2Account = x.CostNumber2Account,
                       CostNumber2Percent = x.CostNumber2Percent,
                       CostNumber3Account = x.CostNumber3Account,
                       CostNumber3Percent = x.CostNumber3Percent,
                       CostNumber4Account = x.CostNumber4Account,
                       CostNumber4Percent = x.CostNumber4Percent,
                       CostNumber5Account = x.CostNumber5Account,
                       CostNumber5Percent = x.CostNumber5Percent,
                       CostNumber6Account = x.CostNumber6Account,
                       CostNumber6Percent = x.CostNumber6Percent,
                       EmployeeClassId = x.EmployeeClassId,
                       Suffix = x.Position.Suffix
                   })
                   .FirstOrDefault();
                Session["EmployeeTypeId"] = e_PositionVm.EmployeeTypeId;
                Session["primaryPosition"] = e_PositionVm.PrimaryPosition;
            }
            if (e_PositionVm != null)
            {
                e_PositionVm.PersonId = personId;
                ViewBag.ePositionId = e_PositionVm.E_PositionId;
                e_PositionVm.PersonName = clientDbContext.Persons.Where(x => x.PersonId == personId).Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();
                e_PositionVm.PositionDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPositionList());
                e_PositionVm.PayFrequencyDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPayFrequencyList());
                e_PositionVm.RateTypeDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetRateTypeList());
                e_PositionVm.PositionCategoryDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPositionCategoryList());
                e_PositionVm.PositionGradeDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetSalaryGradeList());
                e_PositionVm.PositionTypeDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPositionTypeList());
                e_PositionVm.EmployeeTypeDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetEmployeeTypeList());
                e_PositionVm.PayGroupDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPayGroupList());
                e_PositionVm.ReportsToIdDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetReportsToIdList());
                e_PositionVm.EmployeeClassList = _epositionRepository.GetEmployeeClassList();
                e_PositionVm.PositionCode = PositionCode;
            }

            //Returns the EmployeeTypeDescription for Third Tab.
            var emptypeid = e_PositionVm.EmployeeTypeId;
            var emptypedesc = clientDbContext.DdlEmployeeTypes.Where(x => x.EmployeeTypeId == emptypeid).Select(m => m.Description).FirstOrDefault();
            var emptypeCode = clientDbContext.DdlEmployeeTypes.Where(x => x.EmployeeTypeId == emptypeid).Select(m => m.Code).FirstOrDefault();
            var terminationDate = clientDbContext.Employees.Where(x => x.EmployeeId == employeeId).Select(m => m.TerminationDate).FirstOrDefault();
            ViewBag.EmployeeTypeDescription = emptypedesc;
            TempData["EmployeeTypeDescription"] = emptypedesc;
            TempData["EmployeeTypeCode"] = emptypeCode;
            //TempData["TeminationDate"] = terminationDate;
            ViewBag.TerminationDate = terminationDate;
            TempData.Keep();

            return e_PositionVm;
        }
        public string GetPositionList()
        {
            var _list = _epositionRepository.GetPositionList().CleanUp();
            return JsonConvert.SerializeObject(_list);
        }
        public string GetPayFrequencyList()
        {
            var _list = _epositionRepository.GetPayFrequencyList().CleanUp();
            return JsonConvert.SerializeObject(_list);
        }
        public string GetRateTypeList()
        {
            var _list = _epositionRepository.GetRateTypeList().CleanUp();
            return JsonConvert.SerializeObject(_list);
        }
        public string GetEmployeeTypeList()
        {
            var _list = _epositionRepository.GetEmployeeTypeList().CleanUp();
            return JsonConvert.SerializeObject(_list);
        }

        public void GetEmpTypeList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var _list = clientDbContext.DdlEmployeeTypes.Where(m => m.Active == true).Select(m => new DropDownModel { keyvalue = m.Code, keydescription = m.Description }).OrderBy(x => x.keydescription).ToList();
            ViewData["EmpTypeList"] = _list;
        }

        public void GetMemoCode1List()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var _list = clientDbContext.EarningsCodes.GroupBy(x => x.EarningsCodeOffset).Select(x => x.FirstOrDefault()).ToList();
            ViewData["MemoCode1List"] = _list;
        }

        public void GetDepartmentList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var _list = clientDbContext.Departments.Where(m => m.IsDepartmentActive == true && m.IsDeleted == false).Select(m => new DropDownModel { keyvalue = m.DepartmentId.ToString(), keydescription = m.DepartmentDescription }).OrderBy(x => x.keydescription).ToList();
            ViewData["DepartmentList"] = _list;
        }
        public void GetSemisterList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var _list = clientDbContext.DdlSemisters.Where(m => m.Active == true).Select(m => new DropDownModel { keyvalue = m.SemisterID.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList();
            ViewData["SemisterList"] = _list;
        }
        public void GetGLCodesList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var _list = clientDbContext.DdlGLCodes.Where(m => m.Active == true).Select(m => new DropDownModel { keyvalue = m.GLCodeId.ToString(), keydescription = m.Code + "-" + m.Description }).OrderBy(x => x.keydescription).ToList();
            ViewData["GLCodesList"] = _list;
        }
        public void GetRateTypesList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var _list = clientDbContext.DdlRateTypes.Where(m => m.Active == true).Select(m => new DropDownModel { keyvalue = m.RateTypeId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList();
            ViewData["RateTypeList"] = _list;
            ViewData["RateTypeListAdd"] = _list;
        }

        public void GetMemoCode2List()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var _list = clientDbContext.EarningsCodes.GroupBy(x => x.DeductionCodeOffset).Select(x => x.FirstOrDefault()).ToList();
            ViewData["MemoCode2List"] = _list;
        }

        public void GetEarningCodesList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var compyid = Convert.ToInt32(Session["compycodeid"]);
            var _list = clientDbContext.EarningsCodes.Where(x => x.CompanyCodeId == compyid).GroupBy(x => x.EarningsCodeCode).Select(x => x.FirstOrDefault()).ToList();
            ViewData["EarningCodesList"] = _list;
        }

        public string GetPayGroupList()
        {
            var _list = _epositionRepository.GetPayGroupList().CleanUp();
            return JsonConvert.SerializeObject(_list);
        }

        public string GetReportsToIdList()
        {
            var _list = _epositionRepository.GetReportsToIdList().CleanUp();
            return JsonConvert.SerializeObject(_list);
        }

        public ActionResult E_PositionsIndexChangedAjax(int e_PositionIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            E_PositioVm e_PositionVm;

            if (e_PositionIdDdl < 1)
            {
                e_PositionVm = new E_PositioVm();
                e_PositionVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                e_PositionVm.PersonId = personId;
                e_PositionVm.E_PositionId = 0;
            }
            else
                e_PositionVm = GetE_PositionsRecord(e_PositionIdDdl, personId);

            ModelState.Clear();
            return Json(e_PositionVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult E_PositionsSaveAjax(E_PositioVm ePositionVm)
        {
            bool isnew = Convert.ToBoolean(Session["IsnewRecords"]);
            bool isEdit = Convert.ToBoolean(Session["IsEditRecords"]);
            var positionId = Convert.ToUInt32(Session["positionId"]);
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string requestType = User.Identity.GetRequestType();
            TempData["EmpId"] = Session["EmpId"];
            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && ePositionVm.E_PositionId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the employee the Position record is for.");
                    return View(ePositionVm);
                }
                else
                {
                    ePositionVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID));
                }
            }
            if (ModelState.IsValid)
            {
                E_Positions e_Position = clientDbContext.E_Positions
                    .Where(x => x.E_PositionId == ePositionVm.E_PositionId).SingleOrDefault();
                var Empnumber = Convert.ToInt32(Session["EmpNumber"]);
                var EmployeeId = clientDbContext.Employees
                   .Where(x => x.PersonId == ePositionVm.PersonId && x.EmploymentNumber == Empnumber).Select(a => a.EmployeeId).SingleOrDefault();

                var e_PositionId = ePositionVm.E_PositionId;
                int PositionId = ePositionVm.PositionId;
                if (ePositionVm.E_PositionId != null)
                {
                    Session["ePositionId"] = e_PositionId;
                    PositionId = clientDbContext.E_Positions.Where(x => x.E_PositionId == e_PositionId).Select(m => m.PositionId).FirstOrDefault();
                }
                dynamic e_PositionListExists = 0;
                if (PositionId != 0)
                {
                    e_PositionListExists = clientDbContext.E_Positions.Where(m => m.PositionId == PositionId && m.EmployeeId == EmployeeId && m.IsDeleted == false).ToList();
                }
                var recordexistsineposdb = clientDbContext.E_Positions.Where(x => x.EmployeeId == EmployeeId && x.IsDeleted == false).ToList();

                if (isnew == true && recordexistsineposdb != null && recordexistsineposdb.Count > 0)
                {
                    foreach (var items in recordexistsineposdb)
                    {
                        if ((items.EmployeeId == EmployeeId) && (items.PositionId == PositionId) && (ePositionVm.StartDate >= items.StartDate && ePositionVm.StartDate <= items.actualEndDate))
                        {
                            return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "Start Date is Overlap with existing dates") }, JsonRequestBehavior.AllowGet);
                        }
                        if ((items.EmployeeId == EmployeeId) && (items.PositionId == PositionId) && (ePositionVm.actualEndDate >= items.StartDate && ePositionVm.actualEndDate <= items.actualEndDate))
                        {
                            return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "End Date is Overlap with existing dates") }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (e_PositionListExists.Count > 1)
                    {
                        var isRecordsExists = clientDbContext.E_Positions.Where(x => x.PositionId == e_Position.PositionId && x.EmployeeId == EmployeeId && x.StartDate != null
                                                                                &&
                                                                                (ePositionVm.actualEndDate != null && (
                                                                                    (x.actualEndDate != null && ePositionVm.StartDate > x.actualEndDate) || (x.actualEndDate != null && ePositionVm.actualEndDate < x.StartDate))
                                                                                 )).Select(a => a.PositionId).FirstOrDefault();
                        if (e_Position != null && isRecordsExists == 0 && ePositionVm.actualEndDate != null)
                        {
                            return Json(new { succeed = false, Message = "The employee position record  exists for the selected position." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                if (isEdit == true && recordexistsineposdb != null && recordexistsineposdb.Count > 0)
                {
                    foreach (var items in recordexistsineposdb)
                    {
                        if ((items.EmployeeId == EmployeeId) && (items.PositionId == PositionId) && (items.E_PositionId != ePositionVm.E_PositionId) && (ePositionVm.StartDate >= items.StartDate && ePositionVm.StartDate <= items.actualEndDate))
                        {
                            return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "Start Date is Overlap with existing dates") }, JsonRequestBehavior.AllowGet);
                        }
                        if ((items.EmployeeId == EmployeeId) && (items.PositionId == PositionId) && (items.E_PositionId != ePositionVm.E_PositionId) && (ePositionVm.actualEndDate >= items.StartDate && ePositionVm.actualEndDate <= items.actualEndDate))
                        {
                            return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "End Date is Overlap with existing dates") }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (e_PositionListExists.Count > 1)
                    {
                        var isRecordsExists = clientDbContext.E_Positions.Where(x => x.PositionId == e_Position.PositionId && x.EmployeeId == EmployeeId && x.StartDate != null
                                                                                &&
                                                                                (ePositionVm.actualEndDate != null && (
                                                                                    (x.actualEndDate != null && ePositionVm.StartDate > x.actualEndDate) || (x.actualEndDate != null && ePositionVm.actualEndDate < x.StartDate))
                                                                                 )).Select(a => a.PositionId).FirstOrDefault();
                        if (e_Position != null && isRecordsExists == 0 && ePositionVm.actualEndDate != null)
                        {
                            return Json(new { succeed = false, Message = "The employee position record  exists for the selected position." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                if (e_Position == null)
                {
                    e_Position = new E_Positions
                    {
                        EnteredBy = User.Identity.Name,
                        EnteredDate = DateTime.Now,
                        EmployeeId = ePositionVm.EmployeeId
                    };
                    recordIsNew = true;
                }

                if (!string.IsNullOrEmpty(ePositionVm.PayFrequencyDescription))
                {
                    int payFrequencyInDb = clientDbContext.DdlPayFrequencies
                        .Where(x => x.Description == ePositionVm.PayFrequencyDescription).Select(x => x.PayFrequencyId).SingleOrDefault();

                    if (payFrequencyInDb <= 0)
                    {
                        return Json(new { succeed = false, Message = "The Pay Frequency type does not exist." }, JsonRequestBehavior.AllowGet);
                    }
                    else e_Position.PayFrequencyId = payFrequencyInDb;
                }

                if (!string.IsNullOrEmpty(ePositionVm.RateTypeDescription))
                {
                    var rateTypeInDb = clientDbContext.DdlRateTypes
                        .Where(x => x.Description == ePositionVm.RateTypeDescription).Select(x => x.RateTypeId).SingleOrDefault();

                    if (rateTypeInDb <= 0)
                    {
                        return Json(new { succeed = false, Message = "The Rate type does not exist." }, JsonRequestBehavior.AllowGet);
                    }
                    else e_Position.RateTypeId = rateTypeInDb;
                }
                if (!string.IsNullOrEmpty(ePositionVm.PositionCode))
                {
                    int positionInDb = clientDbContext.Positions
                        .Where(x => x.PositionCode == ePositionVm.PositionCode && x.Suffix == ePositionVm.Suffix).Select(x => x.PositionId).FirstOrDefault();

                    if (positionInDb <= 0)
                    {
                        return Json(new { succeed = false, Message = "The Position does not exist." }, JsonRequestBehavior.AllowGet);
                    }
                    else e_Position.PositionId = positionInDb;
                }
                e_Position.Notes = ePositionVm.Notes;
                int emplNo = Convert.ToInt32(Session["EmplNo"]);
                int empNo = Convert.ToInt32(TempData["EmpNo"]);
                var empId = clientDbContext.Employees.Where(m => m.PersonId == ePositionVm.PersonId && m.EmploymentNumber == emplNo).Select(m => m.EmployeeId).FirstOrDefault();
                var empRecordExists = clientDbContext.E_Positions.Where(m => m.EmployeeId == empId).ToList();
                var positionExists = clientDbContext.E_Positions.Where(n => n.EmployeeId == empId).Select(n => n.PositionId).ToList();
                var newposition = clientDbContext.Positions.Where(m => m.PositionDescription == ePositionVm.PositionDescription).Select(m => m.PositionId).FirstOrDefault();
                var selectedPosId = clientDbContext.E_Positions.Where(m => m.E_PositionId == ePositionVm.E_PositionId).Select(m => m.PositionId).FirstOrDefault();
                var editRecordempId = clientDbContext.E_Positions.Where(s => s.E_PositionId == ePositionVm.E_PositionId).Select(s => s.EmployeeId).FirstOrDefault();
                var existePosList = clientDbContext.E_Positions.Where(m => m.EmployeeId == editRecordempId && m.PositionId != selectedPosId).ToList();
                if (ePositionVm.E_PositionId == 0)
                {
                    if (positionExists.Contains(newposition))
                    {
                        return Json(new { succeed = false, Message = "Selected Position already exists" }, JsonRequestBehavior.AllowGet);
                    }
                    //else if (empRecordExists.Count() > 0 && ePositionVm.PrimaryPosition == true && empRecordExists.Any(m => m.PrimaryPosition.Value) == true)
                    //{
                    //    return Json(new { succeed = false, Message = "Primary Position already exists" }, JsonRequestBehavior.AllowGet);
                    //}
                    else
                    {
                        e_Position.PrimaryPosition = ePositionVm.PrimaryPosition;
                    }
                }
                else
                {
                    //if (ePositionVm.PrimaryPosition == true && existePosList.Count() > 0 && existePosList.Any(s => s.PrimaryPosition.Value) == true)
                    //{
                    //    return Json(new { succeed = false, Message = "Primary Position already exists" }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    e_Position.PrimaryPosition = ePositionVm.PrimaryPosition;
                    //  }
                }
                int result = 0;
                if (ePositionVm.StartDate != null && ePositionVm.actualEndDate != null)
                {
                    result = DateTime.Compare((DateTime)ePositionVm.StartDate, (DateTime)ePositionVm.actualEndDate);
                }

                if (result > 0 && ePositionVm.actualEndDate != null && ePositionVm.StartDate != null)
                {
                    return Json(new { succeed = false, Message = "Actual End Date can't be less than Start Date." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    e_Position.StartDate = ePositionVm.StartDate;
                    e_Position.actualEndDate = ePositionVm.actualEndDate;
                }
                e_Position.StartDate = ePositionVm.StartDate;
                e_Position.actualEndDate = ePositionVm.actualEndDate;
                e_Position.ProjectedEndDate = ePositionVm.projectedEndDate;
                var existEmpId = clientDbContext.E_Positions.Where(m => m.E_PositionId == e_Position.E_PositionId).Select(m => m.EmployeeId).FirstOrDefault();
                if (e_Position.E_PositionId > 0)
                {
                    e_Position.EmployeeId = existEmpId;
                }
                else
                {
                    e_Position.EmployeeId = empId;
                }
                var empCompanyCodeId = clientDbContext.Employees.Where(x => x.EmployeeId == e_Position.EmployeeId).Select(x => x.CompanyCodeId).FirstOrDefault();
                e_Position.PayFrequencyId = ePositionVm.PayFrequencyId;
                e_Position.RateTypeId = ePositionVm.RateTypeId;
                e_Position.FTE = ePositionVm.FTE;
                e_Position.PositionCategoryID = ePositionVm.PositionCategoryID;
                e_Position.PositionGradeID = ePositionVm.PositionGradeID;
                e_Position.PrimaryPosition = ePositionVm.PrimaryPosition;
                e_Position.PositionTypeID = ePositionVm.PositionTypeID;
                e_Position.EmployeeTypeId = ePositionVm.EmployeeTypeId;
                e_Position.PayGroupId = ePositionVm.PayGroupId;
                e_Position.salary = TempData["Salary"] == null ? null : (Decimal?)(TempData["Salary"]);
                e_Position.ReportsToID = ePositionVm.ReportsToID;
                e_Position.AdpYear = ePositionVm.AdpYear;
                e_Position.AdpWSLimit = ePositionVm.AdpWSLimit;
                e_Position.DepartmentId = clientDbContext.Departments.Where(x => x.DepartmentDescription == ePositionVm.BusinessLevel && x.IsDeleted == false && x.CompanyCodeId == empCompanyCodeId).Select(r => r.DepartmentId).FirstOrDefault();
                if (e_Position.DepartmentId == 0)
                {
                    var departmentId = clientDbContext.Departments.Where(x => x.DepartmentDescription == ePositionVm.BusinessLevel).FirstOrDefault();
                    Department department = new Department()
                    {
                        CompanyCodeId = empCompanyCodeId.Value,
                        DepartmentDescription = ePositionVm.BusinessLevel,
                        DepartmentCode = departmentId.DepartmentCode,
                        IsDepartmentActive = true,
                        IsDeleted = false
                    };
                    clientDbContext.Departments.Add(department);
                    clientDbContext.SaveChanges();
                    e_Position.DepartmentId = department.DepartmentId;
                }
                e_Position.IsDeleted = false;
                e_Position.UserId = User.Identity.Name;
                e_Position.EmployeeClassId = ePositionVm.EmployeeClassId;
                if (ePositionVm.CostNumber == null)
                {
                    ePositionVm.CostNumber = "000000000000000000";
                    e_Position.CostNumber = ePositionVm.CostNumber;
                }
                else
                {
                    e_Position.CostNumber = ePositionVm.CostNumber;
                }
                if (ePositionVm.CostNumber1Percent == null)
                {
                    ePositionVm.CostNumber1Percent = 100;
                    e_Position.CostNumber1Percent = ePositionVm.CostNumber1Percent;
                }
                else
                {
                    e_Position.CostNumber1Percent = ePositionVm.CostNumber1Percent;
                }
                e_Position.CostNumber2Account = ePositionVm.CostNumber2Account;
                e_Position.CostNumber2Percent = ePositionVm.CostNumber2Percent;
                e_Position.CostNumber3Account = ePositionVm.CostNumber3Account;
                e_Position.CostNumber3Percent = ePositionVm.CostNumber3Percent;
                e_Position.CostNumber4Account = ePositionVm.CostNumber4Account;
                e_Position.CostNumber4Percent = ePositionVm.CostNumber4Percent;
                e_Position.CostNumber5Account = ePositionVm.CostNumber5Account;
                e_Position.CostNumber6Account = ePositionVm.CostNumber6Account;
                e_Position.CostNumber5Percent = ePositionVm.CostNumber5Percent;
                e_Position.CostNumber6Percent = ePositionVm.CostNumber6Percent;
                e_Position.CostNumberEffectiveDate = ePositionVm.CostNumberEffectiveDate;               
                if (recordIsNew)
                {
                    e_Position.EnteredBy = User.Identity.Name;
                    e_Position.EnteredDate = DateTime.Now;
                    clientDbContext.E_Positions.Add(e_Position);                    
                }
                else
                {
                    e_Position.ModifiedBy = User.Identity.Name;
                    e_Position.ModifiedDate = DateTime.Now;                    
                    if (ePositionVm.actualEndDate != null)
                    {
                        DateTime etcCurrentDate = Utils.ConvertTimeFromUtc(DateTime.UtcNow, ConfigurationManager.AppSettings["TimeZone"]);
                        string connString = User.Identity.GetClientConnectionString();
                        using (ClientDbContext clientDbContext1 = new ClientDbContext(connString))
                         {
                            DateTime? actualEndDate = ePositionVm.actualEndDate;                            
                              //if (actualEndDate.Value.Date >= etcCurrentDate.Date)
                              //  {
                                    var lasteffectivedate = clientDbContext.E_PositionSalaryHistories.Where(m => m.E_PositionId == ePositionVm.E_PositionId).OrderByDescending(m => m.EffectiveDate).FirstOrDefault();

                                E_PositionSalaryHistories newitem = new E_PositionSalaryHistories
                                {
                                    UserId = User.Identity.Name,
                                    LastModifiedDate = DateTime.Now,
                                    EndDate = ePositionVm.actualEndDate,
                                    E_PositionId = ePositionVm.E_PositionId
                                };
                                clientDbContext1.E_PositionSalaryHistories.Add(newitem);
                                clientDbContext1.SaveChanges();
                                clientDbContext.Database.ExecuteSqlCommand("Update [E_PositionSalaryHistories] SET EndDate = @EndDate WHERE E_PositionSalaryHistoryId = @E_PositionSalaryHistoryId", new SqlParameter("@EndDate", ePositionVm.actualEndDate), new SqlParameter("@E_PositionSalaryHistoryId", lasteffectivedate.E_PositionSalaryHistoryId));
                           //}
                        }
                    }
                }
                clientDbContext.SaveChanges();
                var position = clientDbContext.Positions.SingleOrDefault(x => x.PositionId == PositionId);
                if (position != null)
                {
                    position.Suffix = ePositionVm.Suffix;
                    clientDbContext.SaveChanges();
                }
                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "New Employee Position Record Added" : "Employee Position Record Saved";
                    ePositionVm = GetE_PositionsRecord(e_Position.E_PositionId, ePositionVm.PersonId);
                    Session["IsnewRecords"] = false;
                }
                catch (Exception err)
                {
                    ViewBag.AlertMessage = "";
                    IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                    if (errors.Count() == 0)
                        ModelState.AddModelError("", err.InnerException.Message);
                    else
                    {
                        foreach (DbEntityValidationResult error in errors)
                        {
                            foreach (var valError in error.ValidationErrors)
                            {
                                ModelState.AddModelError("", valError.ErrorMessage);
                            }
                        }
                    }
                }
            }
            else
            {
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                string _message = "";
                foreach (var item in modelStateErrors)
                {
                    if (_message != "") _message += "\n";
                    _message += item.ErrorMessage;
                }
                _message += "\n\nRecord could not be save at this time.";
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { e_PositionVm = ePositionVm, succeed = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult E_PositionsDelete(int ePositionId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.E_Positions.Where(x => x.E_PositionId == ePositionId).SingleOrDefault();
            if (dbRecord != null)
            {
                clientDbContext.E_Positions.Remove(dbRecord);
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
        public ActionResult E_PositionsDeleteAjax(int e_PositionIdDdl, int personId)
        {
            if (e_PositionIdDdl < 1)
                return Json("The employee position Record does not exist", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            //// this code is duplicated from get PersonAda() action
            E_PositioVm e_PositionVm;

            int e_PositionId = clientDbContext.E_Positions
                .Include("Employee.Person")
                .Where(x => x.Employee.Person.PersonId == personId).Select(x => x.E_PositionId).FirstOrDefault();

            if (e_PositionIdDdl < 1)
            {
                e_PositionVm = new E_PositioVm();
                e_PositionVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                e_PositionVm.PersonId = personId;
                e_PositionVm.E_PositionId = 0;
            }
            else
            {
                e_PositionVm = GetE_PositionsRecord(e_PositionId, personId);
            }
            var e_poisitions = clientDbContext.E_Positions.Where(x => x.E_PositionId == e_PositionIdDdl).SingleOrDefault();

            if ((e_poisitions.actualEndDate != null && e_poisitions.actualEndDate < DateTime.Now.Date) || (e_poisitions.StartDate == null && e_poisitions.actualEndDate == null))
            {
                if (clientDbContext.TimeCards.Any(x => x.PositionId == e_poisitions.PositionId && x.EmployeeId == e_poisitions.EmployeeId && x.IsDeleted == false))
                {
                    return Json(new { Message = "Position already in use.", succeed = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    clientDbContext.Database.ExecuteSqlCommand(" " +
                                        "UPDATE E_PositionSalaryHistories SET IsDeleted = @IsDeleted, DeletedBy = @DeletedBy, LastModifiedDate = @Date " +
                                        "WHERE E_PositionId = @E_PositionId ",
                                        new SqlParameter("@E_PositionId", e_PositionIdDdl),
                                        new SqlParameter("@IsDeleted", 1),
                                        new SqlParameter("@DeletedBy", User.Identity.Name),
                                        new SqlParameter("@Date", DateTime.Now));
                    clientDbContext.Database.ExecuteSqlCommand(" " +
                                        "UPDATE E_Positions SET IsDeleted = @IsDeleted, DeletedBy = @DeletedBy, LastModifiedDate = @Date " +
                                        "WHERE E_PositionId = @E_PositionId ",
                                        new SqlParameter("@E_PositionId", e_PositionIdDdl),
                                        new SqlParameter("@IsDeleted", 1),
                                        new SqlParameter("@DeletedBy", User.Identity.Name),
                                        new SqlParameter("@Date", DateTime.Now));
                    return Json(new { e_PositionVm, succeed = true, Message = "Record deleted successfully!" }, JsonRequestBehavior.AllowGet);
                }
            }
            ModelState.Clear();
            return Json(new { e_PositionVm, succeed = false, Message = "Position not Ended/End Date is not assigned!" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetE_PositionsList(int? EmployeeIdDdl)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string requestType = User.Identity.GetRequestType();
            var e_PositionList = clientDbContext.E_Positions
            .Include("Positions")
            .Where(x => x.EmployeeId == EmployeeIdDdl && x.IsDeleted == false)
            .Select(m => new
            {
                EmployeePositionId = m.E_PositionId,
                EmployeePositionDescription = m.Position.PositionDescription
            }).OrderBy(m => m.EmployeePositionDescription).ToList();

            return Json(e_PositionList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult E_PositionSalaryHistoriesList_Read([DataSourceRequest]DataSourceRequest request, int e_PositionIdDdl)
        {
            if (e_PositionIdDdl < 1)
                return Json("History not exist", JsonRequestBehavior.AllowGet);

            List<E_PositioVm> e_PositionSalaryHistoriesList = new List<E_PositioVm>();
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            {
                e_PositionSalaryHistoriesList = (from eps in clientDbContext.E_PositionSalaryHistories
                                                 join ep in clientDbContext.E_Positions on eps.E_PositionId equals ep.E_PositionId
                                                 join drt in clientDbContext.DdlRateTypes on eps.RateTypeId equals drt.RateTypeId
                                                 orderby eps.EffectiveDate descending
                                                 where eps.E_PositionId == e_PositionIdDdl && eps.EffectiveDate != null
                                                 select new E_PositioVm
                                                 {
                                                     E_PositionSalaryHistoryId = eps.E_PositionSalaryHistoryId,
                                                     RateTypeId = drt.RateTypeId,
                                                     RateTypeDescription = drt.Description == "S" ? "Salary" : drt.Description == "H" ? "Hourly" : "",                                                     // RateTypeDescription = e.RateTypeId == null ? "Select" : e.RateTypeId == 1 ? "Salary" : e.RateTypeId == 2 ? "Hourly" : "",
                                                     PayRate = eps.PayRate,
                                                     HoursPerPayPeriod = eps.HoursPerPayPeriod,
                                                     AnnualSalary = eps.AnnualSalary,
                                                     EffectiveDate = eps.EffectiveDate,
                                                     EndDate = eps.EndDate,
                                                 }).ToList();
                return Json(e_PositionSalaryHistoriesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }
        //For Salary grid
        //public ActionResult E_PositionSalaryHistoriesList_Read([DataSourceRequest]DataSourceRequest request, int e_PositionIdDdl)
        //{
        //    if (e_PositionIdDdl < 1)
        //        return Json("History not exist", JsonRequestBehavior.AllowGet);
        //    string connString = User.Identity.GetClientConnectionString();
        //    ClientDbContext clientDbContext = new ClientDbContext(connString);
        //    var posId = clientDbContext.E_Positions.Where(m => m.E_PositionId == e_PositionIdDdl).Select(m => m.PositionGradeID).FirstOrDefault();
        //    string grade = "";
        //    if (posId != 0)
        //    {
        //        grade = clientDbContext.DdlSalaryGrades.Where(s => s.SalaryGradeID == posId).Select(s => s.description).FirstOrDefault() == null ? "" : clientDbContext.DdlSalaryGrades.Where(s => s.SalaryGradeID == posId).Select(s => s.description).FirstOrDefault();
        //    }
        //    {
        //        var e_PositionSalaryHistoriesList = clientDbContext.E_PositionSalaryHistories
        //            .Include("E_Positions.DdlPayFrequency")
        //             .Include("E_Positions.DdlPositionGrade")
        //            .OrderByDescending(e => e.EffectiveDate)

        //            .Where(x => (x.E_PositionId == e_PositionIdDdl && x.EffectiveDate != null)).ToList()
        //            .Select(e => new E_PositionSalaryHistorVm
        //            {
        //                RateTypeId = e.RateTypeId,
        //                PayRate = e.PayRate,
        //                // Grade = grade,
        //                HoursPerPayPeriod = e.HoursPerPayPeriod,
        //                AnnualSalary = e.AnnualSalary,
        //                EffectiveDate = e.EffectiveDate,
        //                EndDate = e.EndDate
        //            });

        //        // return Json(e_PositionSalaryHistoriesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //        return KendoCustomResult(e_PositionSalaryHistoriesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //    }
        //}
        public ActionResult EPositionSalaryHistoryList_Read([DataSourceRequest]DataSourceRequest request, int e_PositionId)
        {
            if (e_PositionId < 1)
                return Json("History not exist", JsonRequestBehavior.AllowGet);

            List<E_PositioVm> e_PositionSalaryHistoriesList = new List<E_PositioVm>();
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            {
                e_PositionSalaryHistoriesList = (from eps in clientDbContext.E_PositionSalaryHistories
                                                 join drt in clientDbContext.DdlRateTypes on eps.RateTypeId equals drt.RateTypeId
                                                 orderby eps.EffectiveDate descending
                                                 where eps.E_PositionId == e_PositionId && eps.EffectiveDate != null
                                                 select new E_PositioVm
                                                 {
                                                     E_PositionSalaryHistoryId = eps.E_PositionSalaryHistoryId,
                                                     RateTypeId = drt.RateTypeId,
                                                     RateTypeDescription = drt.Description == "S" ? "Salary" : drt.Description == "H" ? "Hourly" : "",                                                     // RateTypeDescription = e.RateTypeId == null ? "Select" : e.RateTypeId == 1 ? "Salary" : e.RateTypeId == 2 ? "Hourly" : "",
                                                     PayRate = eps.PayRate,
                                                     HoursPerPayPeriod = eps.HoursPerPayPeriod,
                                                     AnnualSalary = eps.AnnualSalary,
                                                     EffectiveDate = eps.EffectiveDate,
                                                     EndDate = eps.EndDate
                                                 }).ToList();
                return Json(e_PositionSalaryHistoriesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult ContractList_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<ContractsVm> contactsvm = new List<ContractsVm>();
            string connString = User.Identity.GetClientConnectionString();
            int ePositionId = Convert.ToInt32(Session["ePositionId"]);
            ClientDbContext clientdbcontext = new ClientDbContext(connString);
            {

                contactsvm = clientdbcontext.Contracts
                    .Select(e => new ContractsVm
                    {
                        Id = e.Id,
                        PayPeriod = e.PayPeriod,
                        Status1FlagCode = e.Status1FlagCode,
                        EarningsCode = clientdbcontext.EarningsCodes.Where(x => x.EarningsCodeCode == e.EarningCodes).Select(x => x.EarningsCodeCode).FirstOrDefault(),
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        AmountsInDollar = e.AmountsInDollar,
                        ADJlimits = e.ADJlimits,
                        Semister = clientdbcontext.DdlSemisters.Where(x => x.SemisterID == e.SemisterId).Select(x => x.Description).FirstOrDefault(),
                        GLCode = clientdbcontext.DdlGLCodes.Where(x => x.GLCodeId == e.GLCodeId).Select(x => x.Description).FirstOrDefault(),
                        Amounts = e.Amounts,
                        Department = e.Department,
                        MemoCode1 = e.MemoCode1,
                        MemoCode1Amount = e.MemoCode1Amount,
                        MemoCode2 = e.MemoCode2,
                        MemoCode2Amount = e.MemoCode2Amount,
                        EpositionId = e.EpositionId,
                        TreatyCode = clientdbcontext.EarningsCodes.Where(x => x.EarningsCodeCode == e.EarningCodes).Select(x => x.TreatyCode).FirstOrDefault()
                    }).Where(x => x.EpositionId == ePositionId).ToList();

                foreach (var item in contactsvm)
                {
                    string statusFlag1 = Convert.ToString(item.Status1FlagCode);
                    var status1FlagDesc = clientdbcontext.DdlEmployeeTypes.Where(x => x.Code == statusFlag1).Select(x => x.Description).FirstOrDefault();
                    item.Status1FlagCodeDesc = status1FlagDesc;

                }
                //Add Edit calc 
                var Contracts = clientdbcontext.Contracts.FirstOrDefault(x => x.EpositionId == ePositionId);
                if (Contracts != null && Contracts.PayPeriod == 0)
                {
                    var Periodcount = getPayperiodscount(Convert.ToDateTime(Contracts.StartDate), Convert.ToDateTime(Contracts.EndDate));
                    Contracts.PayPeriod = Periodcount;
                    clientdbcontext.SaveChanges();
                }
                return Json(contactsvm.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult E_PositionSalaryHistoriesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.E_PositionSalaryHistories e_PositionSalaryHistory)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (e_PositionSalaryHistory != null && ModelState.IsValid)
                {
                    //Check for duplicate record
                    var e_PositionSalaryHistoryInDb = clientDbContext.E_PositionSalaryHistories
                        .Where(x => x.E_PositionId == e_PositionSalaryHistory.E_PositionId && x.EffectiveDate == e_PositionSalaryHistory.EffectiveDate)
                        .SingleOrDefault();

                    if (e_PositionSalaryHistoryInDb != null)
                    {
                        ModelState.AddModelError("", "The details are already defined.");
                    }
                    else
                    {
                        var newE_PositionSalaryHistory = new E_PositionSalaryHistories
                        {
                            E_PositionId = e_PositionSalaryHistory.E_PositionId,
                            EffectiveDate = e_PositionSalaryHistory.EffectiveDate,
                            PayRate = e_PositionSalaryHistory.PayRate,
                            HoursPerPayPeriod = e_PositionSalaryHistory.HoursPerPayPeriod,
                            IsDeleted = false,
                            UserId = User.Identity.Name,
                            LastModifiedDate = DateTime.Now
                        };

                        clientDbContext.E_PositionSalaryHistories.Add(newE_PositionSalaryHistory);
                        clientDbContext.SaveChanges();
                        e_PositionSalaryHistory.E_PositionSalaryHistoryId = newE_PositionSalaryHistory.E_PositionId;
                    }
                }

                return Json(new[] { e_PositionSalaryHistory }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult E_PositionSalaryHistoriesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.E_PositionSalaryHistories e_PositionSalaryHistory)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (e_PositionSalaryHistory != null && ModelState.IsValid)
                {
                    var e_PositionSalaryInDb = clientDbContext.E_PositionSalaryHistories
                        .Where(x => x.E_PositionId == e_PositionSalaryHistory.E_PositionId && x.EffectiveDate == e_PositionSalaryHistory.EffectiveDate)
                        .SingleOrDefault();

                    if (e_PositionSalaryInDb != null)
                    {
                        e_PositionSalaryInDb.EffectiveDate = e_PositionSalaryHistory.EffectiveDate;
                        e_PositionSalaryInDb.PayRate = e_PositionSalaryHistory.PayRate;
                        e_PositionSalaryInDb.HoursPerPayPeriod = e_PositionSalaryHistory.HoursPerPayPeriod;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { e_PositionSalaryHistory }.ToDataSourceResult(request, ModelState));
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult E_PositionSalaryHistoriesList_Destroy([DataSourceRequest] DataSourceRequest request
            , E_PositionSalaryHistories e_PositionSalaryHistory)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (e_PositionSalaryHistory != null)
                {
                    E_PositionSalaryHistories e_PositionSalaryHistoryInDb = clientDbContext.E_PositionSalaryHistories
                        .Where(x => x.E_PositionSalaryHistoryId == e_PositionSalaryHistory.E_PositionSalaryHistoryId).SingleOrDefault();

                    if (e_PositionSalaryHistoryInDb != null)
                    {
                        clientDbContext.E_PositionSalaryHistories.Remove(e_PositionSalaryHistoryInDb);

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

                return Json(new[] { e_PositionSalaryHistory }.ToDataSourceResult(request, ModelState));
            }
        }
        public ActionResult ViewSalaryDetail(int e_PositionId)
        {
            return RedirectToAction("E_PositionSalaryHistoriesList_Read", new { e_PositionIdDdl = e_PositionId });
        }
        public ActionResult EditePositionDetail(int ePositionId, int personId)
        {
            ViewBag.E_PositionId = ePositionId;
            Session["IsEditRecords"] = true;
            GetEmpTypeList();
            GetMemoCode1List();
            GetMemoCode2List();
            GetDepartmentList();
            GetSemisterList();
            GetEarningCodesList();
            GetRateTypesList();
            GetGLCodesList();
            return PartialView(GetE_PositionsRecord(ePositionId, personId));
        }

        [HttpPost]
        public ActionResult SaveContracts(ContractsVm contractsVm)
        {
            string connString = User.Identity.GetClientConnectionString();
            int epositionId = Convert.ToInt32(Session["ePositionId"]);
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            Contracts contracts = new Contracts();
            contracts.EarningsCodeId = contractsVm.EarningsCodeId;
            contracts.MemoCode1 = contractsVm.MemoCode1;
            contracts.MemoCode1Amount = contractsVm.MemoCode1Amount;
            contracts.MemoCode2 = contractsVm.MemoCode2;
            contracts.MemoCode2Amount = contractsVm.MemoCode2Amount;
            contracts.PayPeriod = contractsVm.PayPeriod;
            contracts.EpositionId = epositionId;
            contracts.EarningCodes = contractsVm.EarningCodes;
            if (Session["EmployeeTypeId"] == null || Convert.ToString(Session["EmployeeTypeId"]) == "0")
            {
                contracts.Status1FlagCode = null;
            }
            else
            {
                contracts.Status1FlagCode = Convert.ToInt32(Session["EmployeeTypeId"]);
            }
            contracts.StartDate = contractsVm.StartDate;
            contracts.EndDate = contractsVm.EndDate;
            contracts.Amounts = contractsVm.Amounts;
            contracts.Department = contractsVm.Department;
            contracts.AmountsInDollar = contractsVm.AmountsInDollar;
            contracts.Job = contractsVm.Job;
            contracts.Notes = contractsVm.Notes;
            contracts.AddNewContract = contractsVm.AddNewContract;
            contracts.ADJlimits = contractsVm.ADJlimits;
            contracts.SemisterId = contractsVm.SemisterId;
            contracts.GLCodeId = contractsVm.GLCodeId;
            contracts.GLCode = contractsVm.GLCode;
            clientDbContext.Contracts.Add(contracts);
            clientDbContext.SaveChanges();
            return Json(new { contracts, succeed = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getPositionDetails(int positionId, decimal? salary, int? RateType, decimal? PayRate, decimal? HoursPerPayPeriod, DateTime? EffectiveDate, int? payFreq)
        {
            TempData["Salary"] = salary;
            string connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString);
            var personID = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID));
            var employeeid = clientDbContext.Employees.Where(x => x.PersonId == personID).Select(x => x.EmployeeId).FirstOrDefault();
            var E_PositionId = clientDbContext.E_Positions.Where(x => x.PositionId == positionId && x.EmployeeId == employeeid).OrderByDescending(m => m.E_PositionId).Select(r => r.E_PositionId).FirstOrDefault();
            var companycodeid = clientDbContext.Employees.Where(x => x.EmployeeId == employeeid).Select(x => x.CompanyCodeId).FirstOrDefault();
            bool isnew = Convert.ToBoolean(Session["IsnewRecords"]);
            Session["positionId"] = positionId;
            if (isnew == true)
            {
                var EmployeeId = clientDbContext.Employees
                   .Where(x => x.PersonId == personID && x.CompanyCodeId == companycodeid).Select(a => a.EmployeeId).SingleOrDefault();
                var isRecordsExists = clientDbContext.E_Positions.Where(x => x.E_PositionId == E_PositionId
                                                                        && x.actualEndDate == null
                                                                        && x.EmployeeId == employeeid)
                                                                        .Select(a => a.PositionId).SingleOrDefault();
                if (isRecordsExists != 0)
                {
                    return Json(new { succeed = false, Message = "The employee position record  exists for the selected position." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    SavePositionSalaySource1(positionId, salary, RateType, PayRate, HoursPerPayPeriod, EffectiveDate, payFreq);
                }
            }
            GetRateTypesList();
            return View(GetE_PositionsRecord(Convert.ToInt32(Session["E_PositionId"]), personID));
        }
        public E_PositioVm GetPositiondetail(int PositionId, decimal salary)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            E_PositioVm e_PositionVm = new E_PositioVm();
            var businesslevelNbr = clientDbContext.Positions.Where(m => m.PositionId == PositionId).Select(m => m.BusinessLevelNbr).FirstOrDefault();
            var businesslevel = clientDbContext.PositionBusinessLevels.Where(n => n.BusinessLevelNbr == businesslevelNbr).Select(s => s.BusinessLevelTitle).FirstOrDefault();
            var jobid = clientDbContext.Positions.Where(n => n.PositionId == PositionId).Select(m => m.JobId).FirstOrDefault();
            var job = clientDbContext.Jobs.Where(n => n.JobId == jobid).Select(s => s.JobDescription).FirstOrDefault();
            var primaryposition = clientDbContext.E_Positions.Where(m => m.PositionId == PositionId).Select(s => s.PrimaryPosition).FirstOrDefault();
            var positiongradeid = clientDbContext.E_Positions.Where(n => n.PositionId == PositionId).Select(m => m.PositionGradeID).FirstOrDefault();
            if (PositionId != 0)
            {
                e_PositionVm = clientDbContext.Positions
                   .Include("Employee.Person")
                   .Include("DdlPayFrequencies.Description")
                   .Include("DdlRateTypes.Description")
                   .Include("DdlPositionCategory.Description")
                   .Include("DdlPositionGrade.Description")
                   .Include("DdlPositionTypes.Description")
                   .Where(x => x.PositionId == PositionId)
                   .Select(x => new E_PositioVm
                   {
                       PositionId = x.PositionId,
                       PositionDescription = x.PositionDescription,
                       BusinessLevel = businesslevel,
                       Job = job,
                       FTE = x.FTE,
                       PositionCategoryID = x.PositionCategoryID,
                       PositionCategoryDesc = x.DdlPositionCategories.description,
                       PositionTypeID = x.PositionTypeID,
                       PositionTypeDesc = x.DdlPositionType.description,
                       PositionGradeID = x.PositionGradeID,
                       PositionGradeDesc = x.DdlPositionGrades.Description,
                       PrimaryPosition = primaryposition,
                       StartDate = (DateTime?)null,  //x.StartDate,
                       projectedEndDate = (DateTime?)null,   //x.ProjectEndDate,
                       actualEndDate = (DateTime?)null,    //x.ActualEnddate,
                       salary = salary,
                       EnteredBy = x.EnteredBy,
                       EnteredDate = x.EnteredDate,
                   })
                   .FirstOrDefault();
            }
            if (e_PositionVm != null)
            {
                e_PositionVm.PayFrequencyDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPayFrequencyList());
                e_PositionVm.RateTypeDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetRateTypeList());
                e_PositionVm.PositionCategoryDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPositionCategoryList());
                e_PositionVm.PositionGradeDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetSalaryGradeList());
                e_PositionVm.PositionTypeDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPositionTypeList());
                e_PositionVm.EmployeeTypeDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetEmployeeTypeList());
            }


            var E_PositionId = clientDbContext.E_Positions.Where(n => n.PositionId == PositionId).Select(m => m.E_PositionId).FirstOrDefault();
            ViewBag.ePositionId = E_PositionId;
            ViewBag.PositionTypes = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPositionTypeList());
            ViewBag.PositionCategory = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPositionCategoryList());
            ViewBag.PositionGrade = JsonConvert.DeserializeObject<List<DropDownModel>>(GetSalaryGradeList());
            ViewBag.PayFrequency = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPayFrequencyList());
            return e_PositionVm;
        }
        public string GetPositionCategoryList()
        {
            var _list = _epositionRepository.GetPositionCategoryList().CleanUp();
            return JsonConvert.SerializeObject(_list);
        }
        public string GetPositionGradeList()
        {
            var _list = _epositionRepository.GetPositionGradeList().CleanUp();
            return JsonConvert.SerializeObject(_list);
        }
        public string GetPositionTypeList()
        {

            var _list = _epositionRepository.GetPositionTypeList().CleanUp();
            return JsonConvert.SerializeObject(_list);
        }
        [HttpPost]
        public ActionResult SavepositionSalaySource(int E_PositionId, int RateType, decimal? PayRate, decimal? HoursPerPayPeriod, DateTime? EffectiveDate)
        {
            string returnmsg = "";
            string connString = User.Identity.GetClientConnectionString();
            try
            {
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    var payfrequencycode = new DdlPayFrequency();
                    var payfrequencyid = clientDbContext.E_Positions.Where(x => x.E_PositionId == E_PositionId).FirstOrDefault();
                    var payCode = "";
                    if (payfrequencyid != null)
                    {
                        payfrequencycode = clientDbContext.DdlPayFrequencies.Where(x => x.PayFrequencyId == payfrequencyid.PayFrequencyId).FirstOrDefault();
                    }
                    if (payfrequencycode != null)
                    {
                        payCode = payfrequencycode.Code.ToLower();
                    }
                    else
                    {
                        payCode = "";
                    }
                    var lasteffectivedate = clientDbContext.E_PositionSalaryHistories.Where(m => m.E_PositionId == E_PositionId && m.EffectiveDate != null).OrderByDescending(m => m.EffectiveDate).FirstOrDefault();
                    var Payrate = clientDbContext.E_PositionSalaryHistories.OrderByDescending(x => x.E_PositionSalaryHistoryId).Where(x => (x.E_PositionId == E_PositionId && x.EffectiveDate != null)).Select(ai => ai.PayRate).FirstOrDefault();
                    var RateTypeDescription = clientDbContext.DdlRateTypes
                                                             .Where(m => m.RateTypeId == RateType)
                                                             .Select(d => d.Description).FirstOrDefault();
                    var ePositionsEndDate = clientDbContext.E_Positions.Where(x => x.E_PositionId == E_PositionId).Select(x => x.actualEndDate).FirstOrDefault();
                    if (RateTypeDescription == "H")
                    {
                        var newitem = new E_PositionSalaryHistories
                        {
                            E_PositionId = E_PositionId,
                            RateTypeId = RateType,
                            PayRate = PayRate,
                            HoursPerPayPeriod = HoursPerPayPeriod,
                            AnnualSalary =
                                               payCode.ToLower() == "bw" ? 26 * HoursPerPayPeriod * PayRate :
                                               payCode.ToLower() == "sm" ? 24 * HoursPerPayPeriod * PayRate :
                                               payCode.ToLower() == "m" ? 12 * HoursPerPayPeriod * PayRate :
                                               payCode.ToLower() == "w" ? 52 * HoursPerPayPeriod * PayRate : 0,
                            EffectiveDate = EffectiveDate,
                            IsDeleted = false,
                            UserId = User.Identity.Name,
                            LastModifiedDate = DateTime.Now,
                            EndDate = ePositionsEndDate
                        };
                        if (lasteffectivedate != null)
                        {
                            if (EffectiveDate < Convert.ToDateTime(lasteffectivedate.EffectiveDate))
                            {
                                return Json(new { succeed = false, Message = "Effective Date must be in the future" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        clientDbContext.E_PositionSalaryHistories.Add(newitem);
                        clientDbContext.SaveChanges();
                        clientDbContext.Database.ExecuteSqlCommand("Update [E_PositionSalaryHistories] SET EndDate = @EndDate WHERE E_PositionSalaryHistoryId = @E_PositionSalaryHistoryId", new SqlParameter("@EndDate", EffectiveDate.Value.AddDays(-1)), new SqlParameter("@E_PositionSalaryHistoryId", lasteffectivedate.E_PositionSalaryHistoryId));
                    }
                    else
                    {
                        var newitem = new E_PositionSalaryHistories
                        {
                            E_PositionId = E_PositionId,
                            RateTypeId = RateType,
                            PayRate = PayRate,
                            HoursPerPayPeriod = HoursPerPayPeriod,
                            EffectiveDate = EffectiveDate,
                            AnnualSalary =
                            payCode.ToLower() == "bw" ? 26 * PayRate :
                            payCode.ToLower() == "sm" ? 24 * PayRate :
                            payCode.ToLower() == "m" ? 12 * PayRate :
                            payCode.ToLower() == "w" ? 52 * PayRate : 0,
                            IsDeleted = false,
                            UserId = User.Identity.Name,
                            LastModifiedDate = DateTime.Now,
                            EndDate = ePositionsEndDate
                        };
                        if (lasteffectivedate != null)
                        {
                            if (EffectiveDate < Convert.ToDateTime(lasteffectivedate.EffectiveDate))
                            {
                                return Json(new { succeed = false, Message = "Effective Date must be in the future" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        clientDbContext.E_PositionSalaryHistories.Add(newitem);
                        clientDbContext.SaveChanges();
                        clientDbContext.Database.ExecuteSqlCommand("Update [E_PositionSalaryHistories] SET EndDate = @EndDate WHERE E_PositionSalaryHistoryId = @E_PositionSalaryHistoryId", new SqlParameter("@EndDate", EffectiveDate.Value.AddDays(-1)), new SqlParameter("@E_PositionSalaryHistoryId", lasteffectivedate.E_PositionSalaryHistoryId));
                    }
                }
            }
            catch (System.Exception ex) { returnmsg = ex.ToString(); }
            return RedirectToAction("EPositionSalaryHistoryList_Read", "E_Positions", new { e_PositionId = E_PositionId, rate = RateType });
        }
        public PartialViewResult EditePositionSalaryDetail(int ePositionSalHistoryId, int? RateTypeId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            {
                var e_PositionSalaryHistoriesList = clientDbContext.E_PositionSalaryHistories
                    .Include("E_Positions.DdlPayFrequency")
                    .OrderByDescending(e => e.EffectiveDate)
                    .Where(x => x.E_PositionSalaryHistoryId == ePositionSalHistoryId).ToList()
                    .Select(e => new E_PositionSalaryHistorVm
                    {
                        RateTypeId = e.RateTypeId,
                        PayRate = e.PayRate,
                        HoursPerPayPeriod = e.HoursPerPayPeriod,
                        AnnualSalary = (e.E_Positions.DdlPayFrequency.Description.ToLower().StartsWith("bi", StringComparison.InvariantCultureIgnoreCase) ? (26 * e.HoursPerPayPeriod * e.PayRate) :
                                (e.E_Positions.DdlPayFrequency.Description.ToLower().StartsWith("semi", StringComparison.InvariantCultureIgnoreCase) ? (24 * e.HoursPerPayPeriod * e.PayRate) :
                                    (e.E_Positions.DdlPayFrequency.Description.ToLower().StartsWith("mon", StringComparison.InvariantCultureIgnoreCase) ? (12 * e.HoursPerPayPeriod * e.PayRate) :
                                        (e.E_Positions.DdlPayFrequency.Description.ToLower().StartsWith("week", StringComparison.InvariantCultureIgnoreCase) ? (52 * e.HoursPerPayPeriod * e.PayRate) :
                                        (e.E_Positions.DdlPayFrequency.Description.ToLower().StartsWith("year", StringComparison.InvariantCultureIgnoreCase) ? (1 * e.HoursPerPayPeriod * e.PayRate) :
                                      (e.E_Positions.DdlPayFrequency.Description.ToLower().StartsWith("quar", StringComparison.InvariantCultureIgnoreCase) ? (4 * e.HoursPerPayPeriod * e.PayRate) : 0)
                                    )
                                   )
                                 )
                                )
                             ),
                        EffectiveDate = e.EffectiveDate,
                        EndDate = e.EndDate,
                    }).FirstOrDefault();
                TempData["SalHistoryId"] = ePositionSalHistoryId;
                ViewBag.RateType = RateTypeId;
                GetRateTypesList();
                return PartialView("EditPositionSalaryHistory", e_PositionSalaryHistoriesList);
            }
        }

        public PartialViewResult EditContractDetail(int Id)
        {
            GetEmpTypeList();
            GetMemoCode1List();
            GetMemoCode2List();
            GetDepartmentList();
            GetSemisterList();
            GetEarningCodesList();
            GetGLCodesList();

            int e_PositionId = Convert.ToInt32(Session["ePositionId"]);
            decimal Remainingamount = 0;
            decimal AmountsInDollar = 0;
            int numberofpayperiods = 0;
            decimal Adjlimit = 0;
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            {
                var employeeId = clientDbContext.E_Positions.Where(x => x.E_PositionId == e_PositionId).Select(m => m.EmployeeId).FirstOrDefault();
                var rateValue = clientDbContext.Employees.Where(x => x.EmployeeId == employeeId).Select(m => m.Rate).FirstOrDefault();
                var amount = clientDbContext.Employees.Where(x => x.EmployeeId == employeeId).Select(m => m.Amount).FirstOrDefault();
                ViewBag.RegPayRateAmt = rateValue;
                var employee = clientDbContext.Employees.Where(x => x.EmployeeId == employeeId).FirstOrDefault();


                var Contracts = clientDbContext.Contracts.Where(x => x.Id == Id)
                          .OrderBy(x => x.Id)
                          .Select(x => new PayPeriodVM { StartDate = x.StartDate.Value, EndDate = x.EndDate.Value, Amounts = x.AmountsInDollar, numberofpayperiods = x.PayPeriod.Value, ADJlimits = x.ADJlimits, EarningCodes = x.EarningCodes })
                          .FirstOrDefault();
                //var countpayperiodsExport = getPayperiodsList(Contracts.StartDate, Contracts.EndDate);
                var countpayperiodsExport = GetContractExportCount(Id);
                if (countpayperiodsExport > 0)
                {
                    AmountsInDollar = Contracts.Amounts == 0 ? 0 : Convert.ToDecimal(Contracts.Amounts);
                    numberofpayperiods = Contracts.numberofpayperiods == 0 ? 0 : Convert.ToInt32(Contracts.numberofpayperiods);
                    Adjlimit = Contracts.ADJlimits == 0 ? 0 : Convert.ToInt32(Contracts.ADJlimits);
                    if (Adjlimit == 0)
                    {
                        Remainingamount = AmountsInDollar - (AmountsInDollar / numberofpayperiods) * countpayperiodsExport;
                    }
                    else
                    {
                        //if adjustment input non zero
                        Remainingamount = AmountsInDollar - (AmountsInDollar / numberofpayperiods) * countpayperiodsExport;
                        Remainingamount = Remainingamount == 0 ? 0.00m : Remainingamount - Adjlimit;
                    }
                }
                decimal remainamount = Remainingamount;
                Session["Remainingamount"] = remainamount;
                var contractsList = clientDbContext.Contracts
                .Where(x => x.Id == Id).ToList()
                .Select(e => new ContractsVm
                {
                    EarningsCodeId = e.EarningsCodeId,
                    MemoCode1 = e.MemoCode1,
                    MemoCode1Amount = e.MemoCode1Amount,
                    MemoCode2 = e.MemoCode2,
                    MemoCode2Amount = e.MemoCode2Amount,
                    PayPeriod = e.PayPeriod,
                    Status1FlagCode = e.Status1FlagCode,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Amounts = countpayperiodsExport == 0 ? e.Amounts : Math.Round(Remainingamount, 2),
                    Department = e.Department,
                    AmountsInDollar = e.AmountsInDollar,
                    Job = e.Job,
                    AddNewContract = e.AddNewContract,
                    Notes = e.Notes,
                    SemisterId = e.SemisterId,
                    GLCodeId = e.GLCodeId,
                    ADJlimits = e.ADJlimits,
                    EpositionId = e_PositionId,
                    EarningCodes = e.EarningCodes,
                    GLCode = e.GLCode
                }).FirstOrDefault();
                if (contractsList != null && employee != null)
                {
                    // var originalEarningCodeId = contractsList.EarningsCodeId;
                    //contractsList.EarningsCodeId = employee.EarningsCodeId;
                    //contractsList.AmountsInDollar = employee.Amount;
                    //var earning = clientDbContext.EarningsCodes.Where(x => x.EarningsCodeId == employee.EarningsCodeId).Select(r => r.TreatyCode).FirstOrDefault();
                    //if (employee.EarningsCodeId != null) { contractsList.EarningsCode = (earning) ? "Treaty" : "Non Treaty"; }
                    //contractsList.EarningsCodeId = originalEarningCodeId;
                }
                return PartialView("EditContracts", contractsList);

            }
        }

        [HttpPost]
        public ActionResult UpdateContracts(int Id, ContractsVm contractsVm)
        {
            string connString = User.Identity.GetClientConnectionString();
            int epositionId = Convert.ToInt32(Session["ePositionId"]);
            decimal remaingbal = Convert.ToDecimal(Session["Remainingamount"]);
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var objContracts = (from x in clientDbContext.Contracts where x.Id == Id select x).FirstOrDefault();
                objContracts.EarningsCodeId = contractsVm.EarningsCodeId;
                objContracts.MemoCode1 = contractsVm.MemoCode1;
                objContracts.MemoCode1Amount = contractsVm.MemoCode1Amount;
                objContracts.MemoCode2 = contractsVm.MemoCode2;
                objContracts.MemoCode2Amount = contractsVm.MemoCode2Amount;
                objContracts.PayPeriod = contractsVm.PayPeriod;
                objContracts.SemisterId = contractsVm.SemisterId;
                objContracts.GLCodeId = contractsVm.GLCodeId;
                objContracts.GLCode = contractsVm.GLCode;
                objContracts.EarningCodes = contractsVm.EarningCodes;
                if (Session["EmployeeTypeId"] == null || Convert.ToString(Session["EmployeeTypeId"]) == "0")
                {
                    objContracts.Status1FlagCode = null;
                }
                else
                {
                    objContracts.Status1FlagCode = Convert.ToInt32(Session["EmployeeTypeId"]);
                }
                objContracts.StartDate = contractsVm.StartDate;
                objContracts.EndDate = contractsVm.EndDate;
                objContracts.Amounts = remaingbal;
                objContracts.Department = contractsVm.Department;
                objContracts.AmountsInDollar = contractsVm.AmountsInDollar;
                objContracts.ADJlimits = contractsVm.ADJlimits;
                clientDbContext.SaveChanges();
            }
            return RedirectToAction("ContractList_Read");
        }

        [HttpPost]
        public ActionResult updatePositionSalaySource(int E_PositionId, int RateType, decimal? PayRate, decimal? HoursPerPayPeriod, DateTime? EffectiveDate)
        {
            SalaryHistoryInsertUpdate(E_PositionId, RateType, PayRate, HoursPerPayPeriod, EffectiveDate);
            return RedirectToAction("EPositionSalaryHistoryList_Read", new { e_PositionId = E_PositionId });
        }

        private void SalaryHistoryInsertUpdate(int nEPositionID, int rateId, decimal? dPayRate, decimal? HoursPerPayPeriod, DateTime? rateEffectiveDate)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var rateType = clientDbContext.DdlRateTypes
                                                             .Where(m => m.RateTypeId == rateId)
                                                             .Select(d => d.Description).FirstOrDefault();
            var drEffective = (from c in clientDbContext.E_PositionSalaryHistories
                               join ep in clientDbContext.E_Positions on c.E_PositionId equals ep.E_PositionId
                               join po in clientDbContext.Positions on ep.PositionId equals po.PositionId
                               where c.E_PositionId == nEPositionID && c.PayRate == dPayRate
                               select c.E_PositionSalaryHistoryId).ToList();
            if (drEffective.Count() == 0)
            {
                var lasteffectivedate = clientDbContext.E_PositionSalaryHistories.Where(m => m.E_PositionId == nEPositionID).OrderByDescending(m => m.E_PositionSalaryHistoryId).FirstOrDefault();
                var ePositionsSalHistory = new E_PositionSalaryHistories();
                ePositionsSalHistory.E_PositionId = nEPositionID;
                ePositionsSalHistory.EffectiveDate = rateEffectiveDate;
                ePositionsSalHistory.RateTypeId = rateId;
                ePositionsSalHistory.EnteredBy = User.Identity.Name;
                ePositionsSalHistory.EnteredDate = DateTime.Now;
                ePositionsSalHistory.HoursPerPayPeriod = 40.0m;
                ePositionsSalHistory.IsDeleted = false;
                ePositionsSalHistory.PayRate = dPayRate;
                if (rateType.ToUpper().Trim() == "H")
                {
                    ePositionsSalHistory.AnnualSalary = 26 * 40.0m * dPayRate;
                }
                else if (rateType.ToUpper().Trim() == "S")
                {
                    ePositionsSalHistory.AnnualSalary = 26 * dPayRate;
                }
                clientDbContext.E_PositionSalaryHistories.Add(ePositionsSalHistory);
                clientDbContext.SaveChanges();
                if (lasteffectivedate != null)
                {
                    var salaryEndDate = rateEffectiveDate.Value.AddDays(-1);
                    var ePosSalHisId = lasteffectivedate.E_PositionSalaryHistoryId;
                    clientDbContext.Database.ExecuteSqlCommand("Update [E_PositionSalaryHistories] SET EndDate = @EndDate WHERE E_PositionSalaryHistoryId = @E_PositionSalaryHistoryId", new SqlParameter("@EndDate", salaryEndDate), new SqlParameter("@E_PositionSalaryHistoryId", ePosSalHisId));
                    clientDbContext.SaveChanges();
                }
            }
            else
            {
                var drEffectiveWithRate = (from c in clientDbContext.E_PositionSalaryHistories
                                           join ep in clientDbContext.E_Positions on c.E_PositionId equals ep.E_PositionId
                                           join po in clientDbContext.Positions on ep.PositionId equals po.PositionId
                                           where c.E_PositionId == nEPositionID && c.PayRate == dPayRate && c.EffectiveDate == rateEffectiveDate
                                           select c.E_PositionSalaryHistoryId).ToList();
                if (drEffectiveWithRate.Count == 0)
                {
                    var lasteffectivedatenew = clientDbContext.E_PositionSalaryHistories.Where(m => m.E_PositionId == nEPositionID).OrderByDescending(m => m.E_PositionSalaryHistoryId).FirstOrDefault();
                    var ePositionsSalHistory = new E_PositionSalaryHistories();
                    ePositionsSalHistory.E_PositionId = nEPositionID;
                    ePositionsSalHistory.EffectiveDate = rateEffectiveDate;
                    ePositionsSalHistory.RateTypeId = rateId;
                    ePositionsSalHistory.EnteredBy = User.Identity.Name;
                    ePositionsSalHistory.EnteredDate = DateTime.Now;
                    ePositionsSalHistory.HoursPerPayPeriod = 40.0m;
                    ePositionsSalHistory.IsDeleted = false;
                    ePositionsSalHistory.PayRate = dPayRate;
                    if (rateType.ToUpper().Trim() == "H")
                    {
                        ePositionsSalHistory.AnnualSalary = 26 * 40.0m * dPayRate;
                    }
                    else if (rateType.ToUpper().Trim() == "S")
                    {
                        ePositionsSalHistory.AnnualSalary = 26 * dPayRate;
                    }
                    clientDbContext.E_PositionSalaryHistories.Add(ePositionsSalHistory);
                    clientDbContext.SaveChanges();
                    if (lasteffectivedatenew != null)
                    {
                        var salaryEndDate = DateTime.Now.AddDays(-1);
                        var ePosSalHisId = lasteffectivedatenew.E_PositionSalaryHistoryId;
                        clientDbContext.Database.ExecuteSqlCommand("Update [E_PositionSalaryHistories] SET EndDate = @EndDate WHERE E_PositionSalaryHistoryId = @E_PositionSalaryHistoryId", new SqlParameter("@EndDate", salaryEndDate), new SqlParameter("@E_PositionSalaryHistoryId", ePosSalHisId));
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    var EPosSalHistoryId = Convert.ToInt32(drEffectiveWithRate[0]);
                    var objEPosSalHistory = (from x in clientDbContext.E_PositionSalaryHistories where x.E_PositionSalaryHistoryId == EPosSalHistoryId select x).FirstOrDefault();
                    objEPosSalHistory.E_PositionId = nEPositionID;
                    objEPosSalHistory.EffectiveDate = rateEffectiveDate;
                    objEPosSalHistory.RateTypeId = rateId;
                    objEPosSalHistory.ModifiedBy = User.Identity.Name;
                    objEPosSalHistory.ModifiedDate = DateTime.Now;
                    objEPosSalHistory.HoursPerPayPeriod = 40.0m;
                    objEPosSalHistory.IsDeleted = false;
                    objEPosSalHistory.PayRate = dPayRate;
                    if (rateType.ToUpper().Trim() == "H")
                    {
                        objEPosSalHistory.AnnualSalary = 26 * 40.0m * dPayRate;
                    }
                    else if (rateType.ToUpper().Trim() == "S")
                    {
                        objEPosSalHistory.AnnualSalary = 26 * dPayRate;
                    }
                    clientDbContext.SaveChanges();
                }
            }
        }

        private string GetSalaryGradeList()
        {
            var connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString);
            var content = (from ss in clientDbContext.PositionSalaryGradeSourceHistories
                           join sg in clientDbContext.DdlSalaryGrades on
                               ss.SalaryGradeID equals sg.SalaryGradeID
                           where sg.SalaryGradeID == ss.SalaryGradeID
                           select new
                           {
                               sg.description,
                               sg.SalaryGradeID
                           }).Distinct().ToList();
            var salarydescription = new List<DropDownModel>();
            salarydescription.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "--Select--" });
            if (content.Any())
            {
                salarydescription = content.Select(x => new DropDownModel
                {
                    keyvalue = x.SalaryGradeID.ToString(),
                    keydescription = x.description
                }).OrderBy(k => k.keydescription).ToList();
            }
            salarydescription.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "--Select--" });
            return JsonConvert.SerializeObject(salarydescription);
        }

        public int GetEmployeeId(int empNo)
        {
            var connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString);
            var personID = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID));
            var EmpId = clientDbContext.Employees.Where(m => m.EmploymentNumber == empNo && m.PersonId == personID).Select(m => m.EmployeeId).FirstOrDefault();
            return EmpId;
        }

        public void SavePositionSalaySource1(int ePositionId, decimal? salary, int? RateType, decimal? PayRate, decimal? HoursPerPayPeriod, DateTime? EffectiveDate, int? payFreq)
        {
            string returnmsg = "";
            string connString = User.Identity.GetClientConnectionString();
            try
            {
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    var personID = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID));
                    var number = Convert.ToInt32(Session["EmplNo"]);
                    var EmpId = clientDbContext.Employees.Where(m => m.PersonId == personID && m.EmploymentNumber == number).Select(m => m.EmployeeId).FirstOrDefault();
                    var ePositions = new E_Positions();
                    ePositions.EmployeeId = EmpId;
                    ePositions.PositionId = ePositionId;
                    ePositions.RateTypeId = RateType;
                    ePositions.PayFrequencyId = payFreq;
                    ePositions.salary = salary;
                    ePositions.PrimaryPosition = false;
                    ePositions.IsDeleted = false;
                    ePositions.UserId = User.Identity.Name;
                    ePositions.EnteredBy = User.Identity.Name;
                    ePositions.EnteredDate = DateTime.Now;
                    clientDbContext.E_Positions.Add(ePositions);
                    clientDbContext.SaveChanges();
                    Session["E_PositionId"] = Convert.ToString(ePositions.E_PositionId);
                    int E_PositionId = Convert.ToInt32(Session["E_PositionId"]);
                    var payfrequencycode = clientDbContext.DdlPayFrequencies.Where(x => x.PayFrequencyId == payFreq).Select(r => r.Code).FirstOrDefault();
                    //comment by hitesh 
                    // var E_PositionId = clientDbContext.E_Positions.Where(x => x.PositionId == ePositionId).OrderByDescending(m => m.E_PositionId).Select(r => r.E_PositionId).FirstOrDefault();
                    string RateTypeCode = clientDbContext.DdlRateTypes.Where(x => x.RateTypeId == RateType).Select(r => r.Code).FirstOrDefault();

                    decimal? AnnualSalary = 0;

                    if (RateTypeCode == "H" || RateTypeCode == "D")
                    {
                        AnnualSalary = payfrequencycode.ToLower() == "bw" ? 26 * HoursPerPayPeriod * PayRate :
                                           payfrequencycode.ToLower() == "sm" ? 24 * HoursPerPayPeriod * PayRate :
                                           payfrequencycode.ToLower() == "m" ? 12 * HoursPerPayPeriod * PayRate :
                                           payfrequencycode.ToLower() == "w" ? 52 * HoursPerPayPeriod * PayRate : 0;
                    }
                    else
                    {
                        AnnualSalary = payfrequencycode.ToLower() == "bw" ? 26 * PayRate :
                                          payfrequencycode.ToLower() == "sm" ? 24 * PayRate :
                                          payfrequencycode.ToLower() == "m" ? 12 * PayRate :
                                          payfrequencycode.ToLower() == "w" ? 52 * PayRate : 0;
                    }
                    var newitem = new E_PositionSalaryHistories
                    {
                        E_PositionId = E_PositionId,
                        RateTypeId = RateType,
                        PayRate = PayRate,
                        HoursPerPayPeriod = HoursPerPayPeriod,
                        EffectiveDate = EffectiveDate,
                        AnnualSalary = AnnualSalary,
                        IsDeleted = false,
                        UserId = User.Identity.Name,
                        LastModifiedDate = DateTime.Now
                    };
                    clientDbContext.E_PositionSalaryHistories.Add(newitem);
                    clientDbContext.SaveChanges();
                }
            }
            catch (System.Exception ex) { returnmsg = ex.ToString(); }
        }

        public JsonResult GetSlots(int PositionId)
        {
            var connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString);
            int slotsfilled = clientDbContext.E_Positions.Where(x => x.PositionId == PositionId).ToList().Count();
            var totalslots = clientDbContext.Positions.Where(m => m.PositionId == PositionId).Select(m => m.TotalSlots).FirstOrDefault();
            return Json(new { slotsfilled, totalslots }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmpStatus(int EmpId, int personid, int EmpNumber)
        {

            var connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString);
            var EmpData = clientDbContext.Employees.Where(x => x.PersonId == personid && x.TerminationDate == null).FirstOrDefault();
            var Empdatasatusid = clientDbContext.Employees.Where(x => x.PersonId == personid && x.EmploymentNumber == EmpId).FirstOrDefault();
            string Empstatus = "";
            if (Empdatasatusid != null && Empdatasatusid.EmploymentStatusId != null)
            {
                Empstatus = clientDbContext.DdlEmploymentStatuses.Where(x => x.EmploymentStatusId == Empdatasatusid.EmploymentStatusId).Select(d => d.Description).FirstOrDefault();

            }
            return Json(new { Empstatus }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ContractDeleteAjax(int Id)
        {
            try
            {
                _epositionRepository.DeleteContract(Id);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
            // return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);
            return RedirectToAction("ContractList_Read");
        }


        public PartialViewResult AddContractDetail()
        {
            GetEmpTypeList();
            GetMemoCode1List();
            GetMemoCode2List();
            GetDepartmentList();
            GetSemisterList();
            GetGLCodesList();
            GetEarningCodesList();
            int e_PositionId = Convert.ToInt32(Session["ePositionId"]);
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            {
                var employeeId = clientDbContext.E_Positions.Where(x => x.E_PositionId == e_PositionId).Select(m => m.EmployeeId).FirstOrDefault();
                var employee = clientDbContext.Employees.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
                ContractsVm contractVm = new ContractsVm();

                //if (employee != null)
                //{
                //    contractVm.EarningsCodeId = employee.EarningsCodeId;
                //    contractVm.AmountsInDollar = employee.Amount;
                //    var earning = clientDbContext.EarningsCodes.Where(x => x.EarningsCodeId == employee.EarningsCodeId).Select(r => r.TreatyCode).FirstOrDefault();
                //    if (employee.EarningsCodeId != null) { contractVm.EarningsCode = (earning) ? "Treaty" : "Non Treaty"; }
                //    contractVm.EarningsCodeId = null;
                //}
                return PartialView("AddContracts", contractVm);
            }
        }

        public int getPayperiodscount(DateTime startDate, DateTime endDate)
        {
            string connString = User.Identity.GetClientConnectionString();
            int? paygroupid = Convert.ToInt32(Session["paygroupId"]);
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var payperiodcount = clientDbContext.Database.SqlQuery<PayPeriodVM>(
                                                "exec dbo.spGetPayPeriodDates @StartDate,@EndDate",
                                                 new SqlParameter("@StartDate", startDate),
                                                 new SqlParameter("@EndDate", endDate)
                ).ToList();
            var count = payperiodcount.Where(x => x.PayGroupCode == paygroupid).Count();
            return count;
        }

        public int getPayperiodsList(DateTime startDate, DateTime endDate)
        {
            string connString = User.Identity.GetClientConnectionString();
            int? paygroupid = Convert.ToInt32(Session["paygroupId"]);
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var payperiodcount = clientDbContext.Database.SqlQuery<PayPeriodVM>(
                                                "exec dbo.spGetPayPeriodDates @StartDate,@EndDate",
                                                 new SqlParameter("@StartDate", startDate),
                                                 new SqlParameter("@EndDate", endDate)
                ).ToList();
            var listpayperiodcount = payperiodcount.Where(x => x.PayGroupCode == paygroupid && x.Isexported == true).Count();
            return listpayperiodcount;
        }


        public int GetContractExportCount(int Id)
        {
            string connString = User.Identity.GetClientConnectionString();
            int? paygroupid = Convert.ToInt32(Session["paygroupId"]);
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var payperiodcount = clientDbContext.Database.SqlQuery<PayPeriodVM>(
                                                "exec dbo.spGetContractTrackingStatus @Id",
                                                 new SqlParameter("@Id", Id)
                ).ToList();
            var listpayperiodcount = payperiodcount.Where(x => x.PayGroupCode == paygroupid && x.Isexported == true).Count();
            return listpayperiodcount;
        }


        #region NonStandardSalaryPayment - Third Tab

        //Read Existing Salary History inforamtion same as First Tab. 
        public ActionResult NonStdSalaryGrid_Read([DataSourceRequest]DataSourceRequest request, int e_PositionId)
        {
            if (e_PositionId < 1)
                return Json("History not exist", JsonRequestBehavior.AllowGet);

            List<E_PositioVm> e_PositionSalaryHistoriesList = new List<E_PositioVm>();
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            {
                e_PositionSalaryHistoriesList = (from eps in clientDbContext.E_PositionSalaryHistories
                                                 join drt in clientDbContext.DdlRateTypes on eps.RateTypeId equals drt.RateTypeId
                                                 orderby eps.EffectiveDate descending
                                                 where eps.E_PositionId == e_PositionId && eps.EffectiveDate != null
                                                 select new E_PositioVm
                                                 {
                                                     E_PositionSalaryHistoryId = eps.E_PositionSalaryHistoryId,
                                                     RateTypeId = drt.RateTypeId,
                                                     RateTypeDescription = drt.Description == "S" ? "Salary" : drt.Description == "H" ? "Hourly" : "",                                                     // RateTypeDescription = e.RateTypeId == null ? "Select" : e.RateTypeId == 1 ? "Salary" : e.RateTypeId == 2 ? "Hourly" : "",
                                                     PayRate = eps.PayRate,
                                                     HoursPerPayPeriod = eps.HoursPerPayPeriod,
                                                     AnnualSalary = eps.AnnualSalary,
                                                     EffectiveDate = eps.EffectiveDate,
                                                     EndDate = eps.EndDate
                                                 }).ToList();
                return Json(e_PositionSalaryHistoriesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Returns the Amount and Rate based on the Current Position Salary History Rate.
        /// Binds the Non Standard Salary Payment Grid.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="e_PositionId"></param>
        /// <returns></returns>
        //STATIC VALUES FOR FIRST RECORD AND SECOND RECORD
        /*
        public ActionResult NonStandardSalaryPayment_Read([DataSourceRequest]DataSourceRequest request, int e_PositionId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            //var emptypeid = clientDbContext.E_Positions.Where(x => x.E_PositionId == e_PositionId).Select(m => m.EmployeeTypeId).FirstOrDefault();
            //var emptypedesc = clientDbContext.DdlEmployeeTypes.Where(x => x.EmployeeTypeId == emptypeid).Select(m => m.Description).FirstOrDefault();
            var emptypedesc = TempData["EmployeeTypeDescription"];
            var employeeid = clientDbContext.E_Positions.Where(ep => ep.E_PositionId == e_PositionId).Select(e => e.EmployeeId).FirstOrDefault();
            var fileno = clientDbContext.Employees.Where(x => x.EmployeeId == employeeid).Select(m => m.FileNumber).FirstOrDefault();

            //Returns the employee active current salary history Pay Rate
            var employeecurrentsalaryhistoryrate = (from eps in clientDbContext.E_PositionSalaryHistories
                                                    join drt in clientDbContext.DdlRateTypes on eps.RateTypeId equals drt.RateTypeId
                                                    orderby eps.EffectiveDate descending
                                                    where eps.E_PositionId == e_PositionId && eps.EffectiveDate != null && eps.EndDate == null
                                                    select eps.PayRate).FirstOrDefault();

            decimal? employeerate = Convert.ToDecimal(employeecurrentsalaryhistoryrate);

            List<NonStandardSalaryPayment> nonstandardSalaryPaymentList = new List<NonStandardSalaryPayment>();
            for (int i = 1; i <= 2; i++)
            {
                switch (emptypedesc)
                {
                    case "9 x 12":
                        var nonstandardSalaryPayment = new NonStandardSalaryPayment()
                        {
                            SalaryPaymentId = i
                        };
                        if (i == 1)
                        {
                            nonstandardSalaryPayment.SalaryPaymentCode = "PayPeriod 1-18";
                            nonstandardSalaryPayment.SALMemoCode = "SAL";
                            nonstandardSalaryPayment.SALMemoAmount = ((decimal)26 / (decimal)18 * employeerate);
                            nonstandardSalaryPayment.DPAMemoCode = "DPA";
                            nonstandardSalaryPayment.DPAMemoAmount = nonstandardSalaryPayment.SALMemoAmount - employeerate;
                            nonstandardSalaryPayment.CancelPayDescription = "";
                        }
                        if (i == 2)
                        {
                            nonstandardSalaryPayment.SalaryPaymentCode = "PayPeriod 19-26";
                            nonstandardSalaryPayment.SALMemoCode = "";
                            nonstandardSalaryPayment.SALMemoAmount = null;
                            nonstandardSalaryPayment.DPAMemoCode = "DPA";
                            nonstandardSalaryPayment.DPAMemoAmount = -employeerate;
                            nonstandardSalaryPayment.CancelPayDescription = "";
                        }
                        nonstandardSalaryPaymentList.Add(nonstandardSalaryPayment);
                        break;
                    case "9 x 9":
                        var nonstandardSalaryPayment9X9 = new NonStandardSalaryPayment()
                        {
                            SalaryPaymentId = i,
                            SALMemoCode = "",
                            SALMemoAmount = null,
                            DPAMemoCode = "",
                            DPAMemoAmount = null
                        };
                        if (i == 1)
                        {
                            //NO RECORD
                            nonstandardSalaryPayment9X9.SalaryPaymentCode = "PayPeriod 1-18";
                        }
                        if (i == 2)
                        {
                            nonstandardSalaryPayment9X9.SalaryPaymentCode = "PayPeriod 19-26";
                            nonstandardSalaryPayment9X9.SALMemoCode = "";
                            nonstandardSalaryPayment9X9.SALMemoAmount = null;
                            nonstandardSalaryPayment9X9.DPAMemoCode = "";
                            nonstandardSalaryPayment9X9.DPAMemoAmount = null;
                            nonstandardSalaryPayment9X9.CancelPayDescription = "Y";
                        }
                        nonstandardSalaryPaymentList.Add(nonstandardSalaryPayment9X9);
                        break;
                    default:
                        break;
                }
            }
            return Json(nonstandardSalaryPaymentList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        */

        /// <summary>
        /// Returns the Exported Log inforamtion for a particular Employee with Employee Type and performs Totals of all the Amounts and Rate.
        /// Binds the Non Standard Salary Payment Grid.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="e_PositionId"></param>
        /// <returns></returns>
        /*
        public ActionResult NonStandardSalaryTotalPayment_Read([DataSourceRequest]DataSourceRequest request, int e_PositionId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            //var emptypeid = clientDbContext.E_Positions.Where(x => x.E_PositionId == e_PositionId).Select(m => m.EmployeeTypeId).FirstOrDefault();
            //var emptypedesc = clientDbContext.DdlEmployeeTypes.Where(x => x.EmployeeTypeId == emptypeid).Select(m => m.Description).FirstOrDefault();
            var emptypedesc = TempData["EmployeeTypeDescription"];
            var employeeid = clientDbContext.E_Positions.Where(ep => ep.E_PositionId == e_PositionId).Select(e => e.EmployeeId).FirstOrDefault();
            var fileno = clientDbContext.Employees.Where(x => x.EmployeeId == employeeid).Select(m => m.FileNumber).FirstOrDefault();

            //Returns the employee active current salary history Pay Rate
            //var employeecurrentsalaryhistoryrate = (from eps in clientDbContext.E_PositionSalaryHistories
            //                                        join drt in clientDbContext.DdlRateTypes on eps.RateTypeId equals drt.RateTypeId
            //                                        orderby eps.EffectiveDate descending
            //                                        where eps.E_PositionId == e_PositionId && eps.EffectiveDate != null && eps.EndDate == null
            //                                        select eps.PayRate).FirstOrDefault();
            //decimal? employeerate = Convert.ToDecimal(employeecurrentsalaryhistoryrate);

            //Returns the Exported Log and calculates Total Sum of Salry for the Employee for all Pay Period Numbers.
            if (emptypedesc == null)
                return Json(new { Message = "No Employee Type is assigned!", succeed = false }, JsonRequestBehavior.AllowGet);

            //Returns the Exported Log information of all the Pay Period Numbers for a particular Employee by FileNumber and EmployeeType
            var nonstadardExportedLog = GetExportedLogByEmployee(fileno, emptypedesc.ToString());
            decimal MemoAmount1Total = 0;
            decimal MemoAmount2Total = 0;
            decimal SalaryHistoryRateTotal = 0;
            string ExportedLogSALCode = "";
            string ExportedLogDPACode = "";
            string ExportedLogCalcelPay = "";
            if (nonstadardExportedLog.Count > 0)
            {
                foreach (var item in nonstadardExportedLog)
                {
                    if (!string.IsNullOrEmpty(item.MemoAmount1))
                        MemoAmount1Total += Convert.ToDecimal(item.MemoAmount1);
                    if (!String.IsNullOrEmpty(item.MemoAmount2))
                        MemoAmount2Total += Convert.ToDecimal(item.MemoAmount2);
                    if (!string.IsNullOrEmpty(item.SalaryHistoryRate))
                        SalaryHistoryRateTotal += Convert.ToDecimal(item.SalaryHistoryRate);

                    ExportedLogSALCode = item.MemoCode1;
                    ExportedLogDPACode = item.MemoCode2;
                    ExportedLogCalcelPay = item.CancelPay;
                }
            }

            List<NonStandardSalaryPayment> nonstandardSalaryPaymentList = new List<NonStandardSalaryPayment>();
            for (int i = 1; i <= 2; i++)
            {
                switch (emptypedesc)
                {
                    case "9 x 12":
                        var nonstandardSalaryPayment = new NonStandardSalaryPayment()
                        {
                            SalaryPaymentId = i
                        };
                        if (i == 1)
                        {
                            nonstandardSalaryPayment.SalaryPaymentCode = "PayPeriod 1-18";
                            nonstandardSalaryPayment.SALMemoCode = ExportedLogSALCode;//"SAL";
                            nonstandardSalaryPayment.SALMemoAmount = (decimal?)MemoAmount1Total == 0 ? null : (decimal?)MemoAmount1Total;//((decimal)26 / (decimal)18 * employeerate);
                            nonstandardSalaryPayment.DPAMemoCode = ExportedLogDPACode;//"DPA";
                            nonstandardSalaryPayment.DPAMemoAmount = (decimal?)MemoAmount2Total == 0 ? null : (decimal?)MemoAmount2Total;//nonstandardSalaryPayment.SALMemoAmount - employeerate;
                            nonstandardSalaryPayment.CancelPayDescription = "";
                        }
                        if (i == 2)
                        {
                            nonstandardSalaryPayment.SalaryPaymentCode = "PayPeriod 19-26";
                            nonstandardSalaryPayment.SALMemoCode = "";
                            nonstandardSalaryPayment.SALMemoAmount = null;
                            nonstandardSalaryPayment.DPAMemoCode = ExportedLogDPACode;//"DPA";
                            nonstandardSalaryPayment.DPAMemoAmount = (decimal?)SalaryHistoryRateTotal == 0 ? null : (decimal?)SalaryHistoryRateTotal;//-employeerate;
                            nonstandardSalaryPayment.CancelPayDescription = "";
                        }
                        nonstandardSalaryPaymentList.Add(nonstandardSalaryPayment);
                        break;
                    case "9 x 9":
                        var nonstandardSalaryPayment9X9 = new NonStandardSalaryPayment()
                        {
                            SalaryPaymentId = i,
                            SALMemoCode = "",
                            SALMemoAmount = null,
                            DPAMemoCode = "",
                            DPAMemoAmount = null
                        };
                        if (i == 1)
                        {
                            //NO RECORD
                            nonstandardSalaryPayment9X9.SalaryPaymentCode = "PayPeriod 1-18";
                        }
                        if (i == 2)
                        {
                            nonstandardSalaryPayment9X9.SalaryPaymentCode = "PayPeriod 19-26";
                            nonstandardSalaryPayment9X9.SALMemoCode = "";
                            nonstandardSalaryPayment9X9.SALMemoAmount = null;
                            nonstandardSalaryPayment9X9.DPAMemoCode = "";
                            nonstandardSalaryPayment9X9.DPAMemoAmount = null;
                            nonstandardSalaryPayment9X9.CancelPayDescription = ExportedLogCalcelPay;//"Y";
                        }
                        nonstandardSalaryPaymentList.Add(nonstandardSalaryPayment9X9);
                        break;
                    default:
                        break;
                }
            }
            return Json(nonstandardSalaryPaymentList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        */

        /// <summary>
        /// Returns the Exported Log information of all the Pay Period Numbers for a particular Employee by FileNumber and EmployeeType
        /// </summary>
        /// <param name="filenumber"></param>
        /// <param name="employeetype"></param>
        /// <returns></returns>
        /*
        public List<PayPeriodsExportedLogVM> GetExportedLogByEmployee(string filenumber, string employeetype)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            List<PayPeriodsExportedLogVM> payperiodsExportedLogList = new List<PayPeriodsExportedLogVM>();
            try
            {
                payperiodsExportedLogList = clientDbContext.PayPeriodsExportedLog.Where(x => x.FileNumber == filenumber && x.EmployeeType.Equals(employeetype))
                                                                                 .Select(d => new PayPeriodsExportedLogVM
                                                                                 {
                                                                                     MemoAmount1 = d.MemoAmount1,
                                                                                     MemoAmount2 = d.MemoAmount2,
                                                                                     MemoCode1 = d.MemoCode1,
                                                                                     MemoCode2 = d.MemoCode2,
                                                                                     SalaryHistoryRate = d.SalaryHistoryRate,
                                                                                     CancelPay = d.CancelPay
                                                                                 }).ToList();
                return payperiodsExportedLogList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        */

        /// <summary>
        /// Returns the Exported Log inforamtion for a particular Employee with Employee Type and performs Totals of all the Amounts and Rate.
        /// Binds the Non Standard Salary Payment Grid.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="e_PositionId"></param>
        /// <returns></returns>
        public ActionResult NonStandardSalaryTotalExportedPayment_Read([DataSourceRequest]DataSourceRequest request, int e_PositionId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            //var emptypeid = clientDbContext.E_Positions.Where(x => x.E_PositionId == e_PositionId).Select(m => m.EmployeeTypeId).FirstOrDefault();
            //var emptypedesc = clientDbContext.DdlEmployeeTypes.Where(x => x.EmployeeTypeId == emptypeid).Select(m => m.Description).FirstOrDefault();
            var emptypedesc = TempData["EmployeeTypeDescription"];
            var emptypecode = TempData["EmployeeTypeCode"];
            var employeeid = clientDbContext.E_Positions.Where(ep => ep.E_PositionId == e_PositionId).Select(e => e.EmployeeId).FirstOrDefault();
            var fileno = clientDbContext.Employees.Where(x => x.EmployeeId == employeeid).Select(m => m.FileNumber).FirstOrDefault();

            //Returns the Exported Log and calculates Total Sum of Salry for the Employee for all Pay Period Numbers.
            if (emptypedesc == null)
                return Json(new { Message = "No Employee Type is assigned!", succeed = false }, JsonRequestBehavior.AllowGet);

            //Returns the Exported Log information of all the Pay Period Numbers for a particular Employee by FileNumber and EmployeeType
            var nonstadardExportedLog = GetExportedLogByFileNumber(fileno);
            decimal MemoAmount1Total = 0;
            decimal MemoAmount2Total = 0;
            decimal SalaryHistoryRateTotal = 0;
            string ExportedLogSALCode = "";
            string ExportedLogDPACode = "";
            string ExportedLogCalcelPay = "";
            int PayPeriodNumber = 0;
            if (nonstadardExportedLog.Count > 0)
            {
                foreach (var item in nonstadardExportedLog)
                {
                    if (!string.IsNullOrEmpty(item.MemoCode1))
                        ExportedLogSALCode = item.MemoCode1;
                    if (!string.IsNullOrEmpty(item.MemoCode2))
                        ExportedLogDPACode = item.MemoCode2;
                    //ExportedLogCalcelPay = item.CancelPay;
                    PayPeriodNumber = Convert.ToInt32(item.PayNumber);
                    if (PayPeriodNumber >= 1 && PayPeriodNumber <= 19)
                    {
                        if (!string.IsNullOrEmpty(item.MemoAmount1.Trim()))
                            MemoAmount1Total += Convert.ToDecimal(item.MemoAmount1);
                        if (!String.IsNullOrEmpty(item.MemoAmount2.Trim()))
                            MemoAmount2Total += Convert.ToDecimal(item.MemoAmount2);
                    }
                    if (PayPeriodNumber >= 20 && PayPeriodNumber <= 26)
                    {
                        if (!string.IsNullOrEmpty(item.SalaryHistoryRate.Trim()))
                            SalaryHistoryRateTotal += Convert.ToDecimal(item.SalaryHistoryRate);
                    }
                    //After adding Contract record and Assign Employee Type,  
                    //On Export...2 records will be generated for there FileNumber
                    //First record for Employee Type 9 * 9 Cancel Pay "Y"
                    //Second record for Contract Export "Earnings3Amount" will show
                    if (string.IsNullOrEmpty(item.Earnings3Amount.Trim()))
                        ExportedLogCalcelPay = item.CancelPay;
                }
            }

            List<NonStandardSalaryPayment> nonstandardSalaryPaymentList = new List<NonStandardSalaryPayment>();
            /*  Table: DdlEmployeeTypes
                -------------------------------------------
             *  Id   Description             Code    Active
                -------------------------------------------
                1	 19/26 Faculty	         2	     1
                2	 Adjunct	             3	     1
                3	 19/19 Faculty	         1	     1
                4	 Staff less than 12mos	 4	     1
            */
            switch (emptypecode)//(emptypedesc)
            {
                case "2"://case "9 x 12":
                    for (int i = 1; i <= 2; i++)
                    {
                        var nonstandardSalaryPayment = new NonStandardSalaryPayment()
                        {
                            SalaryPaymentId = i
                        };
                        if (i == 1)
                        {
                            nonstandardSalaryPayment.SalaryPaymentCode = "PayPeriod 1-19";
                            nonstandardSalaryPayment.SALMemoAmount = (decimal?)MemoAmount1Total == 0 ? null : (decimal?)MemoAmount1Total;//((decimal)26 / (decimal)18 * employeerate);
                            if (nonstandardSalaryPayment.SALMemoAmount != null)
                                nonstandardSalaryPayment.SALMemoCode = ExportedLogSALCode;
                            else
                                nonstandardSalaryPayment.SALMemoCode = "";
                            nonstandardSalaryPayment.DPAMemoAmount = (decimal?)MemoAmount2Total == 0 ? null : (decimal?)MemoAmount2Total;//nonstandardSalaryPayment.SALMemoAmount - employeerate;
                            if (nonstandardSalaryPayment.DPAMemoAmount != null)
                                nonstandardSalaryPayment.DPAMemoCode = ExportedLogDPACode;
                            else
                                nonstandardSalaryPayment.DPAMemoCode = "";

                            nonstandardSalaryPayment.CancelPayDescription = "";
                        }
                        if (i == 2)
                        {
                            nonstandardSalaryPayment.SalaryPaymentCode = "PayPeriod 20-26";
                            nonstandardSalaryPayment.SALMemoCode = "";
                            nonstandardSalaryPayment.SALMemoAmount = null;
                            nonstandardSalaryPayment.DPAMemoAmount = (decimal?)SalaryHistoryRateTotal == 0 ? null : (decimal?)SalaryHistoryRateTotal;//-employeerate;
                            if (nonstandardSalaryPayment.DPAMemoAmount != null)
                                nonstandardSalaryPayment.DPAMemoCode = ExportedLogDPACode;//"DPA";
                            else
                                nonstandardSalaryPayment.DPAMemoCode = "";
                            nonstandardSalaryPayment.CancelPayDescription = "";
                        }
                        nonstandardSalaryPaymentList.Add(nonstandardSalaryPayment);
                    }
                    break;
                case "1"://case "9 x 9":
                    for (int i = 1; i <= 2; i++)
                    {
                        var nonstandardSalaryPayment9X9 = new NonStandardSalaryPayment()
                        {
                            SalaryPaymentId = i,
                            SALMemoCode = "",
                            SALMemoAmount = null,
                            DPAMemoCode = "",
                            DPAMemoAmount = null
                        };
                        if (i == 1)
                        {
                            //NO RECORD
                            nonstandardSalaryPayment9X9.SalaryPaymentCode = "PayPeriod 1-19";
                        }
                        if (i == 2)
                        {
                            nonstandardSalaryPayment9X9.SalaryPaymentCode = "PayPeriod 20-26";
                            nonstandardSalaryPayment9X9.SALMemoCode = "";
                            nonstandardSalaryPayment9X9.SALMemoAmount = null;
                            nonstandardSalaryPayment9X9.DPAMemoCode = "";
                            nonstandardSalaryPayment9X9.DPAMemoAmount = null;
                            if (!string.IsNullOrEmpty(ExportedLogCalcelPay.Trim()))
                                nonstandardSalaryPayment9X9.CancelPayDescription = ExportedLogCalcelPay;
                            //else
                            //    nonstandardSalaryPayment9X9.CancelPayDescription = "Y";
                        }
                        nonstandardSalaryPaymentList.Add(nonstandardSalaryPayment9X9);
                    }
                    break;
                default:
                    break;
            }

            return Json(nonstandardSalaryPaymentList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns the Exported Log information of all the Pay Period Numbers for a particular Employee by FileNumber and EmployeeType
        /// </summary>
        /// <param name="filenumber"></param>
        /// <returns></returns>
        public List<PayPeriodsExportedLogVM> GetExportedLogByFileNumber(string filenumber)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            List<PayPeriodsExportedLogVM> payperiodsExportedLogList = new List<PayPeriodsExportedLogVM>();
            try
            {
                payperiodsExportedLogList = clientDbContext.PayPeriodsExportedLog.Where(x => x.FileNumber == filenumber)
                                                                                 .Select(d => new PayPeriodsExportedLogVM
                                                                                 {
                                                                                     MemoAmount1 = d.MemoAmount1,
                                                                                     MemoAmount2 = d.MemoAmount2,
                                                                                     MemoCode1 = d.MemoCode1,
                                                                                     MemoCode2 = d.MemoCode2,
                                                                                     SalaryHistoryRate = d.SalaryHistoryRate,
                                                                                     CancelPay = d.CancelPay,
                                                                                     PayNumber = d.PayNumber,
                                                                                     Earnings3Amount = d.Earnings3Amount
                                                                                 }).ToList();
                return payperiodsExportedLogList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
