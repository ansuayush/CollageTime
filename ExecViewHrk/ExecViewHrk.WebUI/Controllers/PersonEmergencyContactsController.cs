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

namespace ExecViewHrk.WebUI.Controllers
{
    public class PersonEmergencyContactsController : Controller
    {
        // GET: PersonEmergencyContacts
        public ActionResult PersonEmergencyContactsMatrixPartial()
        {
            return View();
        }


        public ActionResult EmergencyContactsList_Read([DataSourceRequest]DataSourceRequest request, bool isSelectedIndex = false)
        {
            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees")) throw new Exception("Client Employee trying to access NSS.");


            if (requestType != "IsSelfService") SessionStateHelper.CheckForPersonSelectedValue();

            string connString = User.Identity.GetClientConnectionString();

            ClientDbContext clientDbContext = new ClientDbContext(connString);

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons
                    .Where(x => x.eMail == User.Identity.Name)
                    .Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            if ((personId == 0) && (requestType == "IsSelfService"))
                throw new Exception("Self service person id is 0 - Personal record cannot be displayed. It doesn't exist.");
            
                var emergencyContactsList = clientDbContext.PersonRelationships
                        .Include("DdlRelationshipType")
                        .Include("Person1")
                        .Include("Person1.PersonPhoneNumbers")
                        .Where(m => (m.PersonId == personId) && (m.EmergencyContact == true))
                        .Select(m => new
                        {
                            RelationPersonId = m.RelationPersonId,
                            RelationshipType = m.DdlRelationshipType.Description,
                            PersonName = m.Person1.Firstname + " " + m.Person1.Lastname,
                            DOB = m.Person1.DOB,
                            PersonPhoneNumbers = m.Person1.PersonPhoneNumbers
                        }).ToList()
                        .Select(e => new PersonEmergencyContactVm
                        {
                            RelationPersonId = e.RelationPersonId,
                            RelationshipType = e.RelationshipType,
                            PersonName = e.PersonName,
                            DOB = e.DOB,
                            PersonPhoneNumbers = e.PersonPhoneNumbers
                        });

                return Json(emergencyContactsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }


    }
