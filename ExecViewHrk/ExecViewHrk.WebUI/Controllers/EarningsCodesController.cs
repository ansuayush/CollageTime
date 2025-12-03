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
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using Newtonsoft.Json;

namespace ExecViewHrk.WebUI.Controllers
{
    public class EarningsCodesController : Controller
    {
        readonly IEarningsCodeRepository _earningsCodeRepo;

        public EarningsCodesController(IEarningsCodeRepository earningsCodeRepo)
        {
            _earningsCodeRepo = earningsCodeRepo;
        }
        // GET: EarningsCodes
        public ActionResult EarningsCodesMatrixPartial()
        {
            PopulateCompanyCodes();
            PopulateADPFieldMappings();
            return View();
        }

        public ActionResult EarningsCodesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            var earningsCodesList = _earningsCodeRepo.GetEarningsCodeList();
            return Json(earningsCodesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetEarningsCodeDetails(int earningsCodeId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PopulateCompanyCodes();
            PopulateADPFieldMappings();
            var objDepartment = _earningsCodeRepo.GetEarningsCodeDetails(earningsCodeId);
            var companyid = clientDbContext.EarningsCodes.Where(e => e.EarningsCodeId == earningsCodeId).Select(c => c.CompanyCodeId).FirstOrDefault();
            Session["companycodeid"] = companyid;
            return View("EarningsCodeDetails", objDepartment);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EarningsCodesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.EarningsCode earningsCode)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (earningsCode != null && ModelState.IsValid)
                {
                    var earningsCodeInDb = clientDbContext.EarningsCodes
                        .Where(x => x.EarningsCodeCode == earningsCode.EarningsCodeCode)
                        .SingleOrDefault();

                    if (earningsCodeInDb != null)
                    {
                        ModelState.AddModelError("", "The Earnings Code is already defined.");
                    }
                    else
                    {
                        var newEarningsCode = new EarningsCode
                        {
                            CompanyCodeId = earningsCode.CompanyCodeId,
                            EarningsCodeDescription = earningsCode.EarningsCodeDescription,
                            EarningsCodeCode = earningsCode.EarningsCodeCode,
                            ADPFieldMappingId = earningsCode.ADPFieldMappingId,
                            EarningsCodeOffset = earningsCode.EarningsCodeOffset,
                            DeductionCodeOffset = earningsCode.DeductionCodeOffset,
                            IsEarningsCodeActive = true,
                            TreatyCode = earningsCode.TreatyCode,
                            IsDefault = earningsCode.IsDefault
                            
                        };

                        clientDbContext.EarningsCodes.Add(newEarningsCode);
                        clientDbContext.SaveChanges();
                        earningsCode.EarningsCodeId = newEarningsCode.EarningsCodeId;
                    }
                }

                return Json(new[] { earningsCode }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EarningsCodesSaveAjax([DataSourceRequest] DataSourceRequest request
            , EarningCodeVm earningCodeVM)
        {            
            var errors = ModelState.Values.SelectMany(m => m.Errors);
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                EarningsCode earningRecordExists = null;
                bool result = false;
                int lastvalue = 0;
                int earnid = 0;
                if (earningCodeVM != null && ModelState.IsValid)
                {
                    var earningsCodeInDb = clientDbContext.EarningsCodes
                        .Where(x => x.EarningsCodeId == earningCodeVM.EarningsCodeId)
                        .SingleOrDefault();                                      
                    if (earningsCodeInDb != null)
                    {

                        if (earningCodeVM.IsDefault)
                        {
                            earningRecordExists = clientDbContext.EarningsCodes.Where(x => x.CompanyCodeId == earningCodeVM.CompanyCodeId && x.IsDefault == true && x.EarningsCodeId != earningsCodeInDb.EarningsCodeId).FirstOrDefault();
                        }
                        else
                        {
                            earningRecordExists = null;
                        }
                    }
                    else
                    {
                        if (earningCodeVM.IsDefault)
                        {
                            earningRecordExists = clientDbContext.EarningsCodes.Where(x => x.CompanyCodeId == earningCodeVM.CompanyCodeId && x.IsDefault == true).FirstOrDefault();
                        }
                        
                    }                    
                    if (earningRecordExists != null )
                    {
                        return Json(new { succeed = false, Message = "Is Default already exists for Company Code." }, JsonRequestBehavior.AllowGet);
                    }
                   
                    if (earningsCodeInDb != null)
                    {
                        earningsCodeInDb.CompanyCodeId = earningCodeVM.CompanyCodeId;
                        earningsCodeInDb.EarningsCodeCode = earningCodeVM.EarningsCode;
                        earningsCodeInDb.EarningsCodeDescription = earningCodeVM.EarningsCodeDescription;
                        earningsCodeInDb.ADPFieldMappingId = earningCodeVM.ADPFieldMappingId;
                        earningsCodeInDb.EarningsCodeOffset = earningCodeVM.EarningsCodeOffset;
                        earningsCodeInDb.DeductionCodeOffset = earningCodeVM.DeductionCodeOffset;
                        earningsCodeInDb.IsEarningsCodeActive = earningCodeVM.Active;
                        earningsCodeInDb.TreatyCode = earningCodeVM.TreatyCode;
                        earningsCodeInDb.IsDefault = earningCodeVM.IsDefault;
                        clientDbContext.SaveChanges();
                        result = true;
                        lastvalue = earningCodeVM.CompanyCodeId;
                        earnid = earningsCodeInDb.EarningsCodeId;
                        var oldearningid = earningCodeVM.OldEarningsCodeId;
                        if (result)
                        {
                            var earninglist = new List<Employee>();
                            if (earningCodeVM.IsDefault)
                            {
                                if (oldearningid != 0)
                                {
                                    earninglist = clientDbContext.Employees.Where(e => e.CompanyCodeId == lastvalue && e.EarningsCodeId==oldearningid).ToList();
                                }
                                if (earninglist != null)
                                {
                                    foreach (var item in earninglist)
                                    {
                                        item.EarningsCodeId = earnid;
                                        clientDbContext.SaveChanges();
                                    }

                                }
                            }
                            else
                            {
                                earninglist = clientDbContext.Employees.Where(e => e.CompanyCodeId == earningCodeVM.CompanyCodeId && e.EarningsCodeId==oldearningid).ToList();
                                if (earninglist != null)
                                {
                                    foreach (var item in earninglist)
                                    {
                                        item.EarningsCodeId = null;
                                    }
                                    clientDbContext.SaveChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        var newEarningsCode = new EarningsCode
                        {
                            CompanyCodeId = earningCodeVM.CompanyCodeId,
                            EarningsCodeDescription = earningCodeVM.EarningsCodeDescription,
                            EarningsCodeCode = earningCodeVM.EarningsCode,
                            ADPFieldMappingId = earningCodeVM.ADPFieldMappingId,
                            EarningsCodeOffset = earningCodeVM.EarningsCodeOffset,
                            DeductionCodeOffset = earningCodeVM.DeductionCodeOffset,
                            TreatyCode=earningCodeVM.TreatyCode,
                            IsEarningsCodeActive = true,
                            IsDefault=earningCodeVM.IsDefault
                        };
                        clientDbContext.EarningsCodes.Add(newEarningsCode);
                        clientDbContext.SaveChanges();
                        result = true;
                        lastvalue = newEarningsCode.CompanyCodeId;
                        earnid = newEarningsCode.EarningsCodeId;
                        earningCodeVM.EarningsCodeId = newEarningsCode.EarningsCodeId;
                        var oldearningid = earningCodeVM.OldEarningsCodeId;
                        if (result)
                        {
                            var earninglist = new List<Employee>();
                            if (earningCodeVM.IsDefault)
                            {
                                if (oldearningid != 0)
                                {
                                    earninglist = clientDbContext.Employees.Where(e => e.CompanyCodeId == lastvalue && e.EarningsCodeId == oldearningid).ToList();
                                }
                                if (earninglist != null)
                                {
                                    foreach (var item in earninglist)
                                    {
                                        item.EarningsCodeId = earnid;
                                    }
                                    clientDbContext.SaveChanges();
                                }
                            }
                            else
                            {
                                earninglist = clientDbContext.Employees.Where(e => e.CompanyCodeId == earningCodeVM.CompanyCodeId && e.EarningsCodeId == oldearningid).ToList();
                                if (earninglist != null)
                                {
                                    foreach (var item in earninglist)
                                    {
                                        item.EarningsCodeId = null;
                                    }

                                    clientDbContext.SaveChanges();
                                }
                            }
                        }
                    }
                }

                return Json(new[] { earningCodeVM }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EarningsCodesList_Destroy([DataSourceRequest] DataSourceRequest request
            , EarningCodeVm earningCodeVM)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (earningCodeVM != null)
                {
                    EarningsCode earningsCodeInDb = clientDbContext.EarningsCodes
                        .Where(x => x.EarningsCodeId == earningCodeVM.EarningsCodeId).SingleOrDefault();
                    if (clientDbContext.Employees.Where(x => x.EarningsCodeId == earningCodeVM.EarningsCodeId).Count() > 0)
                    {
                        return Json(new { succeed = false, Message = "The Earning Code " + CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (earningsCodeInDb != null)
                        {
                            clientDbContext.EarningsCodes.Remove(earningsCodeInDb);

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
                return Json(new { succeed = true }, JsonRequestBehavior.AllowGet);
            }
        }

        public string GetEarningsCodesList()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var earningsCodes = clientDbContext.EarningsCodes
                    .Where(x => x.IsEarningsCodeActive == true)
                    .Select(m => new
                    {
                        EarningsCodeId = m.EarningsCodeId,
                        EarningsCodeDescription = m.EarningsCodeDescription
                    }).OrderBy(x => x.EarningsCodeDescription).ToList();

                return JsonConvert.SerializeObject(earningsCodes);
            }
        }
       

        private void PopulateCompanyCodes()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var companyCodesList = new ClientDbContext(connString).CompanyCodes
                        .Select(c => new CompanyCodeVm
                        {
                            CompanyCodeId = c.CompanyCodeId,
                            CompanyCodeDescription = c.CompanyCodeDescription //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(s => s.CompanyCodeDescription).ToList();

                //companyCodesList.Insert(0, new CompanyCodeVm { CompanyCodeId = 0, CompanyCodeDescription = "--select one--" });

                ViewData["companyCodesList"] = companyCodesList;
                //ViewData["defaultCompanyCode"] = companyCodesList.First();
            }
        }

        private void PopulateADPFieldMappings()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var adpFieldMappingsList = new ClientDbContext(connString).ADPFieldMappings
                        .Select(c => new ADPFieldMappingVm
                        {
                            ADPFieldMappingId = c.ADPFieldMappingId,
                            ADPFieldMappingCode = "Earnings " + c.ADPFieldMappingCode //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(s => s.ADPFieldMappingId).ToList();

                //adpFieldMappingsList.Insert(0, new ADPFieldMappingVm { ADPFieldMappingId = 0, ADPFieldMappingCode = "--select one--" });

                ViewData["adpFieldMappingsList"] = adpFieldMappingsList;
                //ViewData["defaultADPFieldMapping"] = adpFieldMappingsList.First();
            }
        }
        public int GetIsdefaultcount()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            int companyid = Convert.ToInt32(Session["companycodeid"]);
            var isdefaultcount = clientDbContext.EarningsCodes.Where(x => x.CompanyCodeId == companyid && x.IsDefault).Select(d => d.IsDefault).Count();
            return isdefaultcount;
            
        }


    }
}