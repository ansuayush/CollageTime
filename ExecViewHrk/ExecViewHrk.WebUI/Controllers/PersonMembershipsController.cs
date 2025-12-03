using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Newtonsoft.Json;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PersonMembershipsController : Controller
    {
        // GET: PersonMemberships
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PersonMembershipList_Read([DataSourceRequest]DataSourceRequest request)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string connString = User.Identity.GetClientConnectionString();
            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees")) throw new Exception("Client Employee trying to access NSS.");

            if (requestType != "IsSelfService") SessionStateHelper.CheckForPersonSelectedValue();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name)
                .Select(x => x.PersonId).SingleOrDefault() : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personMembersList = clientDbContext.PersonMemberships
                .Include(x => x.DdlProfessionalBody)
                .Include(x => x.Person)
                .Where(x => x.PersonId == personId)
                                .Select(x => new PersonMembershipVm
                                {
                                    PersonMembershipId = x.PersonMembershipId,
                                    PersonId = x.PersonId,
                                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                                    ProfessionalBodyId = x.ProfessionalBodyId,
                                    ProfessionalBodyDescription = x.DdlProfessionalBody.Description,
                                    RegionalChapterDescription=x.DdlRegionalChapter.Description,
                                    FeePaidDate = x.FeePaidDate,
                                    Fee = x.Fee,
                                    ProfessionalTitle = x.ProfessionalTitle,
                                    RegionalChapterId = x.RegionalChapterId,
                                    StartDate = x.StartDate,
                                    Number = x.Number,
                                    RenewalDate=x.RenewalDate,
                                    EnteredBy = x.EnteredBy,
                                    EnteredDate = x.EnteredDate
                                }).ToList();

            return Json(personMembersList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonMembershipsDetail(int personMembershipId, int personId)
        {
            return View(GetPersonMembershipsRecord(personMembershipId, personId));
        }
        public PartialViewResult PersonMembershipsMatrixPartial(bool isSelectedIndex = false, int personMembershipIdParam = 0)
        {
            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees"))
                throw new Exception("Client Employee trying to access NSS.");

            if (requestType != "IsSelfService")
                SessionStateHelper.CheckForPersonSelectedValue();

            string connString = User.Identity.GetClientConnectionString();

            ClientDbContext clientDbContext = new ClientDbContext(connString);

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons
                    .Where(x => x.eMail == User.Identity.Name)
                    .Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            if ((personId == 0) && requestType == "IsSelfService")
                throw new Exception("Self service person id is 0 - Personal record cannot be displayed. It doesn't exist.");

            PersonMembershipVm personMembershipVm;

            if (isSelectedIndex && personMembershipIdParam > 0)
            {
                personMembershipVm = GetPersonMembershipsRecord(personMembershipIdParam, personId);
            }
            else
            {
                int personMembershipId = clientDbContext.PersonMemberships.Where(x => x.PersonId == personId).Select(x => x.PersonMembershipId).FirstOrDefault();
                if (personMembershipId == 0)
                {
                    personMembershipVm = new PersonMembershipVm();
                    personMembershipVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    personMembershipVm.PersonId = personId;
                }
                else
                    personMembershipVm = GetPersonMembershipsRecord(personMembershipId, personId);
            }
            return PartialView(personMembershipVm);
        }


        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedMembershipsAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonMembershipVm personMembershipVm;
            int personMembershipId = clientDbContext.PersonMemberships.Where(x => x.PersonId == personId).Select(x => x.PersonMembershipId).FirstOrDefault();
            if (personMembershipId == 0)
            {
                personMembershipVm = new PersonMembershipVm();
                personMembershipVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personMembershipVm.PersonId = personId;
            }
            else
                personMembershipVm = GetPersonMembershipsRecord(personMembershipId, personId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personMembershipVm, JsonRequestBehavior.AllowGet);
        }

        public PersonMembershipVm GetPersonMembershipsRecord(int personMembershipId, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonMembershipVm personMembershipVm = new PersonMembershipVm();
            if (personMembershipId != 0)
            {
                personMembershipVm = clientDbContext.PersonMemberships
                .Include(x => x.DdlProfessionalBody.Description)
                .Include(x => x.DdlRegionalChapter.Description)
                .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonMembershipId == personMembershipId)
                .Select(x => new PersonMembershipVm
                {
                    PersonMembershipId = x.PersonMembershipId,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                    ProfessionalBodyId = x.ProfessionalBodyId,
                    ProfessionalBodyDescription = x.DdlProfessionalBody.Description,
                    StartDate = x.StartDate,
                    RenewalDate = x.RenewalDate,
                    Number = x.Number,
                    Fee = x.Fee,
                    FeePaidDate = x.FeePaidDate,
                    ProfessionalTitle = x.ProfessionalTitle,
                    RegionalChapterId = x.RegionalChapterId,
                    RegionalChapterDescription = x.DdlRegionalChapter.Description,
                    EnteredBy = x.EnteredBy,
                    EnteredDate = x.EnteredDate,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedDate = x.ModifiedDate,
                    Notes = x.Notes,
                })
                .FirstOrDefault();
            }
            personMembershipVm.PersonId = personId;
            personMembershipVm.PersonMembershipId = 0;
            personMembershipVm.PersonName = clientDbContext.Persons.Where(x => x.PersonId == personId).Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();
            personMembershipVm.ProfessionalBodyDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetProfessionalBodyList());
            personMembershipVm.RegionalChapterDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetRegionalChapterList());
            return personMembershipVm;
        }

        public string GetProfessionalBodyList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var ProfessionBodyList = clientDbContext.DdlProfessionalBodies.Where(x => x.Active == true).Select(m => new DropDownModel { keyvalue = m.ProfessionalBodyId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList().CleanUp();
            return JsonConvert.SerializeObject(ProfessionBodyList);
        }
        public string GetRegionalChapterList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var RegionalChapterList = clientDbContext.DdlRegionalChapters.Where(x => x.Active == true).Select(m => new DropDownModel { keyvalue = m.RegionalChapterId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList().CleanUp();
            return JsonConvert.SerializeObject(RegionalChapterList);
        }
        
        public ActionResult PersonMembershipsIndexChangedAjax(int personMembershipIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);           
            PersonMembershipVm personMembershipVm;

            if (personMembershipIdDdl < 1)
            {
                personMembershipVm = new PersonMembershipVm();
                personMembershipVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personMembershipVm.PersonId = personId;
                personMembershipVm.PersonMembershipId = 0;
            }
            else
                personMembershipVm = GetPersonMembershipsRecord(personMembershipIdDdl, personId);

            ModelState.Clear();

            return Json(personMembershipVm, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult PersonMembershipsSaveAjax(PersonMembershipVm personMembershipVm)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personMembershipVm.PersonMembershipId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Membership record is for.");
                    return View(personMembershipVm);
                }
                else
                {
                    personMembershipVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            if (ModelState.IsValid)
            {
                PersonMembership personMembership = clientDbContext.PersonMemberships.Include(x => x.Person)
                    .Where(x => x.PersonMembershipId == personMembershipVm.PersonMembershipId).SingleOrDefault();

                if (personMembershipVm.ProfessionalBodyId != 0)
                {
                    if (personMembershipVm.PersonMembershipId != 0)
                    {
                        var isRecordExists = clientDbContext.PersonMemberships.Where(x => x.ProfessionalBodyId == personMembershipVm.ProfessionalBodyId
                                                                                && x.RegionalChapterId == personMembershipVm.RegionalChapterId
                                                                                && x.PersonId == personMembershipVm.PersonId
                        ).Select(a => a.PersonMembershipId).SingleOrDefault();


                        if (personMembership == null && isRecordExists != 0)
                        {
                            return Json(new { succeed = false, Message = "The Membership record  exists for the selected start date." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (personMembershipVm.PersonMembershipId > 0)
                    {
                        var isSameMemberExists = clientDbContext.PersonMemberships.Where(x => x.PersonMembershipId != personMembershipVm.PersonMembershipId
                                                            && x.StartDate == personMembershipVm.StartDate
                                                            && x.ProfessionalBodyId == personMembershipVm.ProfessionalBodyId
                                                            && x.PersonId == personMembershipVm.PersonId
                        ).Select(a => a.PersonMembershipId).SingleOrDefault();

                        if (isSameMemberExists != 0)
                        {
                            return Json(new { succeed = false, Message = "The Membership record  exists for start date." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (personMembership == null)
                    {
                        personMembership = new PersonMembership();
                        personMembership.EnteredBy = User.Identity.Name;
                        personMembership.EnteredDate = DateTime.Now;
                        personMembership.PersonId = personMembershipVm.PersonId;
                        recordIsNew = true;
                    }
                    personMembership.ProfessionalBodyId = personMembershipVm.ProfessionalBodyId;

                    if (!string.IsNullOrEmpty(personMembershipVm.ProfessionalBodyDescription))
                    {
                        int professionalBodyInDb = clientDbContext.DdlProfessionalBodies
                            .Where(x => x.Description == personMembershipVm.ProfessionalBodyDescription).Select(x => x.ProfessionalBodyId).SingleOrDefault();

                        if (professionalBodyInDb <= 0)
                        {
                            ModelState.AddModelError("", "The professional body does not exist.");
                            return Json(new { succeed = false, Message = "The professional body does not exist." }, JsonRequestBehavior.AllowGet);
                        }
                        else personMembership.ProfessionalBodyId = professionalBodyInDb;
                    }
                    if (!string.IsNullOrEmpty(personMembershipVm.RegionalChapterDescription))
                    {
                        var regionalChapterInDb = clientDbContext.DdlRegionalChapters
                            .Where(x => x.Description == personMembershipVm.RegionalChapterDescription).Select(x => x.RegionalChapterId).SingleOrDefault();

                        if (regionalChapterInDb <= 0)
                        {
                            ModelState.AddModelError("", "The regional chapter does not exist.");
                            return Json(new { succeed = false, Message = "The regional chapter does not exist." }, JsonRequestBehavior.AllowGet);
                        }
                        else personMembership.RegionalChapterId = regionalChapterInDb;
                    }
                    if (!string.IsNullOrEmpty(personMembershipVm.Number.Trim()))
                    {
                        var numberList = clientDbContext.PersonMemberships.Where(n => n.Number == personMembershipVm.Number && n.PersonMembershipId != personMembershipVm.PersonMembershipId).ToList();

                        if (numberList.Count > 0)
                        {
                            return Json(new { succeed = false, Message = "The entered number already exists!" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                personMembership.ProfessionalBodyId = personMembershipVm.ProfessionalBodyId;
                personMembership.RegionalChapterId = personMembershipVm.RegionalChapterId;
                personMembership.Notes = personMembershipVm.Notes;
                personMembership.StartDate = personMembershipVm.StartDate;
                personMembership.RenewalDate = personMembershipVm.RenewalDate;
                personMembership.Number = personMembershipVm.Number;
                personMembership.Fee = personMembershipVm.Fee;
                personMembership.FeePaidDate = personMembershipVm.FeePaidDate;
                personMembership.ProfessionalTitle = personMembershipVm.ProfessionalTitle;

                if (recordIsNew)
                    clientDbContext.PersonMemberships.Add(personMembership);
                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "New Person Ada Record Added" : "Person Ada Record Updated";
                    personMembershipVm = GetPersonMembershipsRecord(personMembership.PersonMembershipId, personMembership.PersonId);
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
                _message += "\n\nRecord could not be updated at this time.";
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { succeed = true, personMembershipVm }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PersonMembershipsDeleteAjax(int personMembershipIdDdl, int personId)
        {
            if (personMembershipIdDdl < 1)
                return Json("The person membership record does not exist", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            clientDbContext.Database.ExecuteSqlCommand(" " +
                                "DELETE FROM PersonMemberships " +
                                "WHERE PersonMembershipId = @PersonMembershipId ",
                                new SqlParameter("@PersonMembershipId", personMembershipIdDdl));

            PersonMembershipVm personMembershipVm;
            int personMembershipId = clientDbContext.PersonMemberships.Where(x => x.PersonId == personId).Select(x => x.PersonMembershipId).FirstOrDefault();
            if (personMembershipId < 1)
            {
                personMembershipVm = new PersonMembershipVm();
                personMembershipVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personMembershipVm.PersonId = personId;
                personMembershipVm.PersonMembershipId = 0;
            }
            else
                personMembershipVm = GetPersonMembershipsRecord(personMembershipId, personId);

            ModelState.Clear();
            return Json(personMembershipVm, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPersonMembershipsList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personMembershipList = clientDbContext.PersonMemberships.Include(x => x.DdlProfessionalBody.Description)
                .Where(x => x.PersonId == personId)
                .Select(m => new
                {
                    PersonMembershipId = m.PersonMembershipId,
                    PersonMembershipDescription = m.DdlProfessionalBody.Description
                }).OrderBy(x => x.PersonMembershipDescription).ToList();

            return Json(personMembershipList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonMembershipsDelete(int personMembershipId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.PersonMemberships.Where(x => x.PersonMembershipId == personMembershipId).SingleOrDefault();
            if (dbRecord != null)
            {
                clientDbContext.PersonMemberships.Remove(dbRecord);
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
    }
}