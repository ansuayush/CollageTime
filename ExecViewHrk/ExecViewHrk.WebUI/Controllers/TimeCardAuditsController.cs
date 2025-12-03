using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class TimeCardAuditsController : BaseController
    {
        ITimeCardAuditsRepository _timeCardAuditsRepo;

        public TimeCardAuditsController(ITimeCardAuditsRepository timeCardAuditsRepo)
        {
            _timeCardAuditsRepo = timeCardAuditsRepo;
        }
        // GET: TimeCardsAudits
        public ActionResult TimeCardAuditsMatrixPartial()
        {
            return View();
        }

        public ActionResult TimecardAudits_Read([DataSourceRequest] DataSourceRequest request)
        {
            var auditsList = _timeCardAuditsRepo.GetTimecardAuditList();
            //return Json(auditsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            return KendoCustomResult(auditsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ExportAuditreport()
        {

            var exportAuditList = new List<TimeCardAuditsVM>();
            Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/TimeCardAudits"));
            string filename = "TimeCardAudits" + DateTime.Now.ToString("_MM-dd-yyyy_HH-mm-ss-fff") + ".csv";
            string filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TimeCardAudits"), filename);

            if (ModelState.IsValid)
            {
                try
                {
                    exportAuditList = _timeCardAuditsRepo.GetTimecardAuditList();

                    if (exportAuditList.Count() > 0)
                    {
                        List<String> listExportTC = new List<string>();
                        listExportTC.Add("Time Card Id,File Number,Employee,Position,Company Code Description,Actual Date,Time In,Time Out,Audit Type,Audit User Id,Audit Date");

                        foreach (var exportTC in exportAuditList)
                        {
                            listExportTC.Add(exportTC.ToString());
                        }

                        System.IO.File.WriteAllLines(filePath, listExportTC);
                    }
                    else
                    {
                        filename = string.Empty;
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            else
            {
                return Json(new { succeed = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { fileName = filename, succeed = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileContentResult Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception(String.Format("File not exist"));

            byte[] filedata = null;
            string contentType = string.Empty;

            try
            {
                string fullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TimeCardAudits"), fileName);
                if (System.IO.File.Exists(fullPath))
                {
                    filedata = System.IO.File.ReadAllBytes(fullPath);
                    contentType = MimeMapping.GetMimeMapping(fullPath);
                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = fileName,
                        Inline = true,
                    };
                    Response.AppendHeader("Content-Disposition", cd.ToString());
                    System.IO.File.Delete(fullPath);
                }
                else
                {
                    throw new Exception(String.Format("File not exist", fileName));
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return File(filedata, contentType);
        }
    }
}