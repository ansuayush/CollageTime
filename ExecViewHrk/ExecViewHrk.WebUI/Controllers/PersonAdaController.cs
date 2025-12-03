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
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PersonAdaController : Controller
    {
        // GET: PersonAda
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ADAList()
        {
            return View("PersonADAList");
        }
        public ActionResult PersonADAList_Read([DataSourceRequest]DataSourceRequest request)
        {
            if (User.Identity.GetRequestType() == null)
            {
                return RedirectToActionPermanent("Login", "Account");
            }

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
            if ((personId == 0) && (requestType == "IsSelfService"))
                throw new Exception("Self service person id is 0 - Personal record cannot be displayed. It doesn't exist.");


            var PersonADAData = GetPersonAdaData(personId);
            return Json(PersonADAData.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonAdaMatrixPartial(bool isSelectedIndex = false, int personAdaIdParam = 0)
        {
            if (User.Identity.GetRequestType() == null)
            {
                return RedirectToActionPermanent("Login", "Account");
            }

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

            if ((personId == 0) && (requestType == "IsSelfService"))
                throw new Exception("Self service person id is 0 - Personal record cannot be displayed. It doesn't exist.");

            PersonAdaVm personAdaVm;

            if (isSelectedIndex && personAdaIdParam > 0)
            {
                personAdaVm = GetPersonAdaRecord(personAdaIdParam);
            }
            else
            {
                int personAdaId = clientDbContext.PersonADAs.Where(x => x.PersonId == personId).Select(x => x.PersonAdaId).FirstOrDefault();
                if (personAdaId == 0)
                {
                    personAdaVm = new PersonAdaVm();
                    personAdaVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    personAdaVm.PersonId = personId;
                }
                else
                    personAdaVm = GetPersonAdaRecord(personAdaId);
            }


            return PartialView(personAdaVm);
        }

        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedAdaAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonAda() action; drop down on top left of employment screen
            PersonAdaVm personAdaVm;
            int personAdaId = clientDbContext.PersonADAs.Where(x => x.PersonId == personId).Select(x => x.PersonAdaId).FirstOrDefault();
            if (personAdaId == 0)
            {
                personAdaVm = new PersonAdaVm();
                personAdaVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personAdaVm.PersonId = personId;
            }
            else
                personAdaVm = GetPersonAdaRecord(personAdaId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personAdaVm, JsonRequestBehavior.AllowGet);
        }

        public PersonAdaVm GetPersonAdaRecord(int personAdaId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonAdaVm personAdaVm = clientDbContext.PersonADAs
                .Include(x => x.DdlAccommodationType.Description)
                .Include(x => x.DdlDisabilityType.Description)
                .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonAdaId == personAdaId)
                .Select(x => new PersonAdaVm
                {
                    PersonAdaId = x.PersonAdaId,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                    AccommodationProvided = x.AccommodationProvided,
                    AccommodationTypeId = x.AccommodationTypeId,
                    AccommodationDescription = x.DdlAccommodationType.Description,
                    ActualCost = x.ActualCost,
                    AssociatedDisabilityTypeId = x.AssociatedDisabilityId,
                    AssociatedDisabilityDescription = x.DdlDisabilityType.Description,
                    EstimatedCost = x.EstimatedCost,
                    EnteredBy = x.EnteredBy,
                    EnteredDate = x.EnteredDate,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedDate = x.ModifiedDate,
                    Notes = x.Notes,
                    ProvidedDate = x.ProvidedDate,
                    RequestedDate = x.RequestedDate
                })
                .FirstOrDefault();

            return personAdaVm;
        }

        // the record for the same person has changed ; drop down on top right of ADA record
        public ActionResult PersonAdaIndexChangedAjax(int personAdaIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            //// this code is duplicated from get PersonAda() action
            PersonAdaVm personAdaVm;

            if (personAdaIdDdl < 1)
            {
                personAdaVm = new PersonAdaVm();
                personAdaVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personAdaVm.PersonId = personId;
                personAdaVm.PersonAdaId = 0;
            }
            else
                personAdaVm = GetPersonAdaRecord(personAdaIdDdl);

            ModelState.Clear();

            return Json(personAdaVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PersonAdaSaveAjax(PersonAdaVm personAdaVm)
        {
            string _message = "";
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personAdaVm.PersonAdaId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the ADA record is for.");
                    return Json(new { succeed = false, Message = "Cannot determine the id of the person the ADA record is for." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    personAdaVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            if (ModelState.IsValid)
            {
                PersonADA personAda = clientDbContext.PersonADAs.Include(x => x.Person)
                    .Where(x => x.PersonAdaId == personAdaVm.PersonAdaId).SingleOrDefault();

                if (personAda == null)
                {
                    personAda = new PersonADA();
                    personAda.EnteredBy = User.Identity.Name;
                    personAda.EnteredDate = DateTime.UtcNow;
                    personAda.PersonId = personAdaVm.PersonId;
                    recordIsNew = true;
                }
                else
                {
                    personAda.ModifiedBy = User.Identity.Name;
                    personAda.ModifiedDate = DateTime.UtcNow;
                }

                if (!string.IsNullOrEmpty(personAdaVm.AccommodationDescription))//(!short.TryParse(personAdaVm.AccommodationDescription, out accomId))
                {
                    int accomInDb = clientDbContext.DdlAccommodationTypes
                        .Where(x => x.Description == personAdaVm.AccommodationDescription).Select(x => x.AccommodationTypeId).SingleOrDefault();

                    if (accomInDb <= 0)
                    {
                        ModelState.AddModelError("", "The Accommodation Description field is required.");
                        return Json(new { succeed = false, Message = "The Accommodation Description field is required." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        personAda.AccommodationTypeId = accomInDb;
                }
            
                if (!string.IsNullOrEmpty(personAdaVm.AssociatedDisabilityDescription))
                {
                    int disabilityInDb = clientDbContext.DdlDisabilityTypes
                        .Where(x => x.Description == personAdaVm.AssociatedDisabilityDescription).Select(x => x.DisabilityTypeId).SingleOrDefault();

                    if (disabilityInDb <= 0)
                    {
                        return Json(new { succeed = false, Message = "The Associated Disability Description field is required." }, JsonRequestBehavior.AllowGet);
                    }
                    else personAda.AssociatedDisabilityId = disabilityInDb;
                }


                personAda.Notes = personAdaVm.Notes;
                personAda.AccommodationTypeId = personAdaVm.AccommodationTypeId;
                personAda.AssociatedDisabilityId = personAdaVm.DisabilityTypeId;
                personAda.AccommodationProvided = personAdaVm.AccommodationProvided;
                personAda.ActualCost = personAdaVm.ActualCost;
                personAda.EstimatedCost = personAdaVm.EstimatedCost;
                personAda.ProvidedDate = personAdaVm.ProvidedDate;
                personAda.RequestedDate = personAdaVm.RequestedDate;

                if (recordIsNew)
                    clientDbContext.PersonADAs.Add(personAda);

                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "New Person Ada Record Added" : "Person Ada Record Saved";
                    personAdaVm = GetPersonAdaRecord(personAda.PersonAdaId); // refresh the view model
                }
                catch (Exception err)
                {
                    ViewBag.AlertMessage = "";

                    //  ModelState.AddModelError("", CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    _message += "Record with same Accomodation Type and Associated Disability already exist.";

                    IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                    if (errors.Count() == 0)
                        ModelState.AddModelError("", err.InnerException.Message);
                    else
                    {
                        foreach (DbEntityValidationResult error in errors)
                        {
                            foreach (var valError in error.ValidationErrors)
                            {
                                // ModelState.AddModelError("", valError.ErrorMessage);
                                if (_message != "") _message += "\n";
                                _message += valError.ErrorMessage;
                            }
                        }
                    }
                    return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                //var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                //ModelState.AddModelError("", "The record has been altered on transfer and could not be saved at this time.");
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);

                foreach (var item in modelStateErrors)
                {
                    if (_message != "") _message += "\n";
                    _message += item.ErrorMessage;
                }
                _message += "\n\nRecord could not be save at this time.";
                return Json(new { Message = _message, succeed = false, recordIsNew }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { personAdaVm, succeed = true, recordIsNew }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonAdaDeleteAjax(int personAdaIdDdl, int personId)
        {
            if (personAdaIdDdl < 1)
                return Json("The person Ada Record does not exist", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            clientDbContext.Database.ExecuteSqlCommand(" " +
                                "DELETE FROM PersonAda " +
                                "WHERE PersonAdaId = @PersonAdaId ",
                                new SqlParameter("@PersonAdaId", personAdaIdDdl));

            //// this code is duplicated from get PersonAda() action
            PersonAdaVm personAdaVm;
            int personAdaId = clientDbContext.PersonADAs.Where(x => x.PersonId == personId).Select(x => x.PersonAdaId).FirstOrDefault();
            if (personAdaId < 1)
            {
                personAdaVm = new PersonAdaVm();
                personAdaVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personAdaVm.PersonId = personId;
                personAdaVm.PersonAdaId = 0;
            }
            else
                personAdaVm = GetPersonAdaRecord(personAdaId);

            ModelState.Clear();
            //return Json(new { personAdaVm, succeed = true, Message = "Record deleted successfully!" }, JsonRequestBehavior.AllowGet);
            return Json(personAdaVm, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPersonAdaList([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
            var personAdaVm = clientDbContext.PersonADAs
          .Include(x => x.DdlAccommodationType.Description)
          .Include(x => x.DdlDisabilityType.Description)
          .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonId == personId)
          .Select(x => new PersonAdaVm
          {
              PersonAdaId = x.PersonAdaId,
              PersonId = x.PersonId,
              PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
              AccommodationProvided = x.AccommodationProvided,
              AccommodationTypeId = x.AccommodationTypeId,
              AccommodationDescription = x.DdlAccommodationType.Description,
              ActualCost = x.ActualCost,
              AssociatedDisabilityTypeId = x.AssociatedDisabilityId,
              AssociatedDisabilityDescription = x.DdlDisabilityType.Description,
              EstimatedCost = x.EstimatedCost,
              EnteredBy = x.EnteredBy,
              EnteredDate = x.EnteredDate,
              ModifiedBy = x.ModifiedBy,
              ModifiedDate = x.ModifiedDate,
              Notes = x.Notes,
              ProvidedDate = x.ProvidedDate,
              RequestedDate = x.RequestedDate,
              DisabilityTypeId = x.AssociatedDisabilityId
          })
          .ToList();
            //var personAdaList = clientDbContext.PersonADAs.Include(x => x.DdlAccommodationType.Description)
            //    .Where(x => x.PersonId == personId)
            //    .Select(m => new PersonAdaVm
            //    {
            //        PersonAdaId = m.PersonAdaId,
            //        PersonId = m.PersonId,
            //        AccommodationDescription = m.DdlAccommodationType.Description,
            //        AssociatedDisabilityDescription=
            //        Notes = m.Notes,
            //        ActualCost = m.ActualCost

            //    }).OrderBy(x => x.AccommodationDescription).ToList();

            return Json(personAdaVm.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<PersonAdaVm> GetPersonAdaData(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var personAdaVm = clientDbContext.PersonADAs
            .Include(x => x.DdlAccommodationType.Description)
            .Include(x => x.DdlDisabilityType.Description)
            .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonId == personId)
            .Select(x => new PersonAdaVm
            {
                PersonAdaId = x.PersonAdaId,
                PersonId = x.PersonId,
                PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                AccommodationProvided = x.AccommodationProvided,
                AccommodationTypeId = x.AccommodationTypeId,
                AccommodationDescription = x.DdlAccommodationType.Description,
                ActualCost = x.ActualCost,
                AssociatedDisabilityTypeId = x.AssociatedDisabilityId,
                AssociatedDisabilityDescription = x.DdlDisabilityType.Description,
                EstimatedCost = x.EstimatedCost,
                EnteredBy = x.EnteredBy,
                EnteredDate = x.EnteredDate,
                ModifiedBy = x.ModifiedBy,
                ModifiedDate = x.ModifiedDate,
                Notes = x.Notes,
                ProvidedDate = x.ProvidedDate,
                RequestedDate = x.RequestedDate,
                DisabilityTypeId=x.AssociatedDisabilityId
            })
            .ToList();
            return personAdaVm;
        }

        public ActionResult PersonAdaDetail(int PersonAdaId, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            List<PersonAdaVm> PersonAda = new List<PersonAdaVm>();
            PersonAdaVm PersonAdaVM = new PersonAdaVm();
            if (personId==0)
            {
                PersonAdaVM.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
            }
            if (PersonAdaId > 0)
            {
                var pa = GetPersonAdaData(personId);
                PersonAda = pa.ToList();
                PersonAdaVM = PersonAda.SingleOrDefault(a => a.PersonAdaId == PersonAdaId);
            }
            //else
            //{
            //    PersonAdaVM.PersonAdaId = 0;
            //}
            PersonAdaVM.AccomodationList = clientDbContext.DdlAccommodationTypes.Where(x => x.Active == true).ToList();
            PersonAdaVM.AccomodationList.Insert(0, new DdlAccommodationType { AccommodationTypeId = 0, Description = "Select" });
            PersonAdaVM.DisabilityList = clientDbContext.DdlDisabilityTypes.Where(x => x.Active == true).ToList();
            PersonAdaVM.DisabilityList.Insert(0, new DdlDisabilityType { DisabilityTypeId = 0, Description = "Select" });
            return PartialView("PersonAdaDetail", PersonAdaVM);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdaList_Destroy([DataSourceRequest] DataSourceRequest request, PersonAdaVm Ada)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            try
            {
                clientDbContext.Database.ExecuteSqlCommand(" " +
                               "DELETE FROM PersonAda " +
                               "WHERE PersonAdaId = @PersonAdaId ",
                               new SqlParameter("@PersonAdaId", Ada.PersonAdaId));
            }
            catch// (Exception ex)
            {
                ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
            }


            return Json(new[] { Ada }.ToDataSourceResult(request, ModelState));
        }
    }
}