using AutoMapper;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Domain.Models;
using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class GeofenceController : Controller
    {

        IGeofenceRepository _geoRepo;

        public GeofenceController(IGeofenceRepository geoRepo)
        {
            _geoRepo = geoRepo;
        }
        // GET: Geofence
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _NewGeofenceCoordinate(string coordinates)
        {
            coordinates = Uri.UnescapeDataString(coordinates);
            var quaryString = coordinates;
            string[] words = quaryString.Split('~');
            var position = words[0];
            string[] coordinate = position.Split(',');
            GeofenceVM obj = new GeofenceVM();
            {
                obj.latitude = coordinate[0].Replace("(", "");
                obj.longitude = coordinate[1].Replace(")", "");
            }
            var model = obj;
            return PartialView(model);
            //return PartialView("_NewGeofenceCoordinate", model);
            //return PartialView("~/Views/Geofence/_NewGeofenceCoordinate.cshtml", model);
        }
        public ActionResult SaveGeofenceCoordinate(GeofenceVM model)
        {

            if (!ModelState.IsValid)
                return Json(new { Message = "Something went wrong!", succeed = false }, JsonRequestBehavior.AllowGet);
            GeofenceVM modelDM = Mapper.Map<GeofenceVM, GeofenceVM>(model);
            try
            {
                var GeofenceName = modelDM.GeofenceName;
                var Coordinate = modelDM.Coordinate;
                var latitude = modelDM.latitude;
                var longitude = modelDM.longitude;
                var Radius = modelDM.Radius;    
        
                if (_geoRepo.SaveGeofence(GeofenceName, Coordinate, latitude, longitude, Radius, User.Identity.Name))
                    return Json(new { Message = "Success", succeed = true }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Message = "Something went wrong!", succeed = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.InnerException.Message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public IEnumerable<GeofenceDM> GetDesignatedSupervisors1()
        {           
            List<GeofenceDM> list = _geoRepo.GetGeofenceDetails();
            return Mapper.Map<List<GeofenceDM>, List<GeofenceDM>>(list);
        }
        public JsonResult GetDesignatedSupervisors()
        {
            var data = _geoRepo.GetGeofenceDetails();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}