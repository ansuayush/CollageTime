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
    public class DdlAddressTypesController : Controller
    {
        // GET: DdlAddressTypes
        public ActionResult DdlAddressTypesListMaintenance()
        {

            return View();
        }

        public ActionResult DdlAddressTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var addressTypeList = clientDbContext.DdlAddressTypes.OrderBy(e => e.Description).ToList();
                return Json(addressTypeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlAddressTypesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlAddressType addressType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (addressType != null && ModelState.IsValid)
                {
                    var addressTypeInDb = clientDbContext.DdlAddressTypes
                        .Where(x => x.Code == addressType.Code)
                        .SingleOrDefault();

                    if (addressTypeInDb != null)
                    {
                        ModelState.AddModelError("", "The Address Type" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newAddressType = new DdlAddressType
                        {
                            Description = addressType.Description ,
                            Code = addressType.Code,
                            Active = true
                        };

                        clientDbContext.DdlAddressTypes.Add(newAddressType);
                        clientDbContext.SaveChanges();
                        addressType.AddressTypeId = newAddressType.AddressTypeId;
                    }
                }

                return Json(new[] { addressType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlAddressTypesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlAddressType addressType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (addressType != null && ModelState.IsValid)
                {
                    var addressTypeInDb = clientDbContext.DdlAddressTypes
                        .Where(x => x.AddressTypeId == addressType.AddressTypeId)
                        .SingleOrDefault();

                    if (addressTypeInDb != null)
                    {
                        addressTypeInDb.Code = addressType.Code;
                        addressTypeInDb.Description = addressType.Description;
                        addressTypeInDb.Active = addressType.Active;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { addressType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlAddressTypesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlAddressType addressType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (addressType != null)
                {
                    DdlAddressType addressTypeInDb = clientDbContext.DdlAddressTypes
                        .Where(x => x.AddressTypeId == addressType.AddressTypeId).SingleOrDefault();

                    if (addressTypeInDb != null)
                    {
                        clientDbContext.DdlAddressTypes.Remove(addressTypeInDb);

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

                return Json(new[] { addressType }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetAddressTypes(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var addressTypes = clientDbContext.DdlAddressTypes
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        AddressTypeId = m.AddressTypeId,
                        AddressDescription = m.Description
                    }).OrderBy(x => x.AddressDescription).ToList();

                return Json(addressTypes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}