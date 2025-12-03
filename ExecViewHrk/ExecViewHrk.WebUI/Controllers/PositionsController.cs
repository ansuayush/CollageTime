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
using ExecViewHrk.WebUI.Services;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PositionsController : Controller
    {
        // GET: Positions
       readonly PositionService positionService = new PositionService();
        public ActionResult PositionsMatrixPartial()
        {
            return View();
        }
        public ActionResult RateForecasting()
        {
            return View();
        }

        
        public ActionResult PositionsList_Read([DataSourceRequest]DataSourceRequest request)
        { 
            var positionsList = positionService.getAllPositions(null);
            return Json(positionsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult PositionAddModalPartial()
        {
            var details = positionService.getNewPosition();
            return View(details);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PositionsList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.Position position)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (position != null && ModelState.IsValid)
                {
                    var positionInDb = clientDbContext.Positions
                        .Where(x => x.PositionCode == position.PositionCode)
                        .SingleOrDefault();

                    if (positionInDb != null)
                    {
                        ModelState.AddModelError("", "The Position" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        bool classificationValueNULL = false;
                        string criteria = null;

                        //Method check for null value in classification criteria
                        var tuple = FindNULLCriteria(classificationValueNULL, criteria, position, clientDbContext);

                        if (tuple.Item1 == true)  // classificationValueNULL == true
                        {
                            ModelState.AddModelError("", "Select the value for " + tuple.Item2);  //criteria
                        }
                        else
                        {
                            var newPosition = new Position
                            {
                                BusinessUnitId = position.BusinessUnitId,
                                JobId = position.JobId,
                                DepartmentId = position.DepartmentId,
                                LocationId = position.LocationId,
                                UserDefinedSegment1Id = position.UserDefinedSegment1Id,
                                UserDefinedSegment2Id = position.UserDefinedSegment2Id,
                                PositionDescription = position.PositionDescription,
                                PositionCode = position.PositionCode,
                                IsPositionActive = true,
                                ReportsToPositionId = (position.ReportsToPositionId == 0) ? null : position.ReportsToPositionId
                            };

                            clientDbContext.Positions.Add(newPosition);
                            clientDbContext.SaveChanges();
                            position.PositionId = newPosition.PositionId;
                        }
                       
                    }
                }

                return Json(new[] { position }.ToDataSourceResult(request, ModelState));
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PositionsList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.Position position)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (position != null && ModelState.IsValid)
                {
                    var positionInDb = clientDbContext.Positions
                        .Where(x => x.PositionId == position.PositionId)
                        .SingleOrDefault();

                    if (positionInDb != null)
                    {
                        bool classificationValueNULL = false;
                        string criteria = null;

                        //Method check for null value in classification criteria
                        var tuple = FindNULLCriteria(classificationValueNULL, criteria, position, clientDbContext);

                        if (tuple.Item1 == true)  // classificationValueNULL == true
                        {
                            ModelState.AddModelError("", "Select the value for " + tuple.Item2);  //criteria
                        }
                        else
                        {
                            positionInDb.BusinessUnitId = position.BusinessUnitId;
                            positionInDb.PositionCode = position.PositionCode;
                            positionInDb.PositionDescription = position.PositionDescription;
                            positionInDb.JobId = position.JobId;
                            positionInDb.DepartmentId = position.DepartmentId;
                            positionInDb.LocationId = position.LocationId;
                            positionInDb.UserDefinedSegment1Id = position.UserDefinedSegment1Id;
                            positionInDb.UserDefinedSegment2Id = position.UserDefinedSegment2Id;
                            positionInDb.ReportsToPositionId = (position.ReportsToPositionId == 0) ? null : position.ReportsToPositionId;
                            positionInDb.IsPositionActive = position.IsPositionActive;
                            clientDbContext.SaveChanges();
                        }
                    }
                }

                return Json(new[] { position }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PositionsList_Destroy([DataSourceRequest] DataSourceRequest request
            , Position position)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (position != null)
                {
                    //Check all referencing tables before deletion of Position
                    //|| (clientDbContext.BusinessUnits.Any(bu => bu.BusinessUnitId == position.BusinessUnitId))
                    //   || (clientDbContext.DdlEmployeeTypes.Any(det => det.EmployeeTypeId == position.WorkClassificationId))
                    //   || (clientDbContext.DdlPositionCategory.Any(dpc => dpc.PositionCategoryID == position.PositionCategoryID))
                    //   || (clientDbContext.DdlPositionTypes.Any(dpt => dpt.PositionTypeId == position.PositionTypeID))
                    //   || (clientDbContext.Departments.Any(d => d.DepartmentId == position.DepartmentId))
                    //   || (clientDbContext.Jobs.Any(j => j.JobId == position.JobId))
                    //   || (clientDbContext.Locations.Any(l => l.LocationId == position.LocationId))
                    //   || (clientDbContext.PerformanceProfiles.Any(pp => pp.PerProfileID == position.PerProfileID))
                    //   || (clientDbContext.PositionBusinessLevels.Any(pbl => pbl.BusinessLevelNbr == position.BusinessLevelNbr))
                    //   || (clientDbContext.UserDefinedSegment1s.Any(uds1 => uds1.UserDefinedSegment1Id == position.UserDefinedSegment1Id))
                    //   || (clientDbContext.UserDefinedSegment2s.Any(uds2 => uds2.UserDefinedSegment2Id == position.UserDefinedSegment2Id))
                    if (
                          (clientDbContext.E_Positions.Any(ep => ep.PositionId == position.PositionId))
                       || (clientDbContext.EmployeeActuals.Any(x => x.PositionID == position.PositionId))
                       || (clientDbContext.EmployeesAllowedTakens.Any(ea => ea.TypeId == position.PositionId))
                       || (clientDbContext.ManagersPositions.Any(mp => mp.PositionID == position.PositionId))
                       || (clientDbContext.PositionsBudgets.Any(pb => pb.PositionID == position.PositionId))
                       || (clientDbContext.TimeCards.Any(tc => tc.PositionId == position.PositionId))
                       || (clientDbContext.PositionSalaryGrades.Any(psg => psg.PositionId == position.PositionId))
                       )
                    {
                        return Json(new { Message = CustomErrorMessages.ERROR_DELETING_RECORD_ALREADY_IN_USE, succeed = false }, JsonRequestBehavior.AllowGet);
                    }


                    Position positionInDb = clientDbContext.Positions.Where(x => x.PositionId == position.PositionId).SingleOrDefault();
                    if (positionInDb != null)
                    {
                        clientDbContext.Positions.Remove(positionInDb);
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
                return Json(new[] { position }.ToDataSourceResult(request, ModelState));
            }
        }


        public Tuple<bool, string> FindNULLCriteria(bool classificationValueNULL, string criteria, ExecViewHrk.EfClient.Position position, ClientDbContext clientDbContext)
        {
            
            PositionClassificationVm positionClassificationVm = new PositionClassificationVm();
            positionClassificationVm.ClassificationColumns = clientDbContext.PositionClassifications
                .Where(x => x.IsCriteriaApplicable == true).ToList(); 

            foreach (PositionClassification col in positionClassificationVm.ClassificationColumns)
            {
                if (col.ClassificationCriteria.ToUpper().Equals(PositionClassificationConst.B.ToUpper()) && position.BusinessUnitId == null)
                { classificationValueNULL = true; criteria = PositionClassificationConst.B.ToCamelCase(); break; }
                else if (col.ClassificationCriteria.ToUpper().Equals(PositionClassificationConst.J.ToUpper()) && position.JobId == null)
                { classificationValueNULL = true; criteria = PositionClassificationConst.J.ToCamelCase(); break; }
                else if (col.ClassificationCriteria.ToUpper().Equals(PositionClassificationConst.D.ToUpper()) && position.DepartmentId == null)
                { classificationValueNULL = true; criteria = PositionClassificationConst.D.ToCamelCase(); break; }
                else if (col.ClassificationCriteria.ToUpper().Equals(PositionClassificationConst.L.ToUpper()) && position.LocationId == null)
                { classificationValueNULL = true; criteria = PositionClassificationConst.L.ToCamelCase(); break; }
                else if (col.ClassificationCriteria.ToUpper().Equals(PositionClassificationConst.U1.ToUpper()) && position.UserDefinedSegment1Id == null)
                { classificationValueNULL = true; criteria = PositionClassificationConst.U1.ToCamelCase(); break; }
                else if (col.ClassificationCriteria.ToUpper().Equals(PositionClassificationConst.U2.ToUpper()) && position.UserDefinedSegment2Id == null)
                { classificationValueNULL = true; criteria = PositionClassificationConst.U2.ToCamelCase(); break; }
            }

            return new Tuple<bool, string>(classificationValueNULL, criteria);
        }

        private void PopulateBusinessUnits()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var businessUnitsList = new ClientDbContext(connString).BusinessUnits
                        .Select(c => new
                        {
                            BusinessUnitId = c.BusinessUnitId,
                            BusinessUnitCode = c.BusinessUnitCode,
                            BusinessUnitDescription = c.BusinessUnitDescription //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(s => s.BusinessUnitDescription).ToList();

                //companyCodesList.Insert(0, new CompanyCodeVm { CompanyCodeId = 0, CompanyCodeDescription = "--select one--" });

                ViewData["businessUnitssList"] = businessUnitsList;
                ViewData["defaultBusinessUnit"] = businessUnitsList.FirstOrDefault();
            }
        }


        private void PopulateJobs()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var jobsList = new ClientDbContext(connString).Jobs
                        .Select(c => new
                        {
                            JobId = c.JobId,
                            JobCode = c.JobCode,
                            title = c.JobDescription,
                            JobDescription = c.JobDescription //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(s => s.JobDescription).ToList();

                //companyCodesList.Insert(0, new CompanyCodeVm { CompanyCodeId = 0, CompanyCodeDescription = "--select one--" });

                ViewData["jobsList"] = jobsList;
                ViewData["defaultJob"] = jobsList.FirstOrDefault();
                
            }
        }

        private void PopulateDepartments()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var departmentsList = new ClientDbContext(connString).Departments.Where(x=> x.IsDeleted == false)
                        .Select(c => new
                        {
                            DepartmentId = c.DepartmentId,
                            DepartmentDescription = c.DepartmentDescription //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(s => s.DepartmentDescription).ToList();

                //companyCodesList.Insert(0, new CompanyCodeVm { CompanyCodeId = 0, CompanyCodeDescription = "--select one--" });

                ViewData["departmentsList"] = departmentsList;
                ViewData["defaultDepartment"] = departmentsList.FirstOrDefault();
            }
        }


        private void PopulateLocations()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var locationsList = new ClientDbContext(connString).Locations
                        .Select(c => new
                        {
                            LocationId = c.LocationId,
                            LocationDescription = c.LocationDescription //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(s => s.LocationDescription).ToList();

                //companyCodesList.Insert(0, new CompanyCodeVm { CompanyCodeId = 0, CompanyCodeDescription = "--select one--" });

                ViewData["locationsList"] = locationsList;
                ViewData["defaultLocation"] = locationsList.FirstOrDefault();
            }
        }

        private void PopulateUserDefinedSegment1s()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var userDefinedSegment1sList = new ClientDbContext(connString).UserDefinedSegment1s
                        .Select(c => new
                        {
                            UserDefinedSegment1Id = c.UserDefinedSegment1Id,
                            UserDefinedSegment1Description = c.UserDefinedSegment1Description //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(s => s.UserDefinedSegment1Description).ToList();

                //companyCodesList.Insert(0, new CompanyCodeVm { CompanyCodeId = 0, CompanyCodeDescription = "--select one--" });

                ViewData["userDefinedSegment1sList"] = userDefinedSegment1sList;
                ViewData["defaultUserDefinedSegment1"] = userDefinedSegment1sList.FirstOrDefault();
            }
        }


        private void PopulateUserDefinedSegment2s()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var userDefinedSegment2sList = new ClientDbContext(connString).UserDefinedSegment2s
                        .Select(c => new
                        {
                            UserDefinedSegment2Id = c.UserDefinedSegment2Id,
                            UserDefinedSegment2Description = c.UserDefinedSegment2Description //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(s => s.UserDefinedSegment2Description).ToList();

                //companyCodesList.Insert(0, new CompanyCodeVm { CompanyCodeId = 0, CompanyCodeDescription = "--select one--" });

                ViewData["userDefinedSegment2sList"] = userDefinedSegment2sList;
                ViewData["defaultUserDefinedSegment2"] = userDefinedSegment2sList.FirstOrDefault();
            }
        }


        private void PopulateReportToPositionIds()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var reportToPositionsList = new ClientDbContext(connString).Positions
                        .Select(c => new 
                        {
                            ReportToPositionId = c.PositionId,
                            ReportToPositionDescription = c.PositionDescription //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(s => s.ReportToPositionDescription).ToList();

                //reportToPositionsList.Insert(0, new ReportToPositionVm { ReportToPositionId = 0, ReportToPositionDescription = "--select one--" });

                ViewData["reportToPositionsList"] = reportToPositionsList;
                ViewData["defaultReportToPosition"] = reportToPositionsList.FirstOrDefault();
            }
        }


        public JsonResult GetPositions()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var positions = clientDbContext.Positions
                    .Where(x => x.IsPositionActive == true)
                    .Select(m => new
                    {
                        PositionId = m.PositionId,
                        PositionDescription = m.PositionDescription
                    }).OrderBy(x => x.PositionDescription).ToList();

                return Json(positions, JsonRequestBehavior.AllowGet);
            }

        }



        //public ActionResult GetClassificationColumns()
        //{
        //    string connString = User.Identity.GetClientConnectionString();

        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {

        //        PositionClassificationVm positionClassificationVm = new PositionClassificationVm();
        //        positionClassificationVm.ClassificationColumns = clientDbContext.PositionClassifications
        //            .Where(x => x.IsCriteriaApplicable == false)
        //            .Select(m => new PositionClassification
        //            {
        //                ClassificationCriteria = m.ClassificationCriteria,
        //                IsCriteriaApplicable = m.IsCriteriaApplicable
        //            }).OrderBy(x => x.ClassificationCriteria).ToList();


        //        //var positionClassificationCriterias = clientDbContext.PositionClassifications
        //        //    .Where(x => x.IsCriteriaApplicable == false)
        //        //    .Select(m => new
        //        //    {
        //        //        ClassificationCriteria = m.ClassificationCriteria,
        //        //        IsCriteriaApplicable = m.IsCriteriaApplicable
        //        //    }).OrderBy(x => x.ClassificationCriteria).ToList();

        //        return View("GetClassificationColumns", positionClassificationVm);
        //    }

        //}
    }
}