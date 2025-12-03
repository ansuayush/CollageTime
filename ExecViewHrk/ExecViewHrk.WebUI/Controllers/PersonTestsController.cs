using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Infrastructure;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PersonTestsController : Controller
    {
        ITestRepository _testRepository;

        public PersonTestsController(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        public ActionResult PersonTestList_Read([DataSourceRequest]DataSourceRequest request)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string connString = User.Identity.GetClientConnectionString();
            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees")) throw new Exception("Client Employee trying to access NSS.");

            if (requestType != "IsSelfService") SessionStateHelper.CheckForPersonSelectedValue();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name)
                .Select(x => x.PersonId).SingleOrDefault() : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personTestList = _testRepository.GetPersonTestList(personId);

            return Json(personTestList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonTestsDelete(int personTestId)
        {
            try
            {
                _testRepository.DeletePersonTest(personTestId);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, succeed = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonTestsDetail(int personTestId, int personId)
        {
            return View(GetPersonTestsRecord(personTestId, personId));
        }

        public PersonTestVm GetPersonTestsRecord(int personTestId, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonTestVm personTestVm = new PersonTestVm();

            if (personTestId != 0)
            {
                personTestVm = _testRepository.GetPersonTestDetail(personTestId);
            }
            else
            {
                personTestVm.PersonTestId = 0;
                personTestVm.PersonName = clientDbContext.Persons.Where(x => x.PersonId == personId).Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();
            }
            personTestVm.PersonId = personId;
            personTestVm.EvaluationTestDropDownList = _testRepository.GetEvaluationTestList().CleanUp();

            return personTestVm;
        }

        public ActionResult PersonTestsList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Save(PersonTestVm personTestVm)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personTestVm.PersonTestId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Test record is for.");
                    return View(personTestVm);
                }
                else
                {
                    personTestVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            if (ModelState.IsValid)
            {
                PersonTest personTest = clientDbContext.PersonTests.Include(x => x.Person).Where(x => x.PersonTestId == personTestVm.PersonTestId).SingleOrDefault();

                if (personTestVm.PersonTestId == 0)
                {
                    var isRecordExists = clientDbContext.PersonTests.Where(x => x.EvaluationTestId == personTestVm.EvaluationTestId
                                                                            && x.TestDate == personTestVm.TestDate
                                                                            && x.PersonId == personTestVm.PersonId
                    ).Select(a => a.PersonTestId).SingleOrDefault();

                    if (personTest == null && isRecordExists != 0)
                    {
                        return Json(new { succeed = false, Message = "The Evaluation Test  exists for test date." }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (personTestVm.PersonTestId > 0)
                {
                    var isSameTestExists = clientDbContext.PersonTests.Where(x => x.PersonTestId != personTestVm.PersonTestId
                                                        && x.TestDate == personTestVm.TestDate
                                                        && x.EvaluationTestId == personTestVm.EvaluationTestId
                                                        && x.PersonId == personTestVm.PersonId
                    ).Select(a => a.PersonTestId).SingleOrDefault();

                    if (isSameTestExists != 0)
                    {
                        return Json(new { succeed = false, Message = "The Evaluation Test  exists for test date." }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (personTest == null)
                {
                    personTest = new PersonTest();
                    personTest.EnteredBy = User.Identity.Name;
                    personTest.EnteredDate = DateTime.UtcNow;
                    personTest.PersonId = personTestVm.PersonId;
                    recordIsNew = true;
                }
                personTest.EvaluationTestId = personTestVm.EvaluationTestId;

                personTest.Notes = personTestVm.Notes;
                personTest.TestDate = personTestVm.TestDate;
                personTest.Score = personTestVm.Score;
                personTest.Grade = personTestVm.Grade;
                personTest.Administrator = personTestVm.Administrator;

                if (recordIsNew) clientDbContext.PersonTests.Add(personTest);

                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "New Person Test Record Added" : "Person Test Record Saved";
                    personTestVm = GetPersonTestsRecord(personTest.PersonTestId, personTestVm.PersonId); // refresh the view model
                }
                catch (Exception err)
                {
                    string _message = "";
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
                string _message = "";
                foreach (var item in modelStateErrors)
                {
                    if (_message != "") _message += "<br />";
                    _message += item.ErrorMessage;
                }
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { personTestVm, succeed = true }, JsonRequestBehavior.AllowGet);
        }
    }
}