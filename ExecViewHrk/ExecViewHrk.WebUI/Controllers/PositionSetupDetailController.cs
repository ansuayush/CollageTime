using ExecViewHrk.EfClient;
using System.Web.Mvc;
using ExecViewHrk.WebUI.Models;
using ExecViewHrk.WebUI.Services;
using System.Linq;
using ExecViewHrk.WebUI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.EfAdmin;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Net;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PositionSetupDetailController : Controller
    {
        // GET: PositionSetupDetail
        readonly PositionService positionService = new PositionService();
        readonly PositionFundService positionFundService = new PositionFundService();
        public ActionResult PositionSetupDetail(int? positionID)
        {
            PositionDetailsVm details = new PositionDetailsVm();
            if ((positionID != null))
            {
                details = positionService.getPositionDetails(positionID.Value);
                var oldvalue = details.Division;
                var olddepartment = details.DepartmentId;
                var oldsalarygradecode = details.SalaryGrade;
                var oldpayfrequency = details.PayFrequencyCode;
                var oldsalaryplancode = details.SalaryPlanCode;
                var oldsalarystep = details.SalaryStep;

                Session["Division"] = oldvalue;

                Session["Department"] = olddepartment;

                Session["SalaryGrade"] = oldsalarygradecode;

                Session["PayFrequency"] = oldpayfrequency;

                Session["SalaryPlanCode"] = oldsalaryplancode;

                Session["SalaryStep"] = oldsalarystep;
            }
            return View(details);
        }

        public ActionResult PositionAddModalPartial()
        {
            var details = positionService.getNewPosition();
            return View(details);
        }

        public ActionResult PositionViewAllocationModalPartial(int? positionBudgetID)
        {

            IEnumerable<PositionBudgetFundAllocationVm> details = positionService.GetPositionBudgetAllocation(positionBudgetID).ToList();
            return PartialView(details);
        }

        public ActionResult PositionBudgetEditModalPartial(int? positionBudgetID)
        {
            var budget = positionService.getPositionBudget(positionBudgetID, null);
            return PartialView(budget);
        }

        public ActionResult GridPositionBudgetFundAllocationRead(int? positionBudgetID)
        {
            IEnumerable<PositionBudgetFundAllocationVm> details = positionService.GetPositionBudgetAllocation(positionBudgetID).ToList();
            return Json(details, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PositionBudgetDelete(int ID)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.PositionsBudgets.Where(x => x.ID == ID).SingleOrDefault();

            if (dbRecord != null)
            {
                var mList = clientDbContext.PositionsBudgetsMonths.Where(x => x.PositionBudgetsID == dbRecord.ID).ToList();
                for (int i = 0; i < mList.Count; i++)
                {
                    clientDbContext.PositionsBudgetsMonths.Remove(mList[i]);
                }
                clientDbContext.SaveChanges();
                clientDbContext.PositionsBudgets.Remove(dbRecord);
                try
                {

                    clientDbContext.SaveChanges();
                    PositionDetailsVm details;
                    details = positionService.getPositionDetails(dbRecord.PositionID);

                    return PartialView("PositionBudgetPartial", details);
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


        }


        public ActionResult GridPositionFundHistoryRead()
        {
            IEnumerable<FundHistoryVm> fundHistory = positionService.GetPositionFundDefinition().FundDefinitionList.ToList();
            return Json(fundHistory, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PositionBudgetAddModalPartial(short? positionID)
        {
            var bubget = positionService.getPositionBudget(null, positionID);
            return PartialView(bubget);
        }

        public ActionResult PositionAddAllocationModalPartial(int? positionBudgetID)
        {
            var bubget = positionService.GetPositionBudgetFundAllocation(positionBudgetID);
            return PartialView(bubget);
        }

        public ActionResult PositionAddFundDefinitionModalPartial()
        {
            var fundHistory = positionService.GetPositionFundDefinition();
            return PartialView(fundHistory);
        }



        [HttpPost]
        public ActionResult SaveSalaryGrade(AddSalaryGradeitem addSalaryGradeitem)
        {
            var saveResult = positionService.saveSalaryGrade(addSalaryGradeitem);
            return Content(saveResult);
        }

        [HttpPost]
        public ActionResult SaveBudgeMonthAmount(PositionBudgetMonths positionBudgetMonths)
        {
            var saveResult = positionService.SaveBudgeMonthAmount(positionBudgetMonths);
            return Content(saveResult);
        }

        [HttpPost]
        public ActionResult SavePositionBudget(PositionBudgetsVM positionBudgetsVM, IEnumerable<PositionBudgetMonthsVM> positionBudgetMonthsVM)
        {

            positionBudgetsVM.BudgetMonthList = positionBudgetMonthsVM != null ? positionBudgetMonthsVM.ToList() : null;
            var saveResult = positionService.savePositionBudget(positionBudgetsVM);
            try
            {
                int positionBudgetID = Convert.ToInt32(saveResult);
                PositionDetailsVm details = new PositionDetailsVm();
                if (positionBudgetID != 0)
                {
                    details = positionService.getPositionDetails(positionBudgetsVM.PositionID);
                }
                return PartialView("PositionBudgetPartial", details);

            }
            catch
            {
                return Json(new { Message = saveResult, succeed = false }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult SavePositionBudgetAllocation(PositionFundsVm positionBudgetFundsVM)
        {
            var saveResult = positionService.savePositionBudgetFund(positionBudgetFundsVM);
            try
            {
                Convert.ToInt32(saveResult);
                return Content(saveResult);
            }
            catch
            {
                return Json(new { Message = saveResult, succeed = false }, JsonRequestBehavior.AllowGet);
            }



        }


        [HttpPost]
        public ActionResult SaveFundHistory(FormCollection fundHistory)
        {
            var effDate = fundHistory["EffectiveDate"];
            string[] dateString = effDate.Split('/');
            DateTime? effectiveDate = null;
            if (dateString.Length > 1)
            {
                effectiveDate = Convert.ToDateTime(dateString[1] + "/" + dateString[0] + "/" + dateString[2]);
            }

            var fundHistoryVm = new FundHistoryAddVm
            {

                FundCode = fundHistory["FundCode"].ToString(),
                FundDescription = fundHistory["FundDescription"].ToString(),
                EffectiveDate = effectiveDate,
                Amount = Convert.ToDecimal(fundHistory["Amount"])
            };
            var saveResult = positionService.SavePositionFundHistory(fundHistoryVm);
            return Content(saveResult);
        }

        public JsonResult FillSalaryDescription()
        {
            string connString = User.Identity.GetClientConnectionString();

            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var salarydescription = clientDbContext.DdlSalaryGrades.Select(x => new DropDownModel { keyvalue = x.SalaryGradeID.ToString(), keydescription = x.description }).ToList();
            salarydescription.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "--Select--" });
            return Json(salarydescription, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RefreshSalaryGradeHistoryList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var ddlSalaryGradeHistoryList = (from gh in clientDbContext.DdlSalaryGradeHistory
                                             join sg in clientDbContext.DdlSalaryGrades on
                                             gh.SalaryGradeID equals sg.SalaryGradeID
                                             where sg.active == true
                                             select new SalaryGradeVm
                                             {
                                                 ID = gh.ID,
                                                 code = sg.description,
                                                 description = sg.description,
                                                 salaryMaximum = gh.salaryMaximum,
                                                 salaryMidpoint = gh.salaryMidpoint,
                                                 salaryMinimum = gh.salaryMinimum,
                                                 validFrom = gh.validFrom,
                                                 active = sg.active
                                             }).ToList();
            System.Collections.Generic.List<SalaryGradeReloadGrid> salaryGradeReloadGrid = ddlSalaryGradeHistoryList.Select(m => new SalaryGradeReloadGrid
            {

                ID = m.ID,
                code = m.code,
                description = m.description,
                salaryMaximum = m.salaryMaximum,
                salaryMidpoint = m.salaryMinimum,
                salaryMinimum = m.salaryMinimum,
                validFrom = m.validFrom != null ? m.validFrom.Value.ToString("MM/dd/yyyy") : "",
                active = m.active
            }).ToList();
            return Json(salaryGradeReloadGrid, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GridPositionBudgetsListRefresh(int? positionID)
        {
            IEnumerable<PositionBudgetsVM> list = positionService.GetPositionBudgets(positionID).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PositionSaveAjax(PositionDetailsVm positionDetailsVm)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            if (positionDetailsVm.PositionId == 0)
            {
                var businessLevelNbr = clientDbContext.PositionBusinessLevels.Where(x => x.BusinessLevelCode == positionDetailsVm.BUCode).Select(x => x.BusinessLevelNbr).FirstOrDefault();
                var jobId = clientDbContext.Jobs.Where(x => x.JobCode == positionDetailsVm.JobCode).Select(x => x.JobId).FirstOrDefault();
                positionDetailsVm.BusinessLevelNbr = businessLevelNbr;
                positionDetailsVm.JobId = jobId;
            }
            string buCode = "", jobCode = "";
            if (positionDetailsVm.BUCode == null)
            {
                buCode = clientDbContext.PositionBusinessLevels.Where(x => x.BusinessLevelNbr == positionDetailsVm.BusinessLevelNbr).Select(x => x.BusinessLevelCode).FirstOrDefault();
                jobCode = clientDbContext.Jobs.Where(x => x.JobId == positionDetailsVm.JobId).Select(x => x.JobCode).FirstOrDefault();
            }
            else
            {
                buCode = positionDetailsVm.BUCode;
                jobCode = positionDetailsVm.JobCode;
            }
            bool recordIsNew = false;
            string _message = "";
            positionDetailsVm.EnteredBy = User.Identity.Name;
            var posCode = buCode + jobCode;
            //var posCode = code1;//positionDetailsVm.PositionCode;
            positionDetailsVm.PositionCode = posCode;
            positionDetailsVm.Code = posCode;
            if (positionDetailsVm.PositionCode==null)
            {
                var getPosCode = clientDbContext.Positions.Where(x => x.Code == posCode).Select(x => x.PositionCode).FirstOrDefault();
                positionDetailsVm.Code = posCode;
                positionDetailsVm.PositionCode = getPosCode;
            }
            if (positionDetailsVm.CurrentStatus == "Closed")
            {
                if(positionDetailsVm.ClosedDate==null)
                {
                    return Json(new { succeed = false, Message = "Please enter Closed Date" }, JsonRequestBehavior.AllowGet);
                }
            }
            if (Session["Division"] != null)
            {
                positionDetailsVm.oldval = Session["Division"].ToString();
            }
            else
            {
                positionDetailsVm.oldval = string.Empty;
            }
            if (Session["Department"] != null && Session["Department"].ToString() != "0")
            {
                positionDetailsVm.olddepartment = Session["Department"].ToString();
            }
            else
            {
                positionDetailsVm.olddepartment = string.Empty;
            }
            if (Session["SalaryGrade"] != null)
            {
                positionDetailsVm.oldsalarygrade = Session["SalaryGrade"].ToString();
            }
            else
            {
                positionDetailsVm.oldsalarygrade = string.Empty;
            }
            if (Session["PayFrequency"] != null)
            {
                positionDetailsVm.oldpayfrequency = Session["PayFrequency"].ToString();
            }
            else
            {
                positionDetailsVm.oldpayfrequency = string.Empty;
            }
            if (Session["SalaryPlanCode"] != null)
            {
                positionDetailsVm.oldsalaryplancode = Session["SalaryPlanCode"].ToString();
            }
            else
            {
                positionDetailsVm.oldsalaryplancode = string.Empty;

            }
            if (Session["SalaryStep"] != null)
            {
                positionDetailsVm.oldsalarystep = Session["SalaryStep"].ToString();
            }
            else
            {
                positionDetailsVm.oldsalarystep = string.Empty;
            }
            if (positionDetailsVm.PositionId == 0)
            {
                var posCount = clientDbContext.Positions.Where(x => x.PositionCode == posCode && x.Suffix == positionDetailsVm.Suffix).Count();
                if (posCount > 0)
                {
                    return Json(new { succeed = false, Message = "The Position Code already exists" }, JsonRequestBehavior.AllowGet);
                }
            }
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                try
                {
                    if (positionDetailsVm.PositionId == 0) { recordIsNew = true; }
                    positionDetailsVm = positionService.SavePosition(positionDetailsVm);
                    return Json(new { positionDetailsVm, succeed = true, recordIsNew }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    _message = Utils.GetErrorString(err, clientDbContext, null);
                    return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                _message = Utils.GetErrorString(null, null, this.ModelState);
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult FillFundCodeDropDown()
        {
            string connString = User.Identity.GetClientConnectionString();

            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var funddescription = clientDbContext.Funds.Select(x => new DropDownModel { keyvalue = x.ID.ToString(), keydescription = x.Description }).ToList();
            funddescription.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "--Select--" });
            return Json(funddescription, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobCode(string JobDescription)
        {
            string connString = User.Identity.GetClientConnectionString();

            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var Data = clientDbContext.Jobs.Where(x => x.JobDescription == JobDescription).FirstOrDefault();
            return Json(Data.JobCode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBuCode(string BUDescription)
        {
            string connString = User.Identity.GetClientConnectionString();

            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var Data = clientDbContext.PositionBusinessLevels.Where(x => x.BusinessLevelTitle == BUDescription).FirstOrDefault();
            return Json(Data.BusinessLevelCode, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SavepositionFundingSource(List<PosionFundingSourceListVM> sources ,DateTime effectiveDate)
        {
          try
            {
                var saveResult = positionService.SaveFundingSourceList(sources, effectiveDate);          //savePositionFundingSourceitem(sources);
                return Content(saveResult);
            }
            catch
            {
                return Content("");
            }
        }

        [HttpGet]
        public ActionResult RefreshPositionFundSourceList(DateTime EffectiveDate)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var positionfundsourceGrid = (from PFS in clientDbContext.PositionFundingSource
                                          join FD in clientDbContext.Funds
                                          on PFS.FundCodeID equals FD.ID
                                          where FD.Active == true
                                          select new PosionFundingSourceListVM
                                          {
                                              EffectiveDate = PFS.EffectiveDate,
                                              ID = PFS.PositionFundingSourceID.ToString(),
                                              FundCode = FD.Description,
                                              FundPercentage = PFS.Percentage
                                          }).ToList().GroupBy(x => x.EffectiveDate).Select(x => x.First()).ToList();
            //var positionfundsourceGrid = (from PFS in clientDbContext.PositionFundingSource
            // join FD in clientDbContext.Funds
            // on PFS.FundCodeID equals FD.ID
            // where FD.Active == true && PFS.EffectiveDate == EffectiveDate
            //                              select new PosionFundingSourceListVM
            // {
            //     EffectiveDate = PFS.EffectiveDate,
            //     ID = PFS.PositionFundingSourceID.ToString(),
            //     FundCode = FD.Description,
            //     FundPercentage = PFS.Percentage
            // }).ToList();
            return Json(positionfundsourceGrid, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditPositionFundingSource(int FundingSourceID)
        {
            PositionFundingSourceVM positionFundingSource = null;
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            positionFundingSource = (from PDS in clientDbContext.PositionFundingSource
                                     where PDS.PositionFundingSourceID == FundingSourceID
                                     select new PositionFundingSourceVM
                                     {
                                         EditEffectiveDate = PDS.EffectiveDate,
                                         //EditPositionFundingSourceID = PDS.PositionFundingSourceID,
                                         //EditFundCodeID = PDS.FundCodeID,
                                         //EditPercentage = PDS.Percentage
                                     }).FirstOrDefault();
            var funddescription = clientDbContext.Funds.Select(x => new DropDownModel { keyvalue = x.ID.ToString(), keydescription = x.Description }).ToList();
            funddescription.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "--Select--" });
            positionFundingSource.DDlFundCode = funddescription;
            //**********************************************************
            var positionfundsourceGrid = (from PFS in clientDbContext.PositionFundingSource
                                          join FD in clientDbContext.Funds
                                          on PFS.FundCodeID equals FD.ID
                                          where FD.Active == true && PFS.EffectiveDate == positionFundingSource.EditEffectiveDate
                                          select new PosionFundingSourceListVM
                                          {
                                              EffectiveDate = PFS.EffectiveDate,
                                              ID = PFS.PositionFundingSourceID.ToString(),
                                              FundCode = FD.Description,
                                              FundPercentage = PFS.Percentage
                                          }).ToList();

            positionFundingSource.EditposionFundingSourceList = positionfundsourceGrid;
            //**********************************************************
            return PartialView("_EditPositionFunding", positionFundingSource);
        }
        public ActionResult UpdatepositionFundingSource(AddPositionFundSourceitem editPositionFundsourceitem)
        {
            var saveResult = positionService.savePositionFundingSourceitem(editPositionFundsourceitem);
            return Content(saveResult);
        }

        public ActionResult RefreshPositionFundSourceHistoryList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var positionfundsourceHistoryGrid = (from PFSH in clientDbContext.PositionFundingSourceHistories
                                                 join FD in clientDbContext.Funds
                                          on PFSH.FundCodeID equals FD.ID
                                                 where FD.Active == true
                                                 select new PosionFundingSourceHistoryListVM
                                                 {
                                                     EffectiveDate = PFSH.EffectiveDate,
                                                     FundCodeID = FD.Description,
                                                     Percentage = PFSH.Percentage.ToString(),
                                                     ChangeEffectiveDate = PFSH.ChangeEffectiveDate
                                                 }).ToList();
            return Json(positionfundsourceHistoryGrid, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillYearList()
        {
            var yearlist = Enumerable.Range(2011, 10).Select(x => new DropDownModel
            {
                keyvalue = x.ToString(),
                keydescription = x.ToString(),
            }).ToList();
            return Json(yearlist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillFundingList(DateTime EffectiveDate)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var positionfundsourceGrid = (from PFS in clientDbContext.PositionFundingSource
                                          join FD in clientDbContext.Funds
                                          on PFS.FundCodeID equals FD.ID
                                          where FD.Active == true && PFS.EffectiveDate == EffectiveDate
                                          select new PosionFundingSourceListVM
                                          {
                                              EffectiveDate = PFS.EffectiveDate,
                                              ID = PFS.PositionFundingSourceID.ToString(),
                                              FundCode = FD.Description,
                                              FundCodeID = FD.ID,
                                              FundPercentage = PFS.Percentage
                                          }).ToList();
            return Json(positionfundsourceGrid, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FillMonthList()
        {
            var monthlist = Enumerable.Range(1, 12).Select(x => new DropDownModel
            {
                keyvalue = x.ToString(),
                keydescription = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(x)
            }).ToList();
            return Json(monthlist, JsonRequestBehavior.AllowGet);
        }

        //
        //public ActionResult UpdatePositionBudget(PositionBudgetsVM positionBudgetUpdate)
        //{
        //    var saveResult = positionService.UpdatePositionBudget(positionBudgetUpdate);
        //    return Content(saveResult);
        //}

        [HttpGet]
        public ActionResult RefreshPositionBudgetList(int positionID)
        {
            var positionsBudgets = positionService.GetPositionBudgetList(positionID).Where(x => x.PositionID == positionID).ToList();
            return Json(positionsBudgets, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditPositionSalaryDetailGrad(short SalaryGradeGridID)
        {
            EditPositionSalaryGrad editPositionSalaryGrad = new EditPositionSalaryGrad();
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            //*****************************************
            var result = clientDbContext.DdlSalaryGradeHistory
                  .Include(a => a.DdlSalaryGrades)
                  .Select(x => new SalaryGradeVm
                  {
                      ID = x.ID,
                      code = x.DdlSalaryGrades.code,
                      description = x.DdlSalaryGrades.description,
                      salaryMaximum = x.salaryMaximum,
                      salaryMidpoint = x.salaryMidpoint,
                      salaryMinimum = x.salaryMinimum,
                      validFrom = x.validFrom,
                      active = x.DdlSalaryGrades.active
                  }).Where(x => (x.active == true) && (x.ID == SalaryGradeGridID)).FirstOrDefault();
            editPositionSalaryGrad.EditPositionSalaryDescription = result.description;
            editPositionSalaryGrad.EditPositionSalarySalaryMin = result.salaryMinimum;
            editPositionSalaryGrad.EditPositionSalarySalaryMad = result.salaryMidpoint;
            editPositionSalaryGrad.EditPositionSalarySalaryMax = result.salaryMaximum;
            editPositionSalaryGrad.EditPositionSalaryVadidDate = result.validFrom;
            editPositionSalaryGrad.EditSalaryGradeGridID = SalaryGradeGridID;
            //********************************************
            //var salarydescription = clientDbContext.DdlSalaryGrades.Select(x => new DropDownModel { keyvalue = x.SalaryGradeID.ToString(), keydescription = x.description }).ToList();
            //salarydescription.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "--Select--" });
            //editPositionSalaryGrad.DDlSalaryGrad = salarydescription;
            return PartialView("_EditPositionSalaryGrade", editPositionSalaryGrad);
        }

        public ActionResult RelodPositionSalaryGradeHistoryList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var ddlSalaryGradeHistoryList = (from psgsh in clientDbContext.PositionSalaryGradeSourceHistories
                                             join sg in clientDbContext.DdlSalaryGrades on
                                                 psgsh.SalaryGradeID equals sg.SalaryGradeID
                                             where sg.active == true
                                             select new SalaryGradeHistoryListVm
                                             {
                                                 description = sg.description,
                                                 salaryMaximum = psgsh.salaryMaximum.ToString(),
                                                 salaryMidpoint = psgsh.salaryMidpoint.ToString(),
                                                 salaryMinimum = psgsh.salaryMinimum.ToString(),
                                                 validFrom = psgsh.ValidDate,
                                                 ChangeDate = psgsh.ChangeEffectiveDate
                                             }).ToList();

            return Json(ddlSalaryGradeHistoryList, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult CheckPercentageFundingList(DateTime EffectiveDate)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var positionsfund = (from PFS in clientDbContext.PositionFundingSource
                                 join FD in clientDbContext.Funds
                                 on PFS.FundCodeID equals FD.ID
                                 where FD.Active == true && PFS.EffectiveDate == EffectiveDate
                                 select new PosionFundingSourceListVM
                                 {
                                     FundPercentage = PFS.Percentage
                                 }).ToList().Select(x => x.FundPercentage).Sum();
            if (positionsfund != 100)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(positionsfund, JsonRequestBehavior.AllowGet);
            }
        }

        // Funding Source Integration
        public ActionResult PositionFundSelectionModalPartial()
        {
            PositionFundingSourceGroupVM positionFundingSource = positionService.getNewPositionFundingSource();
            return PartialView(positionFundingSource);
        }


        [HttpPost]
        public ActionResult AddNewFundingSource(List<PosionFundingSourceListVM> sources, PositionFundingSourceGroupVM pFSG)
        {
            var FSourceList = new PosionFundingSourceListVM();
            var newpFSG = new PositionFundingSourceGroupVM();
            FSourceList.EffectiveDate = pFSG.EditEffectiveDate.Value;
            FSourceList.FundCodeID = pFSG.EditFundCodeID != null ? Convert.ToInt32(pFSG.EditFundCodeID) : 0;
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            FSourceList.FundCode = clientDbContext.Funds.FirstOrDefault(x => x.ID == FSourceList.FundCodeID).Code;
            FSourceList.FundPercentage = pFSG.EditPercentage;
            var funddescription = clientDbContext.Funds.Select(x => new DropDownModel { keyvalue = x.ID.ToString(), keydescription = x.Description }).ToList();
            funddescription.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "--Select--" });
            sources = sources == null ? new List<PosionFundingSourceListVM>() : sources;
            FSourceList.ID = (-1 * (sources.Count)).ToString();
            FSourceList.FundCodeID = Convert.ToInt32(pFSG.EditFundCodeID);
            FSourceList.PositionId = pFSG.PositionId;
            sources.Add(FSourceList);
            newpFSG.EditposionFundingSourceList = sources.ToList();
            newpFSG.FundCodes = funddescription;
            return PartialView("PositionFundSelectionModalPartial", newpFSG);

        }
        public ActionResult FundAllocationList_Destroy([DataSourceRequest] DataSourceRequest request, PositionBudgetFundAllocationVm positionBudgetFundAllocationVm)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var Fundid = clientDbContext.Funds.Where(m => m.Code == positionBudgetFundAllocationVm.FundCode).FirstOrDefault();
            try
            {
                clientDbContext.Database.ExecuteSqlCommand(" " +
                               "DELETE FROM positionfunds " +
                               "WHERE PositionBudgetID = @PositionBudgetID and FundID=@FundID",
                               new SqlParameter("@PositionBudgetID", positionBudgetFundAllocationVm.PositionFundID), new SqlParameter("@FundID", Fundid.ID));
            }
            catch
            {
                ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
            }


            return Json(new[] { positionBudgetFundAllocationVm }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult EditPositionFund(string FundCode, int PositionBudgetID)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            List<PositionBudgetFundAllocationVm> Fund = new List<PositionBudgetFundAllocationVm>();
            Fund = positionFundService.getPositionBudgetFundAllocation(PositionBudgetID).ToList();
            PositionFundsVm EditFundModel = new PositionFundsVm();
            var Data = Fund.Where(x => x.FundCode == FundCode).FirstOrDefault();
            EditFundModel.Amount = Data.Budget;
            EditFundModel.FundCode = FundCode;        
            EditFundModel.PositionBudgetID = PositionBudgetID;
            var datalist= positionFundService.getNewPositionBudgetFundAllocation();
            EditFundModel.FundCodes = datalist.FundCodes.ToList();
            EditFundModel.FundID = clientDbContext.Funds.Where(m => m.Code == FundCode).Select(m => m.ID).FirstOrDefault();
            return PartialView("PositionAddAllocationModalPartial",EditFundModel);
           
        }
        public ActionResult GridPositionHistoryRead([DataSourceRequest]DataSourceRequest request)
        {
            var details = positionService.getpositionhistory().ToList();
            return Json(details.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}