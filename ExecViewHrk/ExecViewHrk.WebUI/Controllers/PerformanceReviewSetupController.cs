using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Infrastructure;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PerformanceReviewSetupController : Controller
    {
        IPerformanceRepository _peformanceRepository;
        string _message = "";
        public PerformanceReviewSetupController(IPerformanceRepository peformanceRepository)
        {
            _peformanceRepository = peformanceRepository;
        }
        // GET: PerformanceReview
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PerformanceReviewSetupList()
        {
            return View();
        }
        public ActionResult PerformanceReviewSetup_Read([DataSourceRequest]DataSourceRequest request, int perProfileId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string connString = User.Identity.GetClientConnectionString();
            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees")) throw new Exception("Client Employee trying to access NSS.");

            if (requestType != "IsSelfService") SessionStateHelper.CheckForPersonSelectedValue();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name)
                .Select(x => x.PersonId).SingleOrDefault() : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
            var perfomanceList = new List<PerformanceReviewSetupVm>();
            try
            {
                perfomanceList = clientDbContext.PerformanceProfilesSections.Where(m => m.PerProfileID == perProfileId)
                                   .Select(x => new PerformanceReviewSetupVm
                                   {
                                       ID = x.ID,
                                       SectionName = x.SectionName,
                                       Header = x.Header,
                                       NumRows = x.NumRows,
                                       MaxCharacters = x.MaxCharacters,
                                       ProfileID = x.PerProfileID,
                                       Weight = x.Weight,
                                       Position = x.Position,
                                   }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(perfomanceList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult PerformanceReviewSetupDetail(int PerProfileId)
        {
            return View(GetPerformanceReviewRecord(PerProfileId));
        }
        public PerformanceReviewSetupVm GetPerformanceReviewRecord(int PerProfileId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PerformanceReviewSetupVm performanceReviewVm = new PerformanceReviewSetupVm();
            try
            {
                if (PerProfileId != 0)
                {
                    performanceReviewVm = clientDbContext.PerformanceProfiles
                        .Where(m => m.PerProfileID == PerProfileId)
                        .Select(x => new PerformanceReviewSetupVm
                        {
                            PerProfileId = x.PerProfileID,
                            Code = x.Code,
                            Description = x.Description,
                            Active = x.Active
                        }).FirstOrDefault();
                }
                ViewBag.PerProfileID = PerProfileId;
                performanceReviewVm.PerformanceProfileList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPerformanceProfileList());
                ViewBag.PerformanceList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPerformanceProfileList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return performanceReviewVm;
        }
        public string GetPerformanceProfileList()
        {
            var performanceList = new List<DropDownModel>();
            try
            {
                performanceList = _peformanceRepository.GetPerformanceList().CleanUp();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return JsonConvert.SerializeObject(performanceList);
        }
        public ActionResult AddPerformanceProfile()
        {
            var model = new DdlPerformancePotentials() { Active = true };
            return View(model);
        }
        [HttpPost]
        public ActionResult Performance_Create(PerformanceReviewSetupVm performanceList)
        {
            string constring = User.Identity.GetClientConnectionString();
            bool recordIsNew = false;
            using (var clientDbContext = new ClientDbContext(constring))
            {
                if (performanceList != null && ModelState.IsValid)
                {
                    if (performanceList.PerProfileId != 0)
                    {
                        var allrecords = clientDbContext.PerformanceProfiles.Where(m => m.PerProfileID != performanceList.PerProfileId).ToList();
                        foreach (var item in allrecords)
                        {
                            if (item.Code == performanceList.Code || item.Description == performanceList.Description)
                            {
                                return Json(new { succeed = false, Message = "Unable to update due to Code or Description already exists for another profile." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {
                        var isRecordExists = clientDbContext.PerformanceProfiles.Where(x => (x.Code == performanceList.Code || x.Description == performanceList.Description))
                            .Select(a => a.PerProfileID).SingleOrDefault();
                        if (isRecordExists != 0)
                        {
                            return Json(new { succeed = false, Message = "The Code or Description for this performance profile record already exists." }, JsonRequestBehavior.AllowGet);
                        }
                        recordIsNew = true;
                    }
                    var newPerformanceDetails = new PerformanceProfiles();
                    try
                    {
                        if (!string.IsNullOrEmpty(performanceList.Code) && !string.IsNullOrEmpty(performanceList.Description))
                        {
                            newPerformanceDetails = new PerformanceProfiles
                            {
                                Code = performanceList.Code,
                                Description = performanceList.Description,
                                Active = performanceList.Active
                            };
                            performanceList.Code = newPerformanceDetails.Code;
                            performanceList.Description = newPerformanceDetails.Description;
                            performanceList.Active = newPerformanceDetails.Active;
                            if (recordIsNew)
                            {
                                clientDbContext.PerformanceProfiles.Add(newPerformanceDetails);
                                clientDbContext.SaveChanges();
                                performanceList.PerProfileId = newPerformanceDetails.PerProfileID;
                            }
                            else
                            {
                                newPerformanceDetails.PerProfileID = performanceList.PerProfileId;
                                clientDbContext.Database.ExecuteSqlCommand("Update [PerformanceProfiles]" +
                                    " SET Code = @Code,Description=@Description,Active=@Active where PerProfileID=@PerProfileID",
                                    new SqlParameter("@Code", performanceList.Code), new SqlParameter("@Description", performanceList.Description),
                                   new SqlParameter("@Active", performanceList.Active), new SqlParameter("@PerProfileID", performanceList.PerProfileId));
                                clientDbContext.SaveChanges();
                            }
                            ViewBag.AlertMessage = recordIsNew == true ? "New Performance Profile Record Added" : "Performance Profile Record Saved";
                            performanceList = GetPerformanceReviewRecord(performanceList.PerProfileId);
                        }
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
                    _message += "\n\nRecord could not be saved at this time.";
                    return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { performanceReviewVm = performanceList, succeed = true }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult PerformanceProfileSaveAjax(int PerProfileID, string SectionName, string Description, int NumRows, int MaxCharacters, decimal Weight, int Position)
        {
            string returnmsg = "";
            string connString = User.Identity.GetClientConnectionString();
            try
            {
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    if (PerProfileID != 0)
                    {
                        var isRecordExists = clientDbContext.PerformanceProfilesSections.Where(x => (x.SectionName == SectionName && x.Position == Position))
                            .Select(a => a.PerProfileID).SingleOrDefault();
                        if (isRecordExists != 0)
                        {
                            return Json(new { succeed = false, Message = "The performance review Setup record  exists for the selected performance profile." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    var newitem = new PerformanceProfileSections
                    {
                        PerProfileID = PerProfileID,
                        SectionName = SectionName,
                        Header = Description,
                        NumRows = 1,
                        MaxCharacters = 200,
                        Weight = Weight,
                        Position = Position,
                        EnteredBy = User.Identity.Name,
                        EnteredDate = DateTime.Now
                    };
                    clientDbContext.PerformanceProfilesSections.Add(newitem);
                    clientDbContext.SaveChanges();
                }
            }
            catch (System.Exception ex) { returnmsg = ex.ToString(); }
            return RedirectToAction("PerformanceReviewSetup_Read");
        }
        public ActionResult PerformanceProfileList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            {
                try
                {
                    var performanceprofile = clientDbContext.PerformanceProfiles
                        .Select(e => new PerformanceProfileVm
                        {
                            PerProfileId = e.PerProfileID,
                            Code = e.Code,
                            Description = e.Description,
                            Active = e.Active,
                        });
                    return Json(performanceprofile.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public PartialViewResult EditPerformanceReviewSetup(int Id)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            {
                var performanceList = clientDbContext.PerformanceProfilesSections
                    .Where(x => x.ID == Id).ToList()
                    .Select(e => new PerformanceReviewSetupVm
                    {
                        ID = e.ID,
                        SectionName = e.SectionName,
                        Header = e.Header,
                        ProfileID = e.PerProfileID,
                        Weight = e.Weight,
                        Position = e.Position
                    }).FirstOrDefault();
                TempData["PerformanceId"] = Id;
                ViewBag.PerProfileId = performanceList.ProfileID;
                performanceList.PerformanceProfileList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPerformanceProfileList());
                ViewBag.PerformanceList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPerformanceProfileList());
                return PartialView("EditPerformanceProfile", performanceList);
            }
        }
        [HttpPost]
        public ActionResult updatePerformanceProfileReview(int PerProfileID, string SectionName, string Description, int NumRows, int MaxCharacters, decimal Weight, int Position)
        {
            string returnmsg = "";
            string connString = User.Identity.GetClientConnectionString();
            var edititem = new PerformanceProfileSections
            {
                SectionName = SectionName,
                Header = Description,
                Weight = Weight,
                PerProfileID = PerProfileID,
                Position = Position,
                ModifiedBy = User.Identity.Name,
                ModifiedDate = DateTime.Now
            };
            try
            {
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    clientDbContext.Database.ExecuteSqlCommand("Update [PerformanceProfileSections] SET SectionName = @SectionName,Header=@Header,PerProfileID=@PerProfileID," +
                        "NumRows=@NumRows,MaxCharacters=@MaxCharacters,Weight=@Weight,Position=@Position,ModifiedBy=@ModifiedBy,ModifiedDate=@ModifiedDate " +
                        "WHERE ID = @ID", new SqlParameter("@SectionName", SectionName), new SqlParameter("@Header", Description), new SqlParameter("@PerProfileID", PerProfileID),
                        new SqlParameter("@NumRows", NumRows), new SqlParameter("@MaxCharacters", MaxCharacters), new SqlParameter("@Weight", Weight), new SqlParameter("@Position", Position),
                       new SqlParameter("@ModifiedBy", User.Identity.Name), new SqlParameter("@ModifiedDate", DateTime.Now), new SqlParameter("@ID", Convert.ToInt32(TempData["PerformanceId"])));
                    clientDbContext.SaveChanges();
                }
            }
            catch (Exception ex) { returnmsg = ex.ToString(); }
            return RedirectToAction("PerformanceReviewSetup_Read");
        }
        public ActionResult PerProfileDelete(int Id)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.PerformanceProfilesSections.Where(x => x.ID == Id).FirstOrDefault();
            if (dbRecord != null)
            {
                clientDbContext.PerformanceProfilesSections.Remove(dbRecord);
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
            return Json(new { Message = "Record deleted successfully!", succeed = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PerformanceProfileDelete(int PerProfileId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.PerformanceProfiles.Where(x => x.PerProfileID == PerProfileId).FirstOrDefault();
            var dbperfRecord = clientDbContext.PerformanceProfilesSections.Where(x => x.PerProfileID == PerProfileId).FirstOrDefault();

            if (dbRecord != null)
            {
                if (dbperfRecord != null)
                {
                    clientDbContext.PerformanceProfilesSections.Remove(dbperfRecord);
                    clientDbContext.SaveChanges();
                }
                clientDbContext.PerformanceProfiles.Remove(dbRecord);
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
            return Json(new { Message = "Record deleted successfully!", succeed = true }, JsonRequestBehavior.AllowGet);
        }

    }
}
