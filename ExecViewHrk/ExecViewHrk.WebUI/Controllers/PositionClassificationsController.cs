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

namespace ExecViewHrk.WebUI.Controllers
{
    public class PositionClassificationsController : Controller
    {
        // GET: PositionClassifications
        public ActionResult PositionClassificationsMatrixPartial()
        {
            string connString = User.Identity.GetClientConnectionString();

            PositionClassificationVm positionClassificationVm = new PositionClassificationVm();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                positionClassificationVm.ClassificationColumns = clientDbContext.PositionClassifications.ToList();
            }
            return View(positionClassificationVm);
        }


        [HttpPost]
        public ActionResult PositionClassificationsSave(PositionClassificationVm positionClassificationVm)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (positionClassificationVm != null && ModelState.IsValid)
                {

                    //var p = positionClassificationVm.ClassificationColumns.ToLookup(x => x.IsCriteriaApplicable == true);
                    //IEnumerable<PositionClassification> trueList = p[true];
                    //IEnumerable<PositionClassification> falseList = p[false];

                    //clientDbContext.PositionClassifications
                    //    .Where(x => trueList.Contains(x.ClassificationCriteria)) .SingleOrDefault();
                    //    .ToList()
                    //    .ForEach(a=>a. =true);

                    //                 db.SomeTable
                    //.Where(x=>ls.Contains(x.friendid))
                    //.ToList()
                    //.ForEach(a=>a.status=true);

                    //check "Positions" table has records
                    if(clientDbContext.Positions.Any())
                    {
                        ModelState.AddModelError("", "Changes not allowed. Records exists with the existing criterias");
                    }
                    else
                    {
                        for (int i = 0; i < positionClassificationVm.ClassificationColumns.Count(); i++)
                        {
                            var val = positionClassificationVm.ClassificationColumns[i].ClassificationCriteria;
                            var classificationCriteria = clientDbContext.PositionClassifications
                                .Where(x => x.ClassificationCriteria == val).FirstOrDefault();
                            classificationCriteria.IsCriteriaApplicable = positionClassificationVm.ClassificationColumns[i].IsCriteriaApplicable;
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
                    
                }
                return Json(positionClassificationVm, JsonRequestBehavior.AllowGet);
            }
        }

    }
}