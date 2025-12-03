using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.EfAdmin;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Net;
using System.Data;

namespace ExecViewHrk.WebUI.Controllers
{
    public class JobSetupController : Controller
    {
        // GET: JobSetup
        public ActionResult JobSetupDetail(int jobid)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            JobSetupDetail _jobSetupDetail = clientDbContext.Jobs
                .Include(x => x.DdlJobClasses.Description)
                .Include(x => x.DdlWorkersCompensations.Description)
                .Include(x => x.DdlEEOJobCodes.Description)
                .Include(x => x.DdlFLSAs.Description)
                .Include(x => x.DdlEEOJobTrainingStatuses.Description)
                .Include(x => x.DdlUnions.Description)
                .Include(x => x.DdlJobFamilys.Description)
                 .Include(x => x.CompanyCode.CompanyCodeDescription)
           .Where(x => x.JobId == jobid)
                .Select(x => new Models.JobSetupDetail
                {
                    Jobid = x.JobId,
                    JobDescription = x.JobDescription,
                    JobCode = x.JobCode,
                    title = x.title,
                    jobClassIDDescription = x.DdlJobClasses.Description,
                    jobClassID = x.jobClassID.ToString(),
                    workersCompensationIDDescription = x.DdlWorkersCompensations.Description,
                    workersCompensationID = x.workersCompensationID.ToString(),
                    eeoJobCodeIDDescription = x.DdlEEOJobCodes.Description,
                    eeoJobCodeID = x.eeoJobCodeID.ToString(),
                    eeoJobTrainingStatusIDDescription = x.DdlEEOJobTrainingStatuses.Description,
                    eeoJobTrainingStatusID = x.eeoJobTrainingStatusID.ToString(),
                    CompanyCodeIdDescription = x.CompanyCode.CompanyCodeDescription,
                    CompanyCodeID = x.CompanyCodeId.ToString(),
                    createdDate = x.createdDate,
                    endDate = x.endDate,
                    FLSAIDDescription = x.DdlFLSAs.Description,
                    FLSAID = x.FLSAID.ToString(),
                    enteredBy = x.enteredBy,
                    enteredDate = x.enteredDate,
                    lastEvaluationDate = x.lastEvaluationDate,
                    requirements = x.requirements,
                    salaryRange = x.salaryRange,
                    unionIDDescription = x.DdlUnions.Description,
                    unionID = x.unionID.ToString(),
                    Notes = x.Notes,
                    JobFamilyIdDescription = x.DdlJobFamilys.Description,
                    JobFamilyId = x.JobFamilyId,
                })
                .FirstOrDefault();
            _jobSetupDetail.UnionsList = clientDbContext.DdlUnions.Where(x => x.Active == true).ToList();
            _jobSetupDetail.WorkersCompensationsList = clientDbContext.DdlWorkersCompensations.Where(x => x.Active == true).ToList();
            _jobSetupDetail.FLSAList = clientDbContext.DddlFLSAs.Where(x => x.Active == true).ToList();
            _jobSetupDetail.EEOJobCodesList = clientDbContext.DdlEEOJobCodes.Where(x => x.Active == true).ToList();
            _jobSetupDetail.EEOJobTrainingStatusesList = clientDbContext.DdlEEOJobTrainingStatuses.Where(x => x.Active == true).ToList();
            _jobSetupDetail.JobFamilyList = clientDbContext.DdlJobFamilys.Where(x => x.Active == true).ToList();
            _jobSetupDetail.JobClassList = clientDbContext.DdlJobClasses.Where(x => x.Active == true).ToList();
            _jobSetupDetail.CompanyCodeList = clientDbContext.CompanyCodes.Where(x => x.IsCompanyCodeActive == true).ToList();
            return View(_jobSetupDetail);
        }
        public ActionResult JobSetupList()
        {
            return View("JobSetupList");
        }
        public ActionResult JobList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var JobList = clientDbContext.Jobs
             .Include(x => x.DdlJobClasses.Description)
             .Include(x => x.DdlWorkersCompensations.Description)
             .Include(x => x.DdlEEOJobCodes.Description)
             .Include(x => x.DdlFLSAs.Description)
             .Include(x => x.DdlEEOJobTrainingStatuses.Description)
             .Include(x => x.DdlUnions.Description)
             .Include(x => x.DdlJobFamilys.Description)
               .Select(x => new JobSetupDetail
               {
                   Jobid = x.JobId,
                   JobDescription = x.JobDescription,
                   JobCode = x.JobCode,
                   title = x.title,
                   jobClassIDDescription = x.DdlJobClasses.Description,
                   workersCompensationIDDescription = x.DdlWorkersCompensations.Description,
                   eeoJobCodeIDDescription = x.DdlEEOJobCodes.Description,
                   eeoJobTrainingStatusIDDescription = x.DdlEEOJobTrainingStatuses.Description,
                   createdDate = x.createdDate,
                   endDate = x.endDate,
                   FLSAIDDescription = x.DdlFLSAs.Description,
                   enteredBy = x.enteredBy,
                   enteredDate = x.enteredDate,
                   lastEvaluationDate = x.lastEvaluationDate,
                   requirements = x.requirements,
                   salaryRange = x.salaryRange,
                   unionIDDescription = x.DdlUnions.Description,
                   Notes = x.Notes,
                   JobFamilyIdDescription = x.DdlJobFamilys.Description,
                   CompanyCodeID = x.CompanyCodeId.ToString()
               }).ToList();
            return Json(JobList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobList_Create([DataSourceRequest] DataSourceRequest request, Job e_PositionSalaryHistory)
        {
            return Json("");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobList_Update([DataSourceRequest] DataSourceRequest request, Job Jobs)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (Jobs != null && ModelState.IsValid)
                {
                    var JobDb = clientDbContext.Jobs
                        .Where(x => x.JobId == Jobs.JobId)
                        .SingleOrDefault();

                    if (JobDb != null)
                    {
                        JobDb.JobId = Jobs.JobId;
                        JobDb.title = Jobs.title;
                        JobDb.Notes = Jobs.Notes;
                        JobDb.JobDescription = Jobs.JobDescription;

                        clientDbContext.SaveChanges();
                    }
                }
                return Json(new[] { Jobs }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobList_Destroy([DataSourceRequest] DataSourceRequest request, JobSetupDetail jobs)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            try
            {
                if(clientDbContext.Positions.Any(j => j.JobId == jobs.Jobid))
                    return Json(new { Message = CustomErrorMessages.ERROR_DELETING_RECORD_ALREADY_IN_USE, succeed = false }, JsonRequestBehavior.AllowGet);

                clientDbContext.Database.ExecuteSqlCommand(" DELETE FROM jobs WHERE jobid = @jobid ", new SqlParameter("@jobid", jobs.Jobid));
            }
            catch// (Exception ex)
            {
                ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
            }


            return Json(new[] { jobs }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult JobSaveAjax(JobSetupDetail JobSetUp)
        {
            bool recordIsNew = false;
            string _message = "";
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            Job JobRecord = clientDbContext.Jobs.Where(x => x.JobId == JobSetUp.Jobid).SingleOrDefault();
            if (ModelState.IsValid)
            {
                if (JobRecord == null)
                {
                    recordIsNew = true;
                    //if(recordIsNew)
                    var jobCodeInDb = clientDbContext.Jobs.Where(x => x.JobCode == JobSetUp.JobCode).SingleOrDefault();
                    if (jobCodeInDb != null)
                    {

                        ModelState.AddModelError("", "The Job Code" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                        var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                        foreach (var item in modelStateErrors)
                        {
                            if (_message != "") _message += "<br />";
                            _message += item.ErrorMessage;
                        }
                        return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        JobRecord = new Job();
                        JobRecord.title = JobSetUp.title;
                        JobRecord.JobCode = JobSetUp.JobCode;
                        JobRecord.JobId = JobSetUp.Jobid;
                        JobRecord.JobDescription = JobSetUp.JobDescription;
                        JobRecord.jobClassID = JobSetUp.jobClassID != "0" ? Convert.ToSByte(JobSetUp.jobClassID) : (short?)null; ;
                        JobRecord.workersCompensationID = JobSetUp.workersCompensationID != "0" ? Convert.ToSByte(JobSetUp.workersCompensationID) : (short?)null;
                        JobRecord.eeoJobCodeID = JobSetUp.eeoJobCodeID != "0" ? Convert.ToSByte(JobSetUp.eeoJobCodeID) : (short?)null; ;
                        JobRecord.eeoJobTrainingStatusID = JobSetUp.eeoJobTrainingStatusID != "0" ? Convert.ToSByte(JobSetUp.eeoJobTrainingStatusID) : (short?)null;
                        JobRecord.createdDate = DateTime.Now;
                        JobRecord.endDate = JobSetUp.endDate;
                        JobRecord.FLSAID = JobSetUp.FLSAID != "0" ? Convert.ToSByte(JobSetUp.FLSAID) : (short?)null;
                        JobRecord.enteredBy = User.Identity.Name;
                        JobRecord.enteredDate = DateTime.Now;
                        JobRecord.lastEvaluationDate = JobSetUp.lastEvaluationDate;
                        JobRecord.requirements = JobSetUp.requirements;
                        JobRecord.salaryRange = JobSetUp.salaryRange;
                        JobRecord.unionID = JobSetUp.unionID != "0" ? Convert.ToSByte(JobSetUp.unionID) : (short?)null;
                        JobRecord.Notes = JobSetUp.Notes;
                        JobRecord.JobFamilyId = JobSetUp.JobFamilyId != 0 ? JobSetUp.JobFamilyId : (short?)null;
                        JobRecord.CompanyCodeId = Convert.ToSByte(JobSetUp.CompanyCodeID);
                        clientDbContext.Jobs.Add(JobRecord);
                    }
                }
                else
                {
                    if (JobSetUp.JobCode != JobSetUp.hdJobCode)
                    {
                        var jobCodeInDb = clientDbContext.Jobs.Where(x => x.JobCode == JobSetUp.hdJobCode).SingleOrDefault();
                        if (jobCodeInDb != null)
                        {

                            ModelState.AddModelError("", "The Job Code" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                            var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                            foreach (var item in modelStateErrors)
                            {
                                if (_message != "") _message += "<br />";
                                _message += item.ErrorMessage;
                            }
                            return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else {
                        JobRecord.title = JobSetUp.title;
                        JobRecord.JobCode = JobSetUp.JobCode;
                        JobRecord.JobId = JobSetUp.Jobid;
                        JobRecord.JobDescription = JobSetUp.JobDescription;
                        JobRecord.jobClassID = JobSetUp.jobClassID != "0" ? Convert.ToSByte(JobSetUp.jobClassID) : (short?)null; ;
                        JobRecord.workersCompensationID = JobSetUp.workersCompensationID != "0" ? Convert.ToSByte(JobSetUp.workersCompensationID) : (short?)null;
                        JobRecord.eeoJobCodeID = JobSetUp.eeoJobCodeID != "0" ? Convert.ToSByte(JobSetUp.eeoJobCodeID) : (short?)null; ;
                        JobRecord.eeoJobTrainingStatusID = JobSetUp.eeoJobTrainingStatusID != "0" ? Convert.ToSByte(JobSetUp.eeoJobTrainingStatusID) : (short?)null;
                        JobRecord.createdDate = DateTime.Now;
                        JobRecord.endDate = JobSetUp.endDate;
                        JobRecord.FLSAID = JobSetUp.FLSAID != "0" ? Convert.ToSByte(JobSetUp.FLSAID) : (short?)null;
                        JobRecord.enteredBy = JobSetUp.enteredBy;
                        JobRecord.enteredDate = JobSetUp.enteredDate;
                        JobRecord.lastEvaluationDate = JobSetUp.lastEvaluationDate;
                        JobRecord.requirements = JobSetUp.requirements;
                        JobRecord.salaryRange = JobSetUp.salaryRange;
                        JobRecord.unionID = JobSetUp.unionID != "0" ? Convert.ToSByte(JobSetUp.unionID) : (short?)null;
                        JobRecord.Notes = JobSetUp.Notes;
                        JobRecord.JobFamilyId = JobSetUp.JobFamilyId != 0 ? JobSetUp.JobFamilyId : (short?)null;
                        JobRecord.CompanyCodeId = Convert.ToSByte(JobSetUp.CompanyCodeID);
                    }
                }
                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "New Job Record Added" : "Job Record Saved";
                }
                catch (Exception err)
                {
                    IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                    if (errors.Count() == 0)
                    {
                        _message += err.InnerException.InnerException.Message;
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

                foreach (var item in modelStateErrors)
                {
                    if (_message != "") _message += "<br />";
                    _message += item.ErrorMessage;
                }
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { JobSetUp, succeed = true, recordIsNew }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddNewJob(int JobId)
        {
            string connString = User.Identity.GetClientConnectionString();
            JobSetupDetail JobSetupDetailModel = new JobSetupDetail();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            if (JobId == 0)
            {
                JobSetupDetailModel.Jobid = 0;
            }
            else
            {
                JobSetupDetailModel = clientDbContext.Jobs
              .Include(x => x.DdlJobClasses.Description)
              .Include(x => x.DdlWorkersCompensations.Description)
              .Include(x => x.DdlEEOJobCodes.Description)
              .Include(x => x.DdlFLSAs.Description)
              .Include(x => x.DdlEEOJobTrainingStatuses.Description)
              .Include(x => x.DdlUnions.Description)
              .Include(x => x.DdlJobFamilys.Description)
               .Include(x => x.CompanyCode.CompanyCodeDescription)
         .Where(x => x.JobId == JobId)
              .Select(x => new Models.JobSetupDetail
              {
                  Jobid = x.JobId,
                  JobDescription = x.JobDescription,
                  JobCode = x.JobCode,
                  hdJobCode =x.JobCode,
                  title = x.title,
                  jobClassIDDescription = x.DdlJobClasses.Description,
                  jobClassID = x.jobClassID.ToString(),
                  workersCompensationIDDescription = x.DdlWorkersCompensations.Description,
                  workersCompensationID = x.workersCompensationID.ToString(),
                  eeoJobCodeIDDescription = x.DdlEEOJobCodes.Description,
                  eeoJobCodeID = x.eeoJobCodeID.ToString(),
                  eeoJobTrainingStatusIDDescription = x.DdlEEOJobTrainingStatuses.Description,
                  eeoJobTrainingStatusID = x.eeoJobTrainingStatusID.ToString(),
                  CompanyCodeIdDescription = x.CompanyCode.CompanyCodeDescription,
                  CompanyCodeID = x.CompanyCodeId.ToString(),
                  createdDate = x.createdDate,
                  endDate = x.endDate,
                  FLSAIDDescription = x.DdlFLSAs.Description,
                  FLSAID = x.FLSAID.ToString(),
                  enteredBy = x.enteredBy,
                  enteredDate = x.enteredDate,
                  lastEvaluationDate = x.lastEvaluationDate,
                  requirements = x.requirements,
                  salaryRange = x.salaryRange,
                  unionIDDescription = x.DdlUnions.Description,
                  unionID = x.unionID.ToString(),
                  Notes = x.Notes,
                  JobFamilyIdDescription = x.DdlJobFamilys.Description,
                  JobFamilyId = x.JobFamilyId,
              })
              .FirstOrDefault();
            }

            JobSetupDetailModel.UnionsList = clientDbContext.DdlUnions.Where(x => x.Active == true).ToList();
            JobSetupDetailModel.WorkersCompensationsList = clientDbContext.DdlWorkersCompensations.Where(x => x.Active == true).ToList();
            JobSetupDetailModel.FLSAList = clientDbContext.DddlFLSAs.Where(x => x.Active == true).ToList();
            JobSetupDetailModel.EEOJobCodesList = clientDbContext.DdlEEOJobCodes.Where(x => x.Active == true).ToList();
            JobSetupDetailModel.EEOJobTrainingStatusesList = clientDbContext.DdlEEOJobTrainingStatuses.Where(x => x.Active == true).ToList();
            JobSetupDetailModel.JobFamilyList = clientDbContext.DdlJobFamilys.Where(x => x.Active == true).ToList();
            JobSetupDetailModel.JobClassList = clientDbContext.DdlJobClasses.Where(x => x.Active == true).ToList();
            JobSetupDetailModel.CompanyCodeList = clientDbContext.CompanyCodes.Where(x => x.IsCompanyCodeActive == true).ToList();
            JobSetupDDLCleanUp(JobSetupDetailModel);
            return View("AddNewJobSetup", JobSetupDetailModel);
        }

        public void JobSetupDDLCleanUp(JobSetupDetail JobSetupDetail)
        {
            JobSetupDetail.UnionsList.Insert(0, new ddlUnions { unionID = 0, Description = "Select" });
            JobSetupDetail.WorkersCompensationsList.Insert(0, new ddlWorkersCompensations { workersCompensationID = 0, Description = "Select" });
            JobSetupDetail.FLSAList.Insert(0, new ddlFLSAs { FLSAID = 0, Description = "Select" });
            JobSetupDetail.EEOJobCodesList.Insert(0, new ddlEEOJobCodes { eeoJobCodeID = 0, Description = "Select" });
            JobSetupDetail.EEOJobTrainingStatusesList.Insert(0, new ddlEEOJobTrainingStatuses { eeoJobTrainingStatusID = 0, Description = "Select" });
            JobSetupDetail.JobFamilyList.Insert(0, new ddlJobFamilys { JobFamilyId = 0, Description = "Select" });
            JobSetupDetail.JobClassList.Insert(0, new ddlJobClasses { jobClassID = 0, Description = "Select" });
            JobSetupDetail.CompanyCodeList.Insert(0, new CompanyCode { CompanyCodeId = 0, CompanyCodeDescription = "Select" });
        }
        public ActionResult DDLPartialJobSetup(string type)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            dynamic columnnames;
            columnnames = from t in typeof(ddlJobClasses).GetProperties() select t.Name;
            switch (type)
            {
                case "ddlJobFamilys":
                    columnnames = from t in typeof(ddlJobFamilys).GetProperties() select t.Name;
                    ViewBag.Title = "Job Family";
                    break;
                case "ddlJobClasses":
                    columnnames = from t in typeof(ddlJobClasses).GetProperties() select t.Name;
                    ViewBag.Title = "Job Class";
                    break;
                case "ddlEEOJobCodes":
                    columnnames = from t in typeof(ddlEEOJobCodes).GetProperties() select t.Name;
                    ViewBag.Title = "EEO Job Code";
                    break;
                case "ddlUnions":
                    columnnames = from t in typeof(ddlUnions).GetProperties() select t.Name;
                    ViewBag.Title = "Unions";
                    break;
                case "ddlWorkersCompensations":
                    columnnames = from t in typeof(ddlWorkersCompensations).GetProperties() select t.Name;
                    ViewBag.Title = "Worker's Compensation";
                    break;
                case "ddlEEOJobTrainingStatuses":
                    columnnames = from t in typeof(ddlEEOJobTrainingStatuses).GetProperties() select t.Name;
                    ViewBag.Title = "EEO Job Training";
                    break;
                case "CompanyCodes":
                    columnnames = from t in typeof(CompanyCodes).GetProperties() select t.Name;
                    ViewBag.Title = "Company Code";
                    break;
                case "ddlFLSAs":
                    columnnames = from t in typeof(ddlFLSAs).GetProperties() select t.Name;
                    ViewBag.Title = "FLSA";
                    break;
                case "Locations":
                    columnnames = from t in typeof(Location).GetProperties() select t.Name;
                    ViewBag.Title = "Locations";
                    break;

                case "ddlEEOFileStatuses":
                    columnnames = from t in typeof(DdlEEOFileStatuses).GetProperties() select t.Name;
                    ViewBag.Title = "EEO";
                    break;

                case "ddlBusinessLevelTypes":
                    columnnames = from t in typeof(DdlBusinessLevelTypes).GetProperties() select t.Name;
                    ViewBag.Title = "Business Level Type";
                    break;
                default:
                    break;
            }

            List<DDLPartial> DDLPartialList = new List<Models.DDLPartial>();
            foreach (var item in columnnames)
            {

                if (item != "Jobs" && item != "Positions")
                {

                    DDLPartial DDLPartial = new Models.DDLPartial();
                    DDLPartial.FieldID = item;
                    DDLPartial.Caption = item.Contains("description") ? "Description" : item;
                    DDLPartial.FieldValue = null;
                    DDLPartial.DDLName = type;
                    DDLPartialList.Add(DDLPartial);
                }
            }
            Swap(DDLPartialList, 1, 2);
            Session["Ddltype"] = type;
            return View(DDLPartialList);
        }
        public ActionResult DdlSave(FormCollection formCollection)
        {
            string _message = "";
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            if (ModelState.IsValid)
            {
                string type = Session["Ddltype"].ToString();
                string title = formCollection["DDL_TITLE"];
                var returnCode = new SqlParameter();
                returnCode.ParameterName = "@ReturnCode";
                returnCode.SqlDbType = SqlDbType.Int;
                returnCode.Direction = ParameterDirection.Output;
                string Code = formCollection["Code"];
                string Description = formCollection["description"];
                if (type != "CompanyCodes")
                {
                    try
                    {
                        var jobClassesInDb = clientDbContext.Database.ExecuteSqlCommand("exec @ReturnCode = CheckDuplicateDDLData @Code,@Description, @TblName",
                            returnCode,
                               new SqlParameter("@Code", Code),
                               new SqlParameter("@Description", Description),
                               new SqlParameter("@TblName", type));
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Duplicate"))
                        {
                            _message += title + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED;

                        }
                        return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (clientDbContext.CompanyCodes.Where(x => x.CompanyCodeCode == Code || x.CompanyCodeDescription == Description).Count() > 0) 
                    {
                        _message += title + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED;
                        return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                    }
                }
                switch (type)
                {
                    case "ddlJobFamilys":
                        ddlJobFamilys jobfamily = new ddlJobFamilys();
                        jobfamily.Code = formCollection["Code"].ToString();
                        jobfamily.Description = formCollection["description"].ToString();
                        jobfamily.Active = formCollection["Active"].ToString().Equals("true") ? true : false;
                        jobfamily.JobFamilyId = 0;
                        clientDbContext.DdlJobFamilys.Add(jobfamily);
                        break;
                    case "ddlJobClasses":
                        ddlJobClasses JobClasses = new ddlJobClasses();
                        JobClasses.Code = formCollection["Code"].ToString();
                        JobClasses.Description = formCollection["description"].ToString();
                        JobClasses.Active = formCollection["Active"].ToString().Equals("true") ? true : false;
                        JobClasses.jobClassID = 0;
                        clientDbContext.DdlJobClasses.Add(JobClasses);
                        break;
                    case "ddlEEOJobCodes":
                        ddlEEOJobCodes EEOJobCodes = new ddlEEOJobCodes();
                        EEOJobCodes.Code = formCollection["Code"].ToString();
                        EEOJobCodes.Description = formCollection["description"].ToString();
                        EEOJobCodes.Active = formCollection["Active"].ToString().Equals("true") ? true : false;
                        EEOJobCodes.eeoJobCodeID = 0;
                        clientDbContext.DdlEEOJobCodes.Add(EEOJobCodes);
                        break;
                    case "ddlUnions":
                        ddlUnions Unions = new ddlUnions();
                        Unions.Code = formCollection["Code"].ToString();
                        Unions.Description = formCollection["description"].ToString();
                        Unions.unionID = 0;
                        Unions.Active = formCollection["Active"].ToString().Equals("true") ? true : false;
                        clientDbContext.DdlUnions.Add(Unions);
                        break;
                    case "ddlWorkersCompensations":
                        ddlWorkersCompensations WorkersCompensations = new ddlWorkersCompensations();
                        WorkersCompensations.Code = formCollection["Code"].ToString();
                        WorkersCompensations.Description = formCollection["description"].ToString();
                        WorkersCompensations.Active = formCollection["Active"].ToString().Equals("true") ? true : false;
                        WorkersCompensations.workersCompensationID = 0;
                        clientDbContext.DdlWorkersCompensations.Add(WorkersCompensations);
                        break;
                    case "ddlEEOJobTrainingStatuses":
                        ddlEEOJobTrainingStatuses EEOJobTrainingStatuses = new ddlEEOJobTrainingStatuses();
                        EEOJobTrainingStatuses.Code = formCollection["Code"].ToString();
                        EEOJobTrainingStatuses.Description = formCollection["Description"].ToString();
                        EEOJobTrainingStatuses.Active = formCollection["Active"].ToString().Equals("true") ? true : false;
                        EEOJobTrainingStatuses.eeoJobTrainingStatusID = 0;
                        clientDbContext.DdlEEOJobTrainingStatuses.Add(EEOJobTrainingStatuses);
                        break;
                    case "CompanyCodes":
                        CompanyCode CompanyCode = new CompanyCode();
                        CompanyCode.CompanyCodeCode = formCollection["Code"];
                        CompanyCode.CompanyCodeDescription = formCollection["Description"];
                        CompanyCode.CompanyCodeId = 0;
                        CompanyCode.IsCompanyCodeActive = formCollection["Active"].Equals("true") ? true : false;
                        clientDbContext.CompanyCodes.Add(CompanyCode);
                        break;
                    case "ddlFLSAs":
                        ddlFLSAs FLSAs = new ddlFLSAs();
                        FLSAs.Code = formCollection["Code"].ToString();
                        FLSAs.Description = formCollection["description"].ToString();
                        FLSAs.FLSAID = 0;
                        FLSAs.Active = formCollection["Active"].ToString().Equals("true") ? true : false;
                        clientDbContext.DddlFLSAs.Add(FLSAs);
                        break;
                    case "Locations":
                        Location Locations = new Location();
                        Locations.LocationCode = formCollection["LocationCode"].ToString();
                        Locations.LocationDescription = formCollection["LocationDescription"].ToString();
                        Locations.Active = formCollection["Active"].ToString().Equals("true") ? true : false;
                        clientDbContext.Locations.Add(Locations);
                        break;

                    case "ddlEEOFileStatuses":
                        DdlEEOFileStatuses DdlEEOFileStatuses = new DdlEEOFileStatuses();
                        DdlEEOFileStatuses.Code = formCollection["Code"].ToString();
                        DdlEEOFileStatuses.Description = formCollection["Description"].ToString();
                        DdlEEOFileStatuses.Active = formCollection["Active"].ToString().Equals("true") ? true : false;
                        clientDbContext.DdlEEOFileStatuses.Add(DdlEEOFileStatuses);
                        break;
                    case "ddlBusinessLevelTypes":
                        DdlBusinessLevelTypes DdlBusinessLevelTypes = new DdlBusinessLevelTypes();
                        DdlBusinessLevelTypes.Code = formCollection["Code"].ToString();
                        DdlBusinessLevelTypes.Description = formCollection["Description"].ToString();
                        DdlBusinessLevelTypes.Active = formCollection["Active"].ToString().Equals("true") ? true : false;
                        clientDbContext.DdlBusinessLevelTypes.Add(DdlBusinessLevelTypes);
                        break;
                    default:
                        break;
                }

                try
                {
                    clientDbContext.SaveChanges();


                }
                catch (Exception err)
                {
                    IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                    if (errors.Count() == 0)
                    {
                        _message += err.InnerException.InnerException.Message;
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

                foreach (var item in modelStateErrors)
                {
                    if (_message != "") _message += "<br />";
                    _message += item.ErrorMessage;
                }
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }
        public ActionResult ReloadDDL(string DDLName)
        {
            string data = "";

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            dynamic GetList = null;


            switch (DDLName.ToLower())
            {
                case "ddljobclasses":
                    GetList = clientDbContext.DdlJobClasses.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();
                    break;
                case "ddljobfamilys":
                    GetList = clientDbContext.DdlJobFamilys.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();
                    break;
                case "ddleeojobcodes":
                    GetList = clientDbContext.DdlEEOJobCodes.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();
                    break;
                case "ddlunions":
                    GetList = clientDbContext.DdlUnions.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();
                    break;
                case "ddlworkerscompensations":
                    GetList = clientDbContext.DdlWorkersCompensations.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();
                    break;
                case "ddleeojobtrainingstatuses":
                    GetList = clientDbContext.DdlEEOJobTrainingStatuses.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();
                    break;
                case "companycodes":
                    GetList = clientDbContext.CompanyCodes.Where(x => x.IsCompanyCodeActive == true).OrderBy(e => e.CompanyCodeDescription).ToList();
                    break;
                case "ddlflsas":
                    GetList = clientDbContext.DddlFLSAs.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();
                    break;
                case "locations":
                    GetList = clientDbContext.Locations.Where(x => x.Active == true).OrderBy(e => e.LocationDescription).ToList();
                    break;
                case "ddleeofilestatuses":
                    GetList = clientDbContext.DdlEEOFileStatuses.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();
                    break;

                case "ddlbusinessleveltypes":
                    GetList = clientDbContext.DdlBusinessLevelTypes.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();
                    break;
                default:
                    break;
            }
            return Json(GetList, JsonRequestBehavior.AllowGet);
        }
    }
}

