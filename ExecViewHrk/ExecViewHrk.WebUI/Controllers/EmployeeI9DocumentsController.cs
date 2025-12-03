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

namespace ExecViewHrk.WebUI.Controllers
{
    public class EmployeeI9DocumentsController : Controller
    {
        // GET: EmployeeI9Documents
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult EmployeeI9DocumentsMatrixPartial(bool isSelectedIndex = false, int personEmployeeIdParam = 0)
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

            EmployeeI9DocumentVm employeeI9DocumentVm;

            if (isSelectedIndex && personEmployeeIdParam > 0)
            {
                employeeI9DocumentVm = GeEmployeeI9DocumentsRecord(personEmployeeIdParam);
            }
            else
            {
                //ModelState.AddModelError("", "Add Employee");

               // int personEmployeeId = clientDbContext.Employees.Where(x => x.PersonId == personId).Select(x => x.EmployeeId).FirstOrDefault();
                int employeeI9DocumentId = clientDbContext.EmployeeI9Documents
                    .Include("Employee.Person")
                    .Where(x => x.Employee.Person.PersonId == personId)
                    .Select(m => m.EmployeeI9DocumentId).FirstOrDefault();

                if (employeeI9DocumentId == 0)
                {
                    employeeI9DocumentVm = new EmployeeI9DocumentVm();

                    

                    employeeI9DocumentVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    employeeI9DocumentVm.PersonId = personId;              
                }
                else
                    employeeI9DocumentVm = GeEmployeeI9DocumentsRecord(employeeI9DocumentId);
            }

            return PartialView(employeeI9DocumentVm);
        }


        public EmployeeI9DocumentVm GeEmployeeI9DocumentsRecord(int employeeI9DocumentId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            EmployeeI9DocumentVm employeeI9DocumentVm = clientDbContext.EmployeeI9Documents
                .Include("DdlI9DocumentTypes")
                .Include("Employee.Person")
                .Where(x => x.EmployeeI9DocumentId == employeeI9DocumentId)
                .Select(m => new EmployeeI9DocumentVm
                {
                    EmployeeI9DocumentId = m.EmployeeI9DocumentId,
                    EmployeeId = m.EmployeeId,
                    EmploymentNumber = m.Employee.EmploymentNumber,
                    PersonId = m.Employee.Person.PersonId,
                    PersonName = m.Employee.Person.Lastname + ", " + m.Employee.Person.Firstname,
                    I9DocumentTypeId = m.I9DocumentTypeId,
                    I9DocumentTypeDescription = m.DdlI9DocumentTypes.Description,
                    PresentedDate = m.PresentedDate,
                    ExpirationDate = m.ExpirationDate,
                    Notes = m.Notes,
                    EnteredBy = m.EnteredBy,
                    EnteredDate = m.EnteredDate
                })
                .OrderByDescending(n => n.EmploymentNumber)
                .FirstOrDefault();

            //EmployeeI9DocumentVm employeeI9DocumentVm = clientDbContext.Employees
            //    .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.EmployeeId == personEmployeeId)
            //    .Select(x => new PersonEmployeeVm
            //    {
            //        EmployeeId = x.EmployeeId,
            //        EmploymentNumber = x.EmploymentNumber,
            //        Notes = x.Notes
            //        EnteredBy = x.EnteredBy,
            //        EnteredDate = x.EnteredDate,
            //        ModifiedBy = x.ModifiedBy,
            //        ModifiedDate = x.ModifiedDate,  
            //    })
            //    .FirstOrDefault();

            return employeeI9DocumentVm;
        }


        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedEmployeeI9DocumentsAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonAda() action; drop down on top left of employment screen
            EmployeeI9DocumentVm employeeI9DocumentVm = new EmployeeI9DocumentVm();

            int employeeI9DocumentId = clientDbContext.EmployeeI9Documents
                   .Include("Employee.Person")
                   .Where(x => x.Employee.Person.PersonId == personId)
                   .OrderByDescending(x => x.Employee.EmploymentNumber)
                   .Select(m => m.EmployeeI9DocumentId).FirstOrDefault();

            if (employeeI9DocumentId != 0)
            {
                employeeI9DocumentVm = GeEmployeeI9DocumentsRecord(employeeI9DocumentId);
            }
            else
            {
                employeeI9DocumentVm = new EmployeeI9DocumentVm();
                employeeI9DocumentVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                employeeI9DocumentVm.PersonId = personId;
            }
              


            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(employeeI9DocumentVm, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EmployeeI9DocumentsIndexChangedAjax(int employeeI9DocumentIdDdl, int personId, int employeeId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            EmployeeI9DocumentVm employeeI9DocumentVm =  new EmployeeI9DocumentVm();

            if (employeeI9DocumentIdDdl != 0)
            {
                employeeI9DocumentVm = GeEmployeeI9DocumentsRecord(employeeI9DocumentIdDdl);
                //employeeI9DocumentVm.EmploymentNumber = Convert.ToByte(Convert.ToInt32(PersonEmployeeVm.MaxEmploymentNumber) + 1);
            }
            else
            {
                employeeI9DocumentVm = new EmployeeI9DocumentVm();
                employeeI9DocumentVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                employeeI9DocumentVm.PersonId = personId;
                employeeI9DocumentVm.EmployeeId = employeeId;
            }
               

            ModelState.Clear();

            return Json(employeeI9DocumentVm, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeEmploymentNumberList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var employmentNumberList = clientDbContext.Employees
                .Include("Person")
                .Where(x => x.PersonId == personId)
                 .Select (m => new 
                {
                    EmployeeId = m.EmployeeId,
                    EmploymentNumber = m.EmploymentNumber
                }).OrderByDescending(n => n.EmploymentNumber).ToList();
                
                //clientDbContext.EmployeeI9Documents.Include("Employee.Person")
                //.Where(x => x.Employee.Person.PersonId == 79)
                //.Select (m => new 
                //{
                //    EmployeeId = m.Employee.EmployeeId,
                //    EmploymentNumber = m.Employee.EmploymentNumber
                //}).OrderByDescending(n => n.EmploymentNumber).ToList();

            return Json(employmentNumberList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeI9DocumentList(int? EmployeeIdDdl)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();
           

            //if (EmployeeIdDdl != null)
            //{
                var employeeI9DocumentList = clientDbContext.EmployeeI9Documents
                .Include("DdlI9DocumentTypes")
                .Where(x => x.EmployeeId == EmployeeIdDdl)
                .Select(m => new
                {
                    EmployeeI9DocumentId = m.EmployeeI9DocumentId,
                    I9DocumentTypeDescription = m.DdlI9DocumentTypes.Description
                }).ToList();
            //}

            return Json(employeeI9DocumentList, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult EmployeeI9DocumentsSaveAjax(EmployeeI9DocumentVm employeeI9DocumentVm)
        {

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && employeeI9DocumentVm.EmployeeI9DocumentId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Examination record is for.");
                    return View(employeeI9DocumentVm);
                }
                else
                {
                    employeeI9DocumentVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                    var containEmployeeId = clientDbContext.Employees
                        .Include("Person")
                        .Any(m => m.PersonId == employeeI9DocumentVm.PersonId && m.EmployeeId == employeeI9DocumentVm.EmployeeId);

                    if(!containEmployeeId)
                    {
                        ModelState.AddModelError("", "Select an Employment Number.");
                        return View(employeeI9DocumentVm); 
                    }
                       
                }
            }

            if (ModelState.IsValid)
            {
                EmployeeI9Documents employeeI9Document = clientDbContext.EmployeeI9Documents
                    .Include("DdlI9DocumentTypes")
                    .Include("Employee.Person")
                    .Where(x => x.EmployeeI9DocumentId == employeeI9DocumentVm.EmployeeI9DocumentId).SingleOrDefault();

                if (employeeI9Document == null)
                {
                    employeeI9Document = new EmployeeI9Documents();
                    employeeI9Document.EnteredBy = User.Identity.Name;
                    employeeI9Document.EnteredDate = DateTime.Now;
                    employeeI9Document.EmployeeId = employeeI9DocumentVm.EmployeeId;
                    recordIsNew = true;
                }
                //else
                //{
                //    personExamination.ModifiedBy = User.Identity.Name;
                //    personExamination.ModifiedDate = DateTime.Now;
                //}

                byte i9DocumentTypeId = 0;
                if (!byte.TryParse(employeeI9DocumentVm.I9DocumentTypeDescription, out i9DocumentTypeId))
                {
                    var i9DocumentTypeInDb = clientDbContext.DdlI9DocumentTypes
                        .Where(x => x.Description == employeeI9DocumentVm.I9DocumentTypeDescription).SingleOrDefault();

                    if (i9DocumentTypeInDb == null)
                    {
                        ModelState.AddModelError("", "I9 Document type does not exist.");
                        return View(employeeI9Document);
                    }
                    else i9DocumentTypeId = i9DocumentTypeInDb.I9DocumentTypeId;
                }
                employeeI9Document.I9DocumentTypeId = i9DocumentTypeId;

                employeeI9Document.Notes = employeeI9DocumentVm.Notes;
                employeeI9Document.PresentedDate = employeeI9DocumentVm.PresentedDate;
                employeeI9Document.ExpirationDate = employeeI9DocumentVm.ExpirationDate;

                if (recordIsNew)
                    clientDbContext.EmployeeI9Documents.Add(employeeI9Document);

                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "Employee New I9 Document Added" : "Employee I9 Document Saved";
                    employeeI9DocumentVm = GeEmployeeI9DocumentsRecord(employeeI9Document.EmployeeI9DocumentId); // refresh the view model
                }
                catch (Exception err)
                {
                    ViewBag.AlertMessage = "";

                    IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                    if (errors.Any(a=> a.ValidationErrors != null))
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
              //  var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                ModelState.AddModelError("", "The record has been altered on transfer and could not be saved at this time.");
            }

            return Json(employeeI9DocumentVm, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EmployeeI9DocumentsDeleteAjax(int employeeI9DocumentIdDdl, int personId)
        {
            if (employeeI9DocumentIdDdl < 1)
                return Json("The Employee I9 Document does not exist.", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            clientDbContext.Database.ExecuteSqlCommand(" " +
                                "DELETE FROM EmployeeI9Documents " +
                                "WHERE EmployeeI9DocumentId = @EmployeeI9DocumentId ",
                                new SqlParameter("@EmployeeI9DocumentId", employeeI9DocumentIdDdl));

            //// this code is duplicated from get PersonAda() action
            EmployeeI9DocumentVm employeeI9DocumentVm ;
            int employeeI9DocumentId = clientDbContext.EmployeeI9Documents.Where(x => x.Employee.Person.PersonId == personId)
                .OrderByDescending(x => x.Employee.EmploymentNumber)                      
                .Select(x => x.EmployeeI9DocumentId).FirstOrDefault();

            if (employeeI9DocumentId != 0)
            {
                employeeI9DocumentVm = GeEmployeeI9DocumentsRecord(employeeI9DocumentId);
            }
            else
            {
                employeeI9DocumentVm = new EmployeeI9DocumentVm();
                employeeI9DocumentVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                employeeI9DocumentVm.PersonId = personId;
                employeeI9DocumentVm.EmployeeI9DocumentId = 0;
            }
              

            ModelState.Clear();

            return Json(employeeI9DocumentVm, JsonRequestBehavior.AllowGet);
        }

     
    }
}