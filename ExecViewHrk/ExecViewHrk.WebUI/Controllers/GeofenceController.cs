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
using System.Globalization;
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
            if (string.IsNullOrWhiteSpace(coordinates))
                return PartialView(new GeofenceVM());

            coordinates = Uri.UnescapeDataString(coordinates);
            string[] words = coordinates.Split(new[] { '~' }, StringSplitOptions.None);
            var model = new GeofenceVM();

            // Preferred format from map: lat~lng~address~name
            decimal latVal;
            decimal lngVal;
            if (words.Length >= 2
                && !words[0].Contains("(")
                && decimal.TryParse(words[0].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out latVal)
                && decimal.TryParse(words[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out lngVal))
            {
                model.latitude = words[0].Trim();
                model.longitude = words[1].Trim();
                model.PlaceAddress = words.Length > 2 ? words[2] : null;
                model.PlaceName = words.Length > 3 ? words[3] : null;
            }
            else
            {
                // Legacy format: (lat, lng)~address~name
                var position = words[0];
                string[] coordinate = position.Split(',');
                if (coordinate.Length >= 2)
                {
                    model.latitude = coordinate[0].Replace("(", "").Trim();
                    model.longitude = coordinate[1].Replace(")", "").Trim();
                }
                model.PlaceAddress = words.Length > 1 ? words[1] : null;
                model.PlaceName = words.Length > 2 ? words[2] : null;
            }

            model.Coordinate = string.IsNullOrEmpty(model.latitude) || string.IsNullOrEmpty(model.longitude)
                ? null
                : model.latitude + "," + model.longitude;
            model.Radius = "1000";

            return PartialView(model);
        }
        public ActionResult SaveGeofenceCoordinate(GeofenceVM model)
        {
            if (!ModelState.IsValid)
            {
                // Keep save usable even if optional VM fields fail binding
                ModelState.Clear();
            }
            if (model == null)
                return Json(new { Message = "Invalid request.", succeed = false }, JsonRequestBehavior.AllowGet);

            try
            {
                var GeofenceName = (model.GeofenceName ?? "").Trim();
                var latitude = (model.latitude ?? "").Trim();
                var longitude = (model.longitude ?? "").Trim();
                var Radius = (model.Radius ?? "").Trim();
                var Coordinate = string.IsNullOrWhiteSpace(model.Coordinate)
                    ? latitude + "," + longitude
                    : model.Coordinate.Trim();

                if (string.IsNullOrWhiteSpace(GeofenceName) || string.IsNullOrWhiteSpace(latitude) || string.IsNullOrWhiteSpace(longitude) || string.IsNullOrWhiteSpace(Radius))
                    return Json(new { Message = "Geofence name, latitude, longitude and radius are required.", succeed = false }, JsonRequestBehavior.AllowGet);

                if (_geoRepo.SaveGeofence(GeofenceName, Coordinate, latitude, longitude, Radius, User.Identity.Name))
                    return Json(new { Message = "Success", succeed = true }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Message = "Something went wrong!", succeed = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { Message = message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public IEnumerable<GeofenceDM> GetDesignatedSupervisors1()
        {           
            List<GeofenceDM> list = _geoRepo.GetGeofenceDetails();
            return Mapper.Map<List<GeofenceDM>, List<GeofenceDM>>(list);
        }
        public JsonResult GetDesignatedSupervisors()
        {
            var data = _geoRepo.GetGeofenceDetails() ?? new List<GeofenceDM>();
            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
    }
}