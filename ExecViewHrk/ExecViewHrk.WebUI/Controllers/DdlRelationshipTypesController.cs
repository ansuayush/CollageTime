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
    public class DdlRelationshipTypesController : Controller
    {
        // GET: DdlRelationshipTypes
        public ActionResult DdlRelationshipTypesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlRelationshipTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var relationshipTypeList = clientDbContext.DdlRelationshipTypes.OrderBy(e => e.Description).ToList();
                return Json(relationshipTypeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlRelationshipTypesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlRelationshipType relationshipType)
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

                return Json(new[] { relationshipType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlRelationshipTypesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlRelationshipType relationshipType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (relationshipType != null && ModelState.IsValid)
                {
                    var relationshipTypeInDb = clientDbContext.DdlRelationshipTypes
                        .Where(x => x.RelationshipTypeId == relationshipType.RelationshipTypeId)
                        .SingleOrDefault();
                    var relationtypelist = clientDbContext.DdlRelationshipTypes.Where(n => n.RelationshipTypeId != relationshipType.RelationshipTypeId).ToList();
                    var usedrelationlist = clientDbContext.PersonRelationships.ToList();
                    var active = relationshipTypeInDb.Active;

                    if (relationshipTypeInDb != null)
                    {
                        if (relationtypelist.Select(m => m.Code).Contains(relationshipType.Code) || relationtypelist.Select(m => m.Description).Contains(relationshipType.Description))
                        {
                            ModelState.AddModelError("", "The Relationship Type Code or Description already exists!");
                        }
                        else if (usedrelationlist.Select(s => s.DdlRelationshipType.Description).Contains(relationshipType.Description) && active != relationshipType.Active)
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                        else
                        {
                            relationshipTypeInDb.Code = relationshipType.Code;
                            relationshipTypeInDb.Description = relationshipType.Description;
                            relationshipTypeInDb.Active = relationshipType.Active;
                            relationshipTypeInDb.CobraEligible = relationshipType.CobraEligible;
                            relationshipTypeInDb.IsSpouse = relationshipTypeInDb.IsSpouse;
                            clientDbContext.SaveChanges();
                        }
                    }
                }
                return Json(new[] { relationshipType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlRelationshipTypesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlRelationshipType relationshipType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (relationshipType != null)
                {
                    DdlRelationshipType relationshipTypeInDb = clientDbContext.DdlRelationshipTypes
                        .Where(x => x.RelationshipTypeId == relationshipType.RelationshipTypeId).SingleOrDefault();

                    if (relationshipTypeInDb != null)
                    {
                        clientDbContext.DdlRelationshipTypes.Remove(relationshipTypeInDb);

                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch// (Exception err)
                        {
                            ModelState.AddModelError("", "Can not be deleted due to record is already in use or not exists.");
                        }

                    }
                }

                return Json(new[] { relationshipType }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetRelationshipTypes(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var relationshipTypes = clientDbContext.DdlRelationshipTypes
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        RelationshipTypeId = m.RelationshipTypeId,
                        RelationshipDescription = m.Description,
                        RelationshipCobraEligible = m.CobraEligible,
                        RelationshipIsspouse = m.IsSpouse
                    }).OrderBy(x => x.RelationshipDescription).ToList();

                return Json(relationshipTypes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}