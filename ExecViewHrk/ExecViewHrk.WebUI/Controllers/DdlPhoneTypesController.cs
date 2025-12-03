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
using ExecViewHrk.WebUI.Helpers;
namespace ExecViewHrk.WebUI.Controllers
{
    public class DdlPhoneTypesController : Controller
    {
        // GET: DdlPhoneTypes
        public ActionResult DdlPhoneTypesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlPhoneTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var phoneTypeList = clientDbContext.DdlPhoneTypes.OrderBy(e => e.Description).ToList();
                return Json(phoneTypeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPhoneTypesList_Create([DataSourceRequest] DataSourceRequest request,DdlPhoneType phoneType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (phoneType != null && ModelState.IsValid)
                {
                    var phoneTypeInDb = clientDbContext.DdlPhoneTypes.Where(x => x.Code == phoneType.Code || x.Description == phoneType.Description).FirstOrDefault();

                    if (phoneTypeInDb != null)
                    {
                        ModelState.AddModelError("", "Phone type" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED); 
                    }
                    else
                    {
                        var newPhoneType = new DdlPhoneType
                        {
                            Description = phoneType.Description,
                            Code = phoneType.Code,
                            Active = phoneType.Active
                        };

                        clientDbContext.DdlPhoneTypes.Add(newPhoneType);
                        clientDbContext.SaveChanges();
                        phoneType.PhoneTypeId = newPhoneType.PhoneTypeId;
                    }
                }

                return Json(new[] { phoneType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPhoneTypesList_Update([DataSourceRequest] DataSourceRequest request, DdlPhoneType phoneType)
        {
            if (phoneType != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var phoneTypeInDb = clientDbContext.DdlPhoneTypes.Where(x => x.PhoneTypeId != phoneType.PhoneTypeId && (x.Code == phoneType.Code || x.Description == phoneType.Description)).SingleOrDefault();
                if (phoneTypeInDb == null)
                {
                    phoneTypeInDb = clientDbContext.DdlPhoneTypes.Where(x => x.PhoneTypeId == phoneType.PhoneTypeId).SingleOrDefault();

                    if (phoneTypeInDb.Active != phoneType.Active && phoneType.Active == false)
                    {
                        if (clientDbContext.PersonPhoneNumbers.Where(x => x.PhoneTypeId == phoneType.PhoneTypeId).Count() == 0)
                        {
                            phoneTypeInDb.Code = phoneType.Code;
                            phoneTypeInDb.Description = phoneType.Description;
                            phoneTypeInDb.Active = phoneType.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        phoneTypeInDb.Code = phoneType.Code;
                        phoneTypeInDb.Description = phoneType.Description;
                        phoneTypeInDb.Active = phoneType.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "Phone type"));
                }
            }


            return Json(new[] { phoneType }.ToDataSourceResult(request, ModelState));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPhoneTypesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlPhoneType phoneType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (phoneType != null)
                {
                    DdlPhoneType phoneTypeInDb = clientDbContext.DdlPhoneTypes
                        .Where(x => x.PhoneTypeId == phoneType.PhoneTypeId).SingleOrDefault();

                    if (phoneTypeInDb != null)
                    {
                        clientDbContext.DdlPhoneTypes.Remove(phoneTypeInDb);

                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch// (Exception err)
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                        }

                    }
                }

                return Json(new[] { phoneType }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetPhoneTypes()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var phoneTypes = clientDbContext.DdlPhoneTypes
                    .Select(m => new
                    {
                        PhoneTypeId = m.PhoneTypeId,
                        PhoneTypeDescription = m.Description,
                    }).OrderBy(x => x.PhoneTypeDescription).ToList();

                return Json(phoneTypes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}