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
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.Models;
using ExecViewHrk.Domain.Interface;
using Newtonsoft.Json;
using System.Data.Entity.Validation;
using System.Data.SqlClient;

namespace ExecViewHrk.WebUI.Controllers
{
    public class ManagersController : Controller
    {
        private IPersonRepository _personRepository;

        public ManagersController(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }
        public ActionResult ManagersMatrixPartial()
        {
            PopulatePersonNames();
            return View();
        }

        public ActionResult ManagersList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                List<ManagerVm> getManagerList = new List<ManagerVm>();
                getManagerList = (from e in clientDbContext.Managers
                                  join e1 in clientDbContext.Persons on
                                      e.PersonId equals e1.PersonId
                                  select new ManagerVm
                                  {

                                      ManagerId = e.ManagerId,
                                      PersonName = e1.Firstname + e1.Lastname
                                  }).Distinct().OrderBy(s => s.PersonName).ToList();
                var ManagerResult = Json(getManagerList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                ManagerResult.MaxJsonLength = Int32.MaxValue;
                return ManagerResult;
            }
        }

        public ActionResult AddNewManager(int managerId)
        {
            return View(GetManagerRecord(managerId));
        }
        public ManagerVm GetManagerRecord(int managerId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var managerVm = new ManagerVm();
            try
            {
                if (managerId != 0)
                {
                    managerVm = clientDbContext.Managers
                        .Where(m => m.ManagerId == managerId)
                        .Select(x => new ManagerVm
                        {
                            ManagerId = x.ManagerId,
                            PersonName = x.Person.Lastname + " " + x.Person.Firstname
                        }).FirstOrDefault();
                }
                ViewBag.managerId = managerId;
                managerVm.PersonsList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPersonsList());
                ViewBag.PerformanceList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPersonsList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return managerVm;
        }
        public string GetPersonsList()
        {
            var personsList = new List<DropDownModel>();
            try
            {
                personsList = _personRepository.GetPersonList().CleanUp();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return JsonConvert.SerializeObject(personsList);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManagersList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.Manager manager)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (manager != null && ModelState.IsValid)
                {
                    if (manager.PersonId == 0)
                    {
                        ModelState.AddModelError("", "Please select the person.");
                    }
                    else
                    {
                        var newManager = new Manager
                        {
                            PersonId = manager.PersonId
                        };

                        clientDbContext.Managers.Add(newManager);
                        clientDbContext.SaveChanges();
                        manager.ManagerId = newManager.ManagerId;
                    }
                }

                return Json(new[] { manager }.ToDataSourceResult(request, ModelState));
            }
        }

        [HttpPost]
        public ActionResult SaveManagerRecord(int personId)
        {
            string returnmsg = "";
            string connString = User.Identity.GetClientConnectionString();
            try
            {
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    var exisitemp = clientDbContext.Managers.Where(x => x.PersonId == personId).Count();
                    if (exisitemp > 0)
                    {
                        return Json(new { succeed = false, Message = "The manager already exists!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var newitem = new Manager
                        {
                            PersonId = personId
                        };
                        clientDbContext.Managers.Add(newitem);
                        clientDbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { returnmsg = ex.ToString(); }
            return RedirectToAction("ManagersList_Read");
        }

        private void PopulatePersonNames()
        {
            var personNamesList = _personRepository.GetPersonsList("PERSONS", "");

            personNamesList.Insert(0, new PersonVm { PersonId = 0, PersonName = "--select one--" });

            ViewData["personNamesList"] = personNamesList;
        }
        public ActionResult EditManager(int managerId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var PosId = clientDbContext.ManagersPositions.Where(m => m.ManagerID == managerId).Select(m => m.PositionID).FirstOrDefault();
            var DpPosId = clientDbContext.ManagerDepartments.Where(m => m.ManagerId == managerId).Select(m => m.DepartmentId).FirstOrDefault();
            var personName = clientDbContext.Managers.Where(m => m.ManagerId == managerId).Select(m => m.Person.Lastname + " " + m.Person.Firstname).FirstOrDefault();
            var managerList = clientDbContext.Managers.Where(x => x.ManagerId == managerId).ToList()
                    .Select(e => new ManagerVm
                    {
                        ManagerId = e.ManagerId,
                        PersonName = personName,
                        PositionID = PosId == 0 ? 0 : PosId,
                        DepartmentID = DpPosId == 0 ? 0 : DpPosId
                    }).FirstOrDefault();
            TempData["ManagerId"] = managerId;
            ViewBag.ManagerId = managerList.ManagerId;
            ViewBag.PositionsList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPositionsList());
            managerList.PersonsList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPersonsList());
            var posList = clientDbContext.ManagersPositions.Where(m => m.ManagerID == managerId).Select(m => m.PositionID).ToList();
            List<DropDownModel> lst1 = new List<DropDownModel>();
            List<DropDownModel> lst2 = new List<DropDownModel>();
            if (PosId != 0)
            {
                ViewBag.PositionsList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPositionFilterbyPositionId(managerId));
                managerList.PositionsList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPositionListbyPositionId(managerId));
            }
            ViewBag.DepartmentList = clientDbContext.Departments
                                                    .Include("CompanyCode")
                                                    .Where(x => x.IsDepartmentActive == true && x.IsDeleted == false)
                                                    .Select(x => new DropDownModel { keyvalue = x.DepartmentId.ToString(), keydescription = x.CompanyCode.CompanyCodeCode + "-" + x.DepartmentCode + "-" + x.DepartmentDescription }).ToList();
            managerList.DepartmentList = (from md in clientDbContext.ManagerDepartments
                                          join d in clientDbContext.Departments.Include("CompanyCode") on md.DepartmentId equals d.DepartmentId
                                          where md.ManagerId == managerList.ManagerId && d.IsDeleted == false
                                          select new DropDownModel
                                          {
                                              keyvalue = md.DepartmentId.ToString(),
                                              keydescription = d.CompanyCode.CompanyCodeCode + "-" + d.DepartmentCode + "-" + d.DepartmentDescription
                                          }).ToList();
            if (managerList.DepartmentList.Count > 0)
            {
                List<DropDownModel> lst = new List<DropDownModel>();
                foreach (var item in ViewBag.DepartmentList)
                {
                    var value = managerList.DepartmentList.Where(x => x.keyvalue == item.keyvalue).FirstOrDefault(); ;
                    if (value == null)
                    {
                        lst.Add(item);
                    }
                }
                ViewBag.DepartmentList = lst;
            }

            return PartialView("EditManagerRecord", managerList);
        }
        [HttpPost]
        public ActionResult SaveManagers(ManagerVm managervm)
        {
            string[] arraylist = null;
            string[] arraylockedlist = null;
            string returnmsg = "";
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            int managerId = Convert.ToInt32(TempData["ManagerId"]);
            string list = managervm.managersnotlockedlist;
            string lockedlist = managervm.managerslockedlist;
            if (lockedlist != null)
            {
                arraylockedlist = lockedlist.Split(',');
            }
            if (list != null)
            {
                arraylist = list.Split(',');
            }
            try
            {
                if (arraylist != null)
                {
                    if (arraylist.Length != 0)
                    {
                        foreach (var item in arraylist)
                        {

                            var ID = clientDbContext.ManagersPositions.Where(m => (m.PositionID.ToString() == item && m.ManagerID == managerId)).Select(m => m.ID).FirstOrDefault();
                            if (ID == 0)
                            {
                                var managersposition = new ManagersPositions
                                {
                                    ManagerID = managerId,
                                    PositionID = Convert.ToInt16(item)
                                };
                                clientDbContext.ManagersPositions.Add(managersposition);
                                clientDbContext.SaveChanges();
                            }
                            else
                            {
                                clientDbContext.Database.ExecuteSqlCommand("Update ManagersPositions set PositionID=@PositionID where ID=@ID", new SqlParameter("@PositionID", Convert.ToInt16(item)), new SqlParameter("@ID", ID));
                                clientDbContext.SaveChanges();
                            }
                            if (arraylist.Length > 1)
                            {
                                TempData.Keep("ManagerId");
                            }
                        }
                        if (arraylockedlist != null)
                        {
                            foreach (var item in arraylockedlist)
                            {
                                var lockedPosList = clientDbContext.ManagersPositions.Where(m => m.ManagerID == managerId).Select(m => m.PositionID).ToList();
                                foreach (var lockeditem in lockedPosList)
                                {
                                    if (item == lockeditem.ToString())
                                    {
                                        var record = Convert.ToInt32(item);
                                        clientDbContext.ManagersPositions.Remove(clientDbContext.ManagersPositions.Where(x => (x.PositionID == record && x.ManagerID == managerId)).FirstOrDefault());
                                        clientDbContext.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
                
                saveDepartmentsList(managervm);
            }
            catch (Exception ex) { returnmsg = ex.ToString(); }
            return RedirectToAction("ManagersList_Read");
        }
        public string saveDepartmentsList(ManagerVm managervm)
        {
            string Return = "";
            string[] arraylockedlist = null;
            string[] arraylist = null;
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            int managerId = Convert.ToInt32(TempData["ManagerId"]);
            string list = managervm.Departmentnotlockedlist;
            string lockedlist = managervm.Departmentlockedlist;
            if (lockedlist != null)
            {
                arraylockedlist = lockedlist.Split(',');
            }
            if (list != null)
            {
                arraylist = list.Split(',');
            }
            try
            {
                if (arraylist != null)
                {
                    if (arraylist.Length != 0)
                    {
                        foreach (var item in arraylist)
                        {

                            var ManagerDepartmentId = clientDbContext.ManagerDepartments.Where(m => (m.DepartmentId.ToString() == item && m.ManagerId == managerId)).Select(m => m.ManagerDepartmentId).FirstOrDefault();
                            if (ManagerDepartmentId == 0)
                            {
                                var managerDepartment = new ManagerDepartment
                                {
                                    ManagerId = managerId,
                                    DepartmentId = Convert.ToInt16(item)
                                };
                                clientDbContext.ManagerDepartments.Add(managerDepartment);
                                clientDbContext.SaveChanges();
                            }
                            else
                            {
                                clientDbContext.Database.ExecuteSqlCommand("Update ManagerDepartments set DepartmentId=@DepartmentId where ManagerDepartmentId=@ManagerDepartmentId", new SqlParameter("@DepartmentId", Convert.ToInt16(item)), new SqlParameter("@ManagerDepartmentId", ManagerDepartmentId));
                                clientDbContext.SaveChanges();
                            }
                            if (arraylist.Length > 1)
                            {
                                TempData.Keep("ManagerId");
                            }
                        }
                        if (arraylockedlist != null)
                        {
                            foreach (var item in arraylockedlist)
                            {
                                var lockedDepList = clientDbContext.ManagerDepartments.Where(m => m.ManagerId == managerId).Select(m => m.DepartmentId).ToList();
                                foreach (var lockeditem in lockedDepList)
                                {
                                    if (item == lockeditem.ToString())
                                    {
                                        var record = Convert.ToInt32(item);
                                        clientDbContext.ManagerDepartments.Remove(clientDbContext.ManagerDepartments.Where(x => (x.DepartmentId == record && x.ManagerId == managerId)).FirstOrDefault());
                                        clientDbContext.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
                Return = "Success";
            }
            catch (Exception ex) { Return = ex.ToString(); }
            return Return;
        }
        public string GetPositionsList()
        {
            var positionsList = new List<DropDownModel>();
            try
            {
                positionsList = _personRepository.GetPositionsList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return JsonConvert.SerializeObject(positionsList);
        }
        public string GetPositionListbyPositionId(int managerId)
        {
            var positionsList = new List<DropDownModel>();
            try
            {
                positionsList = _personRepository.GetPositionListbyPositionId(managerId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return JsonConvert.SerializeObject(positionsList);
        }
        public string GetPositionFilterbyPositionId(int managerId)
        {
            var positionsList = new List<DropDownModel>();
            try
            {
                positionsList = _personRepository.GetPositionFilterbyPositionId(managerId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return JsonConvert.SerializeObject(positionsList);
        }
        public ActionResult ManagerDelete(int managerId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.Managers.Where(x => x.ManagerId == managerId).FirstOrDefault();
            var mgrRecord = clientDbContext.ManagersPositions.Where(x => x.ManagerID == managerId).ToList();
            if (dbRecord != null)
            {
                foreach (var item in mgrRecord)
                {
                    clientDbContext.ManagersPositions.Remove(item);
                    clientDbContext.SaveChanges();
                }
                clientDbContext.Managers.Remove(dbRecord);
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