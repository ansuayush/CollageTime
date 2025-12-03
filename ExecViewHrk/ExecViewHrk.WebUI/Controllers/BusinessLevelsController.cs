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
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Controllers
{
    public class BusinessLevelsController : Controller
    {
        // GET: BusinessLevels
        public ActionResult BusinessLevelsDetailMatrixPartial(int BusinessLevelNbr)
        {

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            BusinessLevelsDetailVm BusinessLevelsDetail = clientDbContext.PositionBusinessLevels
                 .Include(x => x.DdlBusinessLevelTypes.Description)
                 .Include(x => x.DdlEEOFileStatuses.Description)
                 // .Include(x => x.Departments.DepartmentDescription)
                 .Include(x => x.Location.LocationDescription)
                 .Include(x => x.DdlEINs.description)
                 .Include(x => x.DdlPayFrequency.Description)
                 .Where(x => x.BusinessLevelNbr == BusinessLevelNbr)
                .Select(x => new BusinessLevelsDetailVm
                {
                    BusinessLevelNbr = x.BusinessLevelNbr,
                    BusinessLevelNotes = x.BusinessLevelNotes,
                    BusinessLevelTitle = x.BusinessLevelTitle,
                    BusinessLevelTypeDescription = x.DdlBusinessLevelTypes.Description,
                    BusinessLevelCode = x.BusinessLevelCode,
                    ParentBULevelNbr = x.ParentBULevelNbr,
                    LocationId = x.LocationId,
                    FedralEINNbr = x.FedralEINNbr,
                    EEoFileStatusNbr = x.EEoFileStatusNbr,
                    PayFrequencyId = x.PayFrequencyId,
                    BusinessLevelTypeNbr = x.BusinessLevelTypeNbr,
                    ParentBULevelTitle = clientDbContext.PositionBusinessLevels.Where(m => m.ParentBULevelNbr == m.ParentBULevelNbr).FirstOrDefault().BusinessLevelTitle,
                    LocationDescription = x.Location.LocationDescription,
                    EEoDesc = x.DdlEEOFileStatuses.Description,
                    EIN = x.DdlEINs.description,
                    PayFrequency = x.DdlPayFrequency.Description,
                    SchedeuledHours = x.SchedeuledHours,
                    Active = x.Active,
                    BudgetReported = x.BudgetReported,
                    EnteredDate = x.EnteredDate,
                    EnteredBy = x.EnteredBy,
                    ModifiedDate = x.ModifiedDate,
                    ModifiedBy = x.ModifiedBy,
                })
              .FirstOrDefault();
            return View(BusinessLevelsDetail);
        }
        public ActionResult BusinessLevelsListMatrixPartial()
        {
            return View();
        }

        public ActionResult PositionBusinessLevelsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var PositionBusinessLevelsList = clientDbContext.PositionBusinessLevels
                .Include(x => x.DdlBusinessLevelTypes.Description)
                 .Include(x => x.DdlEEOFileStatuses.Description)
                 //.Include(x => x.Departments.DepartmentDescription)
                 .Include(x => x.Location.LocationDescription)
                 .Include(x => x.DdlEINs.description)
                 .Include(x => x.DdlPayFrequency.Description)
                 .Select(x => new BusinessLevelsDetailVm
                 {
                     BusinessLevelNbr = x.BusinessLevelNbr,
                     BusinessLevelNotes = x.BusinessLevelNotes,
                     BusinessLevelTitle = x.BusinessLevelTitle,
                     BusinessLevelTypeDescription = x.DdlBusinessLevelTypes.Description,
                     BusinessLevelCode = x.BusinessLevelCode,
                     LocationDescription = x.Location.LocationDescription,
                     ParentBULevelNbr = x.ParentBULevelNbr,
                     EEoDesc = x.DdlEEOFileStatuses.Description,
                     EIN = x.DdlEINs.description,
                     PayFrequency = x.DdlPayFrequency.Description,
                     SchedeuledHours = x.SchedeuledHours,
                     Active = x.Active,
                     BudgetReported = x.BudgetReported,
                     EnteredDate = x.EnteredDate,
                     EnteredBy = x.EnteredBy,
                     ModifiedDate = x.ModifiedDate,
                     ModifiedBy = x.ModifiedBy,
                 }).ToList();

            foreach (var item in PositionBusinessLevelsList)
            {
                var row = clientDbContext.PositionBusinessLevels.Where(m => m.BusinessLevelNbr == item.ParentBULevelNbr).FirstOrDefault();
                if (row != null)
                {
                    item.ParentBULevelTitle = row.BusinessLevelTitle;
                }
            }

            return Json(PositionBusinessLevelsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        //public ActionResult PositionBusinessLevelsList_Destroy(int BusinessLevelNbr)
        //{
        //    ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
        //    var dbRecord = clientDbContext.PositionBusinessLevels.Where(x => x.BusinessLevelNbr == BusinessLevelNbr).SingleOrDefault();
        //    if (dbRecord != null)
        //    {
        //        if (clientDbContext.Positions.Where(x => x.BusinessLevelNbr == dbRecord.BusinessLevelNbr).Count() > 0)
        //        {
        //            ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
        //        }
        //        else
        //        {
        //            clientDbContext.PositionBusinessLevels.Remove(dbRecord);
        //            try
        //            {
        //                clientDbContext.SaveChanges();
        //            }
        //            catch (Exception ex)
        //            {
        //                return Json(new { Message = ex.Message, succeed = false }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { Message = "Record does not exist.", succeed = false }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult PositionBusinessLevelsList_Destroy([DataSourceRequest] DataSourceRequest request, BusinessLevelsDetailVm BusinessLevels)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            try
            {
                if (clientDbContext.Positions.Any(x => x.BusinessLevelNbr == BusinessLevels.BusinessLevelNbr))
                {
                    return Json(new { Message = CustomErrorMessages.ERROR_DELETING_RECORD_ALREADY_IN_USE, succeed = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    clientDbContext.Database.ExecuteSqlCommand(" DELETE FROM PositionBusinessLevels WHERE BusinessLevelNbr = @BusinessLevelNbr ", new SqlParameter("@BusinessLevelNbr", BusinessLevels.BusinessLevelNbr));
                }
            }
            catch// (Exception ex)
            {
                ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
            }


            return Json(new[] { BusinessLevels }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult BusinessLevelsPartial(int BusinessLevelNbr)
        {
            return View(GetBusinessLevelsDetails(BusinessLevelNbr));
        }
        public BusinessLevelsDetailVm GetBusinessLevelsDetails(int BusinessLevelNbr)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            BusinessLevelsDetailVm BusinessLevelsDetail = new BusinessLevelsDetailVm();
            if (BusinessLevelNbr != 0)
            {
                BusinessLevelsDetail = clientDbContext.PositionBusinessLevels

                .Include(x => x.DdlBusinessLevelTypes.Description)
                .Include(x => x.DdlEEOFileStatuses.Description)
                .Include(x => x.Location.LocationDescription)
                .Include(x => x.DdlEINs.description)
                .Include(x => x.DdlPayFrequency.Description)
                .Where(x => x.BusinessLevelNbr == BusinessLevelNbr)
               .Select(x => new BusinessLevelsDetailVm
               {
                   BusinessLevelNbr = x.BusinessLevelNbr,
                   BusinessLevelNotes = x.BusinessLevelNotes,
                   BusinessLevelTitle = x.BusinessLevelTitle,
                   BusinessLevelTypeDescription = x.DdlBusinessLevelTypes.Description,
                   BusinessLevelCode = x.BusinessLevelCode,
                   ParentBULevelNbr = x.ParentBULevelNbr,
                   LocationId = x.LocationId,
                   FedralEINNbr = x.FedralEINNbr,
                   EEoFileStatusNbr = x.EEoFileStatusNbr,
                   PayFrequencyId = x.PayFrequencyId,
                   BusinessLevelTypeNbr = x.BusinessLevelTypeNbr,
                   ParentBULevelTitle = clientDbContext.PositionBusinessLevels.Where(m => m.BusinessLevelNbr == m.ParentBULevelNbr).FirstOrDefault().BusinessLevelTitle,
                   LocationDescription = x.Location.LocationDescription,
                   EEoDesc = x.DdlEEOFileStatuses.Description,
                   EIN = x.DdlEINs.description,
                   PayFrequency = x.DdlPayFrequency.Description,
                   SchedeuledHours = x.SchedeuledHours,
                   Active = x.Active,
                   BudgetReported = x.BudgetReported,
                   EnteredDate = x.EnteredDate,
                   EnteredBy = x.EnteredBy,
                   ModifiedDate = x.ModifiedDate,
                   ModifiedBy = x.ModifiedBy,
               })
             .FirstOrDefault();
            }
            BusinessLevelsDetail.ParentBULevelDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetParentLevelList());
            BusinessLevelsDetail.LocationDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetLocationList());
            BusinessLevelsDetail.EINDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetEINList());
            BusinessLevelsDetail.EEODropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetEEOList());
            BusinessLevelsDetail.PayFrequencyDropDownLiat = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPayFrequencyList());
            BusinessLevelsDetail.BusinessLevelTypeDropDownLiat = JsonConvert.DeserializeObject<List<DropDownModel>>(GetBusinessLevelTypeList());
            BusinessLevelsDetail.BusinessLevelNbr = BusinessLevelNbr;

            return BusinessLevelsDetail;
        }
        public string GetParentLevelList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            List<DropDownModel> _list = new List<DropDownModel>();
            if (clientDbContext.PositionBusinessLevels.Where(x => x.Active == true).Count() > 0)
            {
                _list = clientDbContext.PositionBusinessLevels.Where(x => x.Active == true).Select(m => new DropDownModel { keyvalue = m.BusinessLevelNbr.ToString(), keydescription = m.BusinessLevelTitle }).OrderBy(x => x.keydescription).ToList().CleanUp();
            }
            else
            {
                _list = _list.CleanUp();
            }
            return JsonConvert.SerializeObject(_list);
        }
        public string GetLocationList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            List<DropDownModel> _list = new List<DropDownModel>();
            if (clientDbContext.Locations.Where(x => x.Active == true).Count() > 0)
            {
                _list = clientDbContext.Locations.Where(x => x.Active == true).Select(m => new DropDownModel { keyvalue = m.LocationId.ToString(), keydescription = m.LocationDescription }).OrderBy(x => x.keydescription).ToList().CleanUp();
            }
            else
            {
                _list = _list.CleanUp();
            }
            return JsonConvert.SerializeObject(_list);
        }

        public string GetEINList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            List<DropDownModel> _list = new List<DropDownModel>();
            if (clientDbContext.DdlEINs.Where(x => x.active == true).Count() > 0)
            {
                _list = clientDbContext.DdlEINs.Where(x => x.active == true).Select(m => new DropDownModel { keyvalue = m.FedralEINNbr.ToString(), keydescription = m.description }).OrderBy(x => x.keydescription).ToList().CleanUp();
            }
            else
            {
                _list = _list.CleanUp();
            }
            return JsonConvert.SerializeObject(_list);
        }

        public string GetEEOList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            List<DropDownModel> _list = new List<DropDownModel>();
            if (clientDbContext.DdlEEOFileStatuses.Where(x => x.Active == true).Count() > 0)
            {
                _list = clientDbContext.DdlEEOFileStatuses.Where(x => x.Active == true).Select(m => new DropDownModel { keyvalue = m.EEoFileStatusNbr.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList().CleanUp();
            }
            else
            {
                _list = _list.CleanUp();
            }
            return JsonConvert.SerializeObject(_list);
        }
        public string GetPayFrequencyList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            List<DropDownModel> _list = new List<DropDownModel>();
            if (clientDbContext.DdlPayFrequencies.Where(x => x.Active == true).Count() > 0)
            {
                _list = clientDbContext.DdlPayFrequencies.Where(x => x.Active == true).Select(m => new DropDownModel { keyvalue = m.PayFrequencyId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList().CleanUp();
            }
            else
            {
                _list = _list.CleanUp();
            }
            return JsonConvert.SerializeObject(_list);

        }
        public string GetBusinessLevelTypeList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            List<DropDownModel> _list = new List<DropDownModel>();
            if (clientDbContext.DdlBusinessLevelTypes.Where(x => x.Active == true).Count() > 0)
            {
                _list = clientDbContext.DdlBusinessLevelTypes.Where(x => x.Active == true).Select(m => new DropDownModel { keyvalue = m.BusinessLevelTypeNbr.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList().CleanUp();
            }
            else
            {
                _list = _list.CleanUp();
            }
            return JsonConvert.SerializeObject(_list);

        }

        [HttpPost]
        public ActionResult BusinessLevelsSaveAjax(BusinessLevelsDetailVm BusinessLevelsVM)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string _message = "";
            bool recordIsNew = false;
           
            PositionBusinessLevels PositionBusinessLevels = clientDbContext.PositionBusinessLevels
                .Where(x => x.BusinessLevelNbr == BusinessLevelsVM.BusinessLevelNbr).SingleOrDefault();
         
             if (PositionBusinessLevels == null)
                {
                        var  isRecordExists = clientDbContext.PositionBusinessLevels.Where(x => x.BusinessLevelCode == BusinessLevelsVM.BusinessLevelCode || x.BusinessLevelTitle == BusinessLevelsVM.BusinessLevelTitle).SingleOrDefault();
                        if (isRecordExists != null)
                        {
                            return Json(new { succeed = false, Message = "The Business Level record  exists for the selected Business Level code or Title." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            PositionBusinessLevels = new PositionBusinessLevels();
                            PositionBusinessLevels.EnteredBy = User.Identity.Name;
                            PositionBusinessLevels.EnteredDate = DateTime.Now;
                            PositionBusinessLevels.Active = BusinessLevelsVM.Active;
                            PositionBusinessLevels.BusinessLevelNotes = BusinessLevelsVM.BusinessLevelNotes;
                            PositionBusinessLevels.BusinessLevelCode = BusinessLevelsVM.BusinessLevelCode;
                            PositionBusinessLevels.BusinessLevelTitle = BusinessLevelsVM.BusinessLevelTitle;
                            PositionBusinessLevels.BusinessLevelTypeNbr = BusinessLevelsVM.BusinessLevelTypeNbr != null ? BusinessLevelsVM.BusinessLevelTypeNbr : null;
                            PositionBusinessLevels.SchedeuledHours = BusinessLevelsVM.SchedeuledHours;
                            PositionBusinessLevels.FedralEINNbr = BusinessLevelsVM.FedralEINNbr;
                            PositionBusinessLevels.EEoFileStatusNbr = BusinessLevelsVM.EEoFileStatusNbr;
                            PositionBusinessLevels.ParentBULevelNbr = BusinessLevelsVM.ParentBULevelNbr != null ? BusinessLevelsVM.ParentBULevelNbr : null;
                            PositionBusinessLevels.LocationId = BusinessLevelsVM.LocationId;
                            PositionBusinessLevels.PayFrequencyId = BusinessLevelsVM.PayFrequencyId != null ? BusinessLevelsVM.PayFrequencyId : null;
                            PositionBusinessLevels.BudgetReported = BusinessLevelsVM.BudgetReported == "Gross" ? "Gross only" : "All taxes-Ded";
                            clientDbContext.PositionBusinessLevels.Add(PositionBusinessLevels);
                            recordIsNew = true;
                        }
                }
                else
                {
                       var  isRecordExists = clientDbContext.PositionBusinessLevels.Where(x => x.BusinessLevelNbr != BusinessLevelsVM.BusinessLevelNbr && (x.BusinessLevelCode == BusinessLevelsVM.BusinessLevelCode || x.BusinessLevelTitle == BusinessLevelsVM.BusinessLevelTitle)).SingleOrDefault();
                        if (isRecordExists != null)
                        {
                            return Json(new { succeed = false, Message = "The Business Level record  exists for the selected Business Level code or Title." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            PositionBusinessLevels.BusinessLevelNotes = BusinessLevelsVM.BusinessLevelNotes;
                            PositionBusinessLevels.BusinessLevelCode = BusinessLevelsVM.BusinessLevelCode;
                            PositionBusinessLevels.BusinessLevelTitle = BusinessLevelsVM.BusinessLevelTitle;
                            PositionBusinessLevels.BusinessLevelTypeNbr = BusinessLevelsVM.BusinessLevelTypeNbr != null ? BusinessLevelsVM.BusinessLevelTypeNbr : null;
                            PositionBusinessLevels.SchedeuledHours = BusinessLevelsVM.SchedeuledHours;
                            PositionBusinessLevels.FedralEINNbr = BusinessLevelsVM.FedralEINNbr;
                            PositionBusinessLevels.EEoFileStatusNbr = BusinessLevelsVM.EEoFileStatusNbr;
                            PositionBusinessLevels.ParentBULevelNbr = BusinessLevelsVM.ParentBULevelNbr != null ? BusinessLevelsVM.ParentBULevelNbr : null;
                            PositionBusinessLevels.LocationId = BusinessLevelsVM.LocationId;
                            PositionBusinessLevels.Active = BusinessLevelsVM.Active;
                            PositionBusinessLevels.PayFrequencyId = BusinessLevelsVM.PayFrequencyId != null ? BusinessLevelsVM.PayFrequencyId : null;
                            PositionBusinessLevels.BudgetReported = BusinessLevelsVM.BudgetReported == "Gross" ? "Gross only" : "All taxes-Ded";
                        }
                }

                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "New Business Levels  Record Added" : "Business Levels Record Saved";
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
           // }
            //else
            //{
            //    var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);

            //    foreach (var item in modelStateErrors)
            //    {
            //        if (_message != "") _message += "<br />";
            //        _message += item.ErrorMessage;
            //    }
            //    return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            //}

            return Json(new { BusinessLevelsVM, succeed = true, recordIsNew }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult AddDDLBusinessLevels(string type)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            dynamic columnnames;
            columnnames = from t in typeof(ddlJobClasses).GetProperties() select t.Name;
            switch (type)
            {
                case "ddlEINs":
                    columnnames = from t in typeof(DdlEINs).GetProperties() select t.Name;
                    ViewBag.Title = "Federal EIN";
                    break;
                case "Locations":
                    columnnames = from t in typeof(Location).GetProperties() select t.Name;
                    ViewBag.Title = "Locations ";
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
        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        public ActionResult DdlSave(FormCollection formCollection)
        {
            string _message = "";
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            if (ModelState.IsValid)
            {
                string type = Session["Ddltype"].ToString();
                switch (type)
                {

                    case "ddlEINs":
                        DdlEINs DdlEINs = new DdlEINs();
                        DdlEINs.EIN = formCollection["EIN"].ToString();
                        DdlEINs.description = formCollection["description"].ToString();
                        DdlEINs.active = formCollection["active"].ToString().Equals("true") ? true : false;
                        DdlEINs.addressLineOne = formCollection["addressLineOne"].ToString();
                        DdlEINs.addressLineTwo = formCollection["addressLineTwo"].ToString();
                        DdlEINs.city = formCollection["city"].ToString();
                        DdlEINs.stateID = Convert.ToByte(formCollection["stateID"].ToString());
                        DdlEINs.stateID = Convert.ToByte(formCollection["stateID"].ToString());
                        DdlEINs.zipCode = formCollection["zipCode"].ToString();
                        DdlEINs.countryID = Convert.ToByte(formCollection["countryID"].ToString());
                        DdlEINs.phoneNumber = formCollection["phoneNumber"].ToString();
                        DdlEINs.faxNumber = formCollection["faxNumber"].ToString();
                        DdlEINs.EEOFileStatusID = Convert.ToByte(formCollection["EEOFileStatusID"].ToString());
                        DdlEINs.notes = formCollection["notes"].ToString();
                        clientDbContext.DdlEINs.Add(DdlEINs);
                        break;
                    case "Locations":
                        Location Location = new Location();
                        Location.LocationDescription = formCollection["LocationDescription"].ToString();
                        Location.LocationCode = formCollection["LocationCode"].ToString();
                        Location.Active = true ;
                        clientDbContext.Locations.Add(Location);
                        break;
                    case "ddlEEOFileStatuses":
                        DdlEEOFileStatuses DdlEEOFileStatuses = new DdlEEOFileStatuses();
                        DdlEEOFileStatuses.Code = formCollection["Code"].ToString();
                        DdlEEOFileStatuses.Description = formCollection["Description"].ToString();
                        DdlEEOFileStatuses.Active = true;
                        clientDbContext.DdlEEOFileStatuses.Add(DdlEEOFileStatuses);
                        break;
                    case "ddlBusinessLevelTypes":
                        DdlBusinessLevelTypes DdlBusinessLevelTypes = new DdlBusinessLevelTypes();
                        DdlBusinessLevelTypes.Code = formCollection["Code"].ToString();
                        DdlBusinessLevelTypes.Description = formCollection["Description"].ToString();
                        DdlBusinessLevelTypes.Active =true ;
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

        public ActionResult ReloadDDL(string DDLName)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            dynamic GetList = null;

            //GetList = clientDbContext.DdlJobClasses.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();

            switch (DDLName)
            {
                case "Locations":
                    GetList = clientDbContext.Locations.OrderBy(e => e.LocationDescription).ToList();
                    break;
                case "ddlEINs":
                    GetList = clientDbContext.DdlEINs.Where(x => x.active == true).OrderBy(e => e.description).ToList();
                    break;
                case "ddlEEOFileStatuses":
                    GetList = clientDbContext.DdlEEOFileStatuses.OrderBy(e => e.Description).ToList();
                    break;

                case "ddlBusinessLevelTypes":
                    GetList = clientDbContext.DdlBusinessLevelTypes.OrderBy(e => e.Description).ToList();
                    break;
                default:
                    break;
            }
            return Json(GetList, JsonRequestBehavior.AllowGet);
        }

     
    }
}