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
    public class DdlPropertyTypesController : Controller
    {
        // GET: DdlPropertyTypes
        public ActionResult DdlPropertyTypesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlPropertyTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var propertyTypeList = clientDbContext.DdlPropertyTypes.OrderBy(e => e.Description).ToList();
                return Json(propertyTypeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPropertyTypesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlPropertyType propertyType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (propertyType != null && ModelState.IsValid)
                {
                    var propertyTypeInDb = clientDbContext.DdlPropertyTypes
                        .Where(x => x.Code == propertyType.Code)
                        .SingleOrDefault();

                    var propertyDescInDb = clientDbContext.DdlPropertyTypes.ToList();
                    var propertytypeList = clientDbContext.DdlPropertyTypes.Where(n => n.PropertyTypeId != propertyType.PropertyTypeId).ToList();

                    if (propertyTypeInDb != null || propertyDescInDb.Select(s => s.Description == propertyType.Description) != null)
                    {
                        if (propertytypeList.Select(n => n.Code).Contains(propertyType.Code.Trim()) || propertytypeList.Select(m => m.Description).Contains(propertyType.Description.Trim()))
                        {
                            ModelState.AddModelError("", "The Property Type already exists!");
                        }
                        else
                        {
                            var newPropertyType = new DdlPropertyType
                            {
                                Description = propertyType.Description,
                                Code = propertyType.Code,
                                Active = propertyType.Active
                            };
                            clientDbContext.DdlPropertyTypes.Add(newPropertyType);
                            clientDbContext.SaveChanges();
                            propertyType.PropertyTypeId = newPropertyType.PropertyTypeId;
                        }
                    }
                }

                return Json(new[] { propertyType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPropertyTypesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlPropertyType propertyType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (propertyType != null && ModelState.IsValid)
                {
                    var propertyTypeInDb = clientDbContext.DdlPropertyTypes
                        .Where(x => x.PropertyTypeId == propertyType.PropertyTypeId)
                        .SingleOrDefault();
                    var propertylist = clientDbContext.DdlPropertyTypes.Where(n => n.PropertyTypeId != propertyType.PropertyTypeId).ToList();
                    var usedpropertylist = clientDbContext.PersonProperties.ToList();
                    var active = propertyTypeInDb.Active;

                    if (propertyTypeInDb != null)
                    {
                        if (propertylist.Select(m => m.Code).Contains(propertyType.Code.Trim()) || propertylist.Select(p => p.Description).Contains(propertyType.Description.Trim()))
                        {
                            ModelState.AddModelError("", "The Property Type  already exists!");
                        }
                        else if (usedpropertylist.Select(s => s.DdlPropertyType.Description).Contains(propertyType.Description) && active != propertyType.Active)
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                        else
                        {
                            propertyTypeInDb.Code = propertyType.Code;
                            propertyTypeInDb.Description = propertyType.Description;
                            propertyTypeInDb.Active = propertyType.Active;
                            clientDbContext.SaveChanges();
                        }
                    }
                }

                return Json(new[] { propertyType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPropertyTypesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlPropertyType propertyType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (propertyType != null)
                {
                    DdlPropertyType propertyTypeInDb = clientDbContext.DdlPropertyTypes
                        .Where(x => x.PropertyTypeId == propertyType.PropertyTypeId).SingleOrDefault();

                    if (propertyTypeInDb != null)
                    {
                        clientDbContext.DdlPropertyTypes.Remove(propertyTypeInDb);

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

                return Json(new[] { propertyType }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetPropertyTypes(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var propertyTypes = clientDbContext.DdlPropertyTypes
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        PropertyTypeId = m.PropertyTypeId,
                        PropertyDescription = m.Description
                    }).OrderBy(x => x.PropertyDescription).ToList();

                return Json(propertyTypes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}