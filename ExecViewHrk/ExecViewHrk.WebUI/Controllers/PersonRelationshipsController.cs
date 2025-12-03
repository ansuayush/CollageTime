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
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Newtonsoft.Json;
using ExecViewHrk.Models;
using ExecViewHrk.Domain.Interface;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PersonRelationshipsController : Controller
    {
        readonly IPersonAddress _personAddressRepository;
        readonly IPersonPhoneNumbersRepository _personPhoneNumbersRepository;
        // GET: PersonRelationships
        public ActionResult Index()
        {
            return View();
        }
        public PersonRelationshipsController(IPersonAddress personAddressRepository, IPersonPhoneNumbersRepository personPhoneNumbersRepository)
        {           
            _personAddressRepository = personAddressRepository;
            _personPhoneNumbersRepository = personPhoneNumbersRepository;
        }

        public PartialViewResult PersonRelationshipsMatrixPartial(bool isSelectedIndex = false, int personRelationshipIdParam = 0)
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

            if ((personId == 0) && (requestType == "IsSelfService"))
                throw new Exception("Self service person id is 0 - Personal record cannot be displayed. It doesn't exist.");

            PersonRelationshipVm personRelationshipVm;

            if (isSelectedIndex && personRelationshipIdParam > 0)
            {
                personRelationshipVm = GetPersonRelationshipsRecord(personRelationshipIdParam);
            }
            else
            {
                int personRelationshipId = clientDbContext.PersonRelationships.Where(x => x.PersonId == personId).Select(x => x.PersonRelationshipId).FirstOrDefault();
                if (personRelationshipId == 0)
                {
                    personRelationshipVm = new PersonRelationshipVm();
                    personRelationshipVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    personRelationshipVm.PersonId = personId;
                }
                else
                    personRelationshipVm = GetPersonRelationshipsRecord(personRelationshipId);
            }


            return PartialView(personRelationshipVm);
        }

        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedRelationshipsAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonAda() action; drop down on top left of employment screen
            PersonRelationshipVm personRelationshipVm;
            int personRelationshipId = clientDbContext.PersonRelationships.Where(x => x.PersonId == personId).Select(x => x.PersonRelationshipId).FirstOrDefault();
            if (personRelationshipId == 0)
            {
                personRelationshipVm = new PersonRelationshipVm();
                personRelationshipVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personRelationshipVm.PersonId = personId;
            }
            else
                personRelationshipVm = GetPersonRelationshipsRecord(personRelationshipId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personRelationshipVm, JsonRequestBehavior.AllowGet);
        }

        public PersonRelationshipVm GetPersonRelationshipsRecord(int personRelationshipId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            int personid = clientDbContext.PersonRelationships.Where(m => m.PersonRelationshipId == personRelationshipId).Select(n => n.RelationPersonId).FirstOrDefault();
            string relationpersonname = clientDbContext.Persons.Where(s => s.PersonId == personid).Select(p => (p.Lastname + ", " + p.Firstname)).FirstOrDefault();
            PersonRelationshipVm personRelationshipVm = clientDbContext.PersonRelationships
                .Include(x => x.DdlRelationshipType.Description)
                .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonRelationshipId == personRelationshipId)
                .Select(x => new PersonRelationshipVm
                {
                    PersonRelationshipId = x.PersonRelationshipId,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                    RelationshipTypeId = x.RelationshipTypeId,
                   RelationshipDescription = x.DdlRelationshipType.Description,
                    RelationPersonId = x.RelationPersonId,
                   RelationPersonName = relationpersonname,
                    Dependent = x.Dependent,
                    EmergencyContact = x.EmergencyContact,
                    Garnishment = x.Garnishment,
                    EnteredBy = x.EnteredBy,
                    EnteredDate = x.EnteredDate,
                })
                .FirstOrDefault();

            var a = _personPhoneNumbersRepository.GetPersonPhoneNumbersList(personid);
            personRelationshipVm.personPhoneNumberVm = _personPhoneNumbersRepository.GetPersonPhoneNumbersList(personid);
            personRelationshipVm.personAddressVm = _personAddressRepository.GetPersonIsPrimaryAddressList(personid);

            return personRelationshipVm;
        }

        // the record for the same person has changed ; drop down on top right of ADA record
        public ActionResult PersonRelationshipsIndexChangedAjax(int personRelationshipIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            //// this code is duplicated from get PersonAda() action
            PersonRelationshipVm personRelationshipVm;

            if (personRelationshipIdDdl < 1)
            {
                personRelationshipVm = new PersonRelationshipVm();
                personRelationshipVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personRelationshipVm.PersonId = personId;
                personRelationshipVm.PersonRelationshipId = 0;
            }
            else
                personRelationshipVm = GetPersonRelationshipsRecord(personRelationshipIdDdl);

            ModelState.Clear();

            return Json(personRelationshipVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PersonRelationshipsSaveAjax(PersonRelationshipVm personRelationshipVm)
        {

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            if (personRelationshipVm.PersonRelationshipId != 0)
            {
                var personid = clientDbContext.PersonRelationships.Where(x => x.PersonRelationshipId == personRelationshipVm.PersonRelationshipId).Select(x => x.PersonId).FirstOrDefault();
                personRelationshipVm.PersonId = personid;
                if (personRelationshipVm.PersonId == personRelationshipVm.RelationPersonId)
                {
                    return Json(new { succeed = false, Message = "You can not select yourself as a relationship." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {

            }
            if (personRelationshipVm.PersonId == personRelationshipVm.RelationPersonId)
            {
                return Json(new { succeed = false, Message = "You can not select yourself as a relationship." }, JsonRequestBehavior.AllowGet);
            }

            var relationId = clientDbContext.PersonRelationships.Where(x => x.RelationPersonId == personRelationshipVm.RelationPersonId).ToList();
            if (relationId.Count > 1)
            {
                return Json(new { succeed = false, Message = "This person already exists as a relation" }, JsonRequestBehavior.AllowGet);
            }
            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personRelationshipVm.PersonRelationshipId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Relationship record is for.");
                    return View(personRelationshipVm);
                }
                else
                {
                    personRelationshipVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            if (ModelState.IsValid)
            {
                PersonRelationship personRelationship = clientDbContext.PersonRelationships.Include(x => x.Person)
                    .Where(x => x.PersonRelationshipId == personRelationshipVm.PersonRelationshipId).SingleOrDefault();

                if (personRelationship == null)
                {
                    personRelationship = new PersonRelationship();
                    personRelationship.EnteredBy = User.Identity.Name;
                    personRelationship.EnteredDate = DateTime.Now;
                    personRelationship.PersonId = personRelationshipVm.PersonId;
                    recordIsNew = true;
                }
                else
                {
                    personRelationship.ModifiedBy = User.Identity.Name;
                    personRelationship.ModifiedDate = DateTime.Now;
                }


                if (!string.IsNullOrEmpty(personRelationshipVm.RelationshipTypeId.ToString()))
                {
                    int relationshipTypeInDb = clientDbContext.DdlRelationshipTypes
                        .Where(x => x.RelationshipTypeId == personRelationshipVm.RelationshipTypeId).Select(x => x.RelationshipTypeId).SingleOrDefault();

                    if (relationshipTypeInDb <= 0)
                    {
                        return Json(new { succeed = false, Message = "The Relationship type does not exist." }, JsonRequestBehavior.AllowGet);
                    }
                    else personRelationship.RelationshipTypeId = relationshipTypeInDb;
                }


                if (!string.IsNullOrEmpty(personRelationshipVm.RelationPersonId.ToString()))
                {
                    var relationPersonInDb = personRelationshipVm.RelationPersonId;
                    if (relationPersonInDb == 0)
                    {
                        return Json(new { succeed = false, Message = "Person doesn't exist!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        personRelationship.RelationPersonId = relationPersonInDb;
                    }
                }


                personRelationship.Dependent = personRelationshipVm.Dependent;
                personRelationship.EmergencyContact = personRelationshipVm.EmergencyContact;
                personRelationship.Garnishment = personRelationshipVm.Garnishment;

                if (recordIsNew)
                    clientDbContext.PersonRelationships.Add(personRelationship);


                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "New Person Relationship Record Added" : "Person Relationship Record Updated";
                    personRelationshipVm = GetPersonRelationshipsRecord(personRelationship.PersonRelationshipId); // refresh the view model
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
                ModelState.AddModelError("", "The record has been altered on transfer and could not be saved at this time.");
            }
            return Json(new { succeed = true, personRelationshipVm, Message = ViewBag.AlertMessage }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PersonRelationshipsDeleteAjax(int personRelationshipIdDdl, int personId)
        {
            if (personRelationshipIdDdl < 1)
                return Json("The person Relationship Record does not exist", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            clientDbContext.Database.ExecuteSqlCommand(" " +
                                "DELETE FROM PersonRelationships " +
                                "WHERE PersonRelationshipId = @PersonRelationshipId ",
                                new SqlParameter("@PersonRelationshipId", personRelationshipIdDdl));

            //// this code is duplicated from get PersonAda() action
            PersonRelationshipVm personRelationshipVm;
            int personRelationshipId = clientDbContext.PersonRelationships.Where(x => x.PersonId == personId).Select(x => x.PersonRelationshipId).FirstOrDefault();
            if (personRelationshipIdDdl < 1)
            {
                personRelationshipVm = new PersonRelationshipVm();
                personRelationshipVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personRelationshipVm.PersonId = personId;
                personRelationshipVm.PersonRelationshipId = 0;
            }
            else
                personRelationshipVm = GetPersonRelationshipsRecord(personRelationshipId);

            ModelState.Clear();

            return Json(new { personRelationshipVm, succeed = true, Message = "Record deleted successfully!" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPersonRelationshipsList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personRelationshipList = clientDbContext.PersonRelationships.Include(x => x.DdlRelationshipType.Description)
                .Where(x => x.PersonId == personId)
                .Select(m => new
                {
                    PersonRelationshipId = m.PersonRelationshipId,
                    PersonRelationshipDescription = m.DdlRelationshipType.Description
                }).OrderBy(x => x.PersonRelationshipDescription).ToList();

            return Json(personRelationshipList, JsonRequestBehavior.AllowGet);
        }

        //All the person names except selected PersonId
        public string GetRelationPersonsList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            //CheckForPersonSelectedValue();

            var RelationPersonsList = clientDbContext.Persons
                //.Where(x => x.PersonId != personId)
                .OrderBy(m => m.Lastname).ThenBy(m => m.Firstname)
                .Select(m => new DropDownModel
                {
                    keyvalue = m.PersonId.ToString(),
                    keydescription = m.Lastname + ", " + m.Firstname
                }).OrderBy(x=>x.keydescription).ToList().CleanUp();

            // return Json(RelationPersonsList, JsonRequestBehavior.AllowGet);
            return JsonConvert.SerializeObject(RelationPersonsList);

        }

        public ActionResult PersonRelationshipsList(bool isSelectedIndex = false, int personTestIdParam = 0)
        {

            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees"))
                throw new Exception("Client Employee trying to access NSS.");


            if (requestType != "IsSelfService") SessionStateHelper.CheckForPersonSelectedValue();
            return View();
        }

        public ActionResult PersonRelationshipList_Read([DataSourceRequest]DataSourceRequest request)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string connString = User.Identity.GetClientConnectionString();
            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees")) throw new Exception("Client Employee trying to access NSS.");

            if (requestType != "IsSelfService") SessionStateHelper.CheckForPersonSelectedValue();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name)
               .Select(x => x.PersonId).SingleOrDefault() : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
            int personRelationshipId = clientDbContext.PersonRelationships.Where(x => x.PersonId == personId).Select(x => x.PersonRelationshipId).FirstOrDefault();
          //  int personid = clientDbContext.PersonRelationships.Where(m => m.PersonRelationshipId == personRelationshipId).Select(n => n.RelationPersonId).FirstOrDefault();
            string relationpersonname = clientDbContext.Persons.Where(s => s.PersonId == personId).Select(p => (p.Lastname + ", " + p.Firstname)).FirstOrDefault();
            PersonRelationshipVm personRelationshipVm;
            var personRelationshipLst = new List<PersonRelationshipVm>();
            if (personRelationshipId == 0)
            {
                personRelationshipVm = new PersonRelationshipVm();
                personRelationshipVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personRelationshipVm.PersonId = personId;
                //personRelationshipLst = (List<PersonRelationshipVm>)personRelationshipVm;
                personRelationshipLst = new List<PersonRelationshipVm> { new PersonRelationshipVm {PersonName = personRelationshipVm.PersonName ,PersonId= personRelationshipVm.PersonId } };
            }

             personRelationshipLst = (from c in clientDbContext.Persons
                                          join p in clientDbContext.PersonRelationships on c.PersonId equals p.RelationPersonId
                                          join d in clientDbContext.DdlRelationshipTypes on p.DdlRelationshipType.RelationshipTypeId equals d.RelationshipTypeId
                                          where p.PersonId == personId
                                          select new PersonRelationshipVm()
                                          {
                                           //   RelationPersonName = c.PreferredName,
                                           RelationPersonName=c.Firstname+" "+c.Lastname,
                                              PersonId = c.PersonId,
                                              RelationshipDescription=p.DdlRelationshipType.Description,
                                              Dependent=p.Dependent,
                                              EmergencyContact = p.EmergencyContact,
                                              EnteredBy = p.EnteredBy,
                                              EnteredDate = p.EnteredDate,
                                              Garnishment = p.Garnishment,
                                              PersonRelationshipId=p.PersonRelationshipId
                                          
                                          }).ToList();
       


            return Json(personRelationshipLst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        public string getDdlPersonRelationshipTypes()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var relationshipTypes = clientDbContext.DdlRelationshipTypes
                   .Where(x => x.Active == true)
                   .Select(m=> new DropDownModel { keyvalue = m.RelationshipTypeId.ToString(), keydescription = m.Description })
                 
                   .OrderBy(x => x.keydescription).ToList().CleanUp();

            return JsonConvert.SerializeObject(relationshipTypes);
        }


        public JsonResult reloadDdlPersonRelationshipTypes()
        {
            List<DropDownModel> listReloaded = JsonConvert.DeserializeObject<List<DropDownModel>>(getDdlPersonRelationshipTypes());

           return Json(listReloaded, JsonRequestBehavior.AllowGet);

        }
        public ActionResult PersonRelationshipsDetail(int personRelationShipID, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            int personRelationshipId = clientDbContext.PersonRelationships.Where(x => x.RelationPersonId == personId).Select(x => x.PersonRelationshipId).FirstOrDefault();

            PersonRelationshipVm personRelationshipVm = new PersonRelationshipVm();
            if (personRelationShipID != 0)
            {
                personRelationshipVm = GetPersonRelationshipsRecord(personRelationShipID);
            }
            personRelationshipVm.lstDdlPersonRelationshipsType = JsonConvert.DeserializeObject<List<DropDownModel>>((getDdlPersonRelationshipTypes()));
            personRelationshipVm.lstRelationPersonsList = JsonConvert.DeserializeObject<List<DropDownModel>>((GetRelationPersonsList()));            

            return View(personRelationshipVm);
        }

        public ActionResult AddDdlRelationshipTypes()
        {
            var model = new DdlRelationshipType() { Active = true, IsSpouse = false, CobraEligible = false };
            return View(model);
        }

        public ActionResult DdlRelationshipTypesSaveAjax(ExecViewHrk.EfClient.DdlRelationshipType relationshipType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (relationshipType != null && ModelState.IsValid)
                {
                    var relationshipTypeInDb = clientDbContext.DdlRelationshipTypes
                        .Where(x => x.Code == relationshipType.Code)
                        .SingleOrDefault();
                    var relationshipTypeDescInDb = clientDbContext.DdlRelationshipTypes.ToList();

                    var relationtypelist = clientDbContext.DdlRelationshipTypes.Where(n => n.RelationshipTypeId != relationshipType.RelationshipTypeId).ToList();
                    var relationDesc = relationshipTypeDescInDb.Select(x => x.Description == relationshipType.Description);

                    if (relationshipTypeInDb != null || relationDesc != null)
                    {
                        if (relationtypelist.Select(m => m.Code).Contains(relationshipType.Code) || relationtypelist.Select(m => m.Description).Contains(relationshipType.Description))
                        {
                            ModelState.AddModelError("", "The Relationship Type Code or Description already exists!");
                        }
                        else
                        {
                            var newRelationshipType = new DdlRelationshipType
                            {
                                Description = relationshipType.Description,
                                Code = relationshipType.Code,
                                Active = relationshipType.Active,
                                CobraEligible = relationshipType.CobraEligible,
                                IsSpouse = relationshipType.IsSpouse
                            };

                            clientDbContext.DdlRelationshipTypes.Add(newRelationshipType);
                            clientDbContext.SaveChanges();
                            relationshipType.RelationshipTypeId = newRelationshipType.RelationshipTypeId;
                        }
                    }
                }

                //   return Json(new[] { relationshipType }.ToDataSourceResult(request, ModelState));
                return Json(new { relationshipType, succeed = true }, JsonRequestBehavior.AllowGet);
            }
        }
       


    }
}