using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ExecViewHrk.WebUI.Controllers
{
    [Authorize]
    public class TrainingAdminController : Controller
    {
        public PartialViewResult SetupPartial()
        {
            try
            {
                Ensure();
            }
            catch (Exception ex)
            {
                ViewBag.SchemaError = ex.GetBaseException().Message;
            }
            return PartialView();
        }

        #region Lookups
        [HttpGet]
        public JsonResult GetCourseCodes()
        {
            try
            {
                using (var db = OpenDb())
                {
                    var list = db.TrainingCourseCodes.OrderBy(x => x.Code)
                        .Select(x => new TrainingLookupVm { Id = x.Id, Code = x.Code, Description = x.Description, Name = x.Code, IsActive = x.IsActive })
                        .ToList();
                    return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveCourseCode(int id, string code, string description, bool isActive)
        {
            using (var db = OpenDb())
            {
                TrainingCourseCode e;
                if (id > 0)
                {
                    e = db.TrainingCourseCodes.FirstOrDefault(x => x.Id == id);
                    if (e == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    e = new TrainingCourseCode();
                    db.TrainingCourseCodes.Add(e);
                }
                e.Code = code;
                e.Description = description;
                e.IsActive = isActive;
                db.SaveChanges();
                return Json(new { success = true, id = e.Id });
            }
        }

        [HttpGet]
        public JsonResult GetCourseCategories()
        {
            using (var db = OpenDb())
            {
                var list = db.TrainingCourseCategories.OrderBy(x => x.Name)
                    .Select(x => new TrainingLookupVm { Id = x.Id, Name = x.Name, Description = x.Description, IsActive = x.IsActive })
                    .ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveCourseCategory(int id, string name, string description, bool isActive)
        {
            using (var db = OpenDb())
            {
                TrainingCourseCategory e;
                if (id > 0)
                {
                    e = db.TrainingCourseCategories.FirstOrDefault(x => x.Id == id);
                    if (e == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    e = new TrainingCourseCategory();
                    db.TrainingCourseCategories.Add(e);
                }
                e.Name = name;
                e.Description = description;
                e.IsActive = isActive;
                db.SaveChanges();
                return Json(new { success = true, id = e.Id });
            }
        }

        [HttpGet]
        public JsonResult GetTrainingTypes()
        {
            using (var db = OpenDb())
            {
                var list = db.TrainingTypes.OrderBy(x => x.Name)
                    .Select(x => new TrainingLookupVm { Id = x.Id, Name = x.Name, IsActive = x.IsActive })
                    .ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveTrainingType(int id, string name, bool isActive)
        {
            using (var db = OpenDb())
            {
                TrainingType e;
                if (id > 0)
                {
                    e = db.TrainingTypes.FirstOrDefault(x => x.Id == id);
                    if (e == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    e = new TrainingType();
                    db.TrainingTypes.Add(e);
                }
                e.Name = name;
                e.IsActive = isActive;
                db.SaveChanges();
                return Json(new { success = true, id = e.Id });
            }
        }

        [HttpGet]
        public JsonResult GetStatuses()
        {
            using (var db = OpenDb())
            {
                var list = db.TrainingStatuses.OrderBy(x => x.SortOrder).ThenBy(x => x.Name)
                    .Select(x => new TrainingLookupVm { Id = x.Id, Name = x.Name, IsActive = x.IsActive, SortOrder = x.SortOrder })
                    .ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveStatus(int id, string name, bool isActive, int sortOrder)
        {
            using (var db = OpenDb())
            {
                TrainingStatus e;
                if (id > 0)
                {
                    e = db.TrainingStatuses.FirstOrDefault(x => x.Id == id);
                    if (e == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    e = new TrainingStatus();
                    db.TrainingStatuses.Add(e);
                }
                e.Name = name;
                e.IsActive = isActive;
                e.SortOrder = sortOrder;
                db.SaveChanges();
                return Json(new { success = true, id = e.Id });
            }
        }
        #endregion

        #region Classes
        [HttpGet]
        public JsonResult GetClasses()
        {
            try
            {
                using (var db = OpenDb())
                {
                    var codes = db.TrainingCourseCodes.ToDictionary(c => c.Id, c => c.Code);
                    var cats = db.TrainingCourseCategories.ToDictionary(c => c.Id, c => c.Name);
                    var list = db.TrainingClasses.OrderBy(c => c.Name).ToList().Select(c => new TrainingClassVm
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Cost = c.Cost,
                        Hours = c.Hours,
                        Location = c.Location,
                        CourseCodeId = c.CourseCodeId,
                        CourseCode = c.CourseCodeId.HasValue && codes.ContainsKey(c.CourseCodeId.Value) ? codes[c.CourseCodeId.Value] : null,
                        CourseCategoryId = c.CourseCategoryId,
                        CourseCategory = c.CourseCategoryId.HasValue && cats.ContainsKey(c.CourseCategoryId.Value) ? cats[c.CourseCategoryId.Value] : null,
                        IsActive = c.IsActive,
                        ExpirationDate = c.ExpirationDate.HasValue ? c.ExpirationDate.Value.ToString("MM/dd/yyyy") : null,
                        EnrollmentStartDate = c.EnrollmentStartDate.HasValue ? c.EnrollmentStartDate.Value.ToString("MM/dd/yyyy") : null,
                        EnrollmentEndDate = c.EnrollmentEndDate.HasValue ? c.EnrollmentEndDate.Value.ToString("MM/dd/yyyy") : null,
                        HasImage = c.Image != null && c.Image.Length > 0
                    }).ToList();
                    return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetClass(int id)
        {
            using (var db = OpenDb())
            {
                var c = db.TrainingClasses.FirstOrDefault(x => x.Id == id);
                if (c == null) return Json(new { success = false, message = "Not found." }, JsonRequestBehavior.AllowGet);
                return Json(new
                {
                    success = true,
                    data = new TrainingClassVm
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Cost = c.Cost,
                        Hours = c.Hours,
                        Location = c.Location,
                        CourseCodeId = c.CourseCodeId,
                        CourseCategoryId = c.CourseCategoryId,
                        IsActive = c.IsActive,
                        ExpirationDate = c.ExpirationDate.HasValue ? c.ExpirationDate.Value.ToString("MM/dd/yyyy") : null,
                        EnrollmentStartDate = c.EnrollmentStartDate.HasValue ? c.EnrollmentStartDate.Value.ToString("MM/dd/yyyy") : null,
                        EnrollmentEndDate = c.EnrollmentEndDate.HasValue ? c.EnrollmentEndDate.Value.ToString("MM/dd/yyyy") : null,
                        HasImage = c.Image != null && c.Image.Length > 0
                    }
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveClass(int id, string name, string description, decimal? cost, decimal? hours,
            string location, int? courseCodeId, int? courseCategoryId, bool isActive, string expirationDate,
            string enrollmentStartDate, string enrollmentEndDate)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Json(new { success = false, message = "Class name is required." });

            var enrollStart = ParseDate(enrollmentStartDate);
            var enrollEnd = ParseDate(enrollmentEndDate);
            if (enrollStart.HasValue && enrollEnd.HasValue && enrollEnd.Value < enrollStart.Value)
                return Json(new { success = false, message = "Enrollment end date must be on or after enrollment start date." });

            using (var db = OpenDb())
            {
                TrainingClass e;
                if (id > 0)
                {
                    e = db.TrainingClasses.FirstOrDefault(x => x.Id == id);
                    if (e == null) return Json(new { success = false, message = "Not found." });
                    e.ModifiedBy = User.Identity.Name;
                    e.ModifiedDate = DateTime.Now;
                }
                else
                {
                    e = new TrainingClass { CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now };
                    db.TrainingClasses.Add(e);
                }
                e.Name = name.Trim();
                e.Description = description;
                e.Cost = cost ?? 0;
                e.Hours = hours;
                e.Location = location;
                e.CourseCodeId = courseCodeId;
                e.CourseCategoryId = courseCategoryId;
                e.IsActive = isActive;
                e.ExpirationDate = ParseDate(expirationDate);
                e.EnrollmentStartDate = enrollStart;
                e.EnrollmentEndDate = enrollEnd;
                db.SaveChanges();
                return Json(new { success = true, id = e.Id });
            }
        }

        [HttpPost]
        public JsonResult DeleteClasses(string idsJson)
        {
            var ids = DeserializeIds(idsJson);
            using (var db = OpenDb())
            {
                var rows = db.TrainingClasses.Where(c => ids.Contains(c.Id)).ToList();
                db.TrainingClasses.RemoveRange(rows);
                db.SaveChanges();
                return Json(new { success = true });
            }
        }
        #endregion

        #region Tracks
        [HttpGet]
        public JsonResult GetTracks()
        {
            using (var db = OpenDb())
            {
                var classMap = db.TrainingClasses.ToDictionary(c => c.Id, c => c.Name);
                var links = db.TrainingTrackClasses.ToList().GroupBy(l => l.TrainingTrackId)
                    .ToDictionary(g => g.Key, g => g.OrderBy(x => x.SortOrder).ToList());
                var list = db.TrainingTracks.OrderBy(t => t.Name).ToList().Select(t =>
                {
                    List<TrainingTrackClass> linkList;
                    links.TryGetValue(t.Id, out linkList);
                    var classIds = linkList != null ? linkList.Select(l => l.TrainingClassId).ToList() : new List<int>();
                    return new TrainingTrackVm
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        TotalHours = t.TotalHours,
                        ExpirationDate = t.ExpirationDate.HasValue ? t.ExpirationDate.Value.ToString("MM/dd/yyyy") : null,
                        ClassInfo = t.ClassInfo,
                        ClassIds = classIds,
                        ClassNames = classIds.Where(id => classMap.ContainsKey(id)).Select(id => classMap[id]).ToList()
                    };
                }).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveTrack(int id, string name, string description, decimal totalHours, string expirationDate, string classIdsJson)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Json(new { success = false, message = "Track name is required." });

            var classIds = DeserializeIds(classIdsJson);
            using (var db = OpenDb())
            {
                TrainingTrack e;
                if (id > 0)
                {
                    e = db.TrainingTracks.FirstOrDefault(x => x.Id == id);
                    if (e == null) return Json(new { success = false, message = "Not found." });
                    e.ModifiedBy = User.Identity.Name;
                    e.ModifiedDate = DateTime.Now;
                }
                else
                {
                    e = new TrainingTrack { CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now };
                    db.TrainingTracks.Add(e);
                }
                e.Name = name.Trim();
                e.Description = description ?? "";
                e.TotalHours = totalHours;
                e.ExpirationDate = ParseDate(expirationDate);
                var classNames = db.TrainingClasses.Where(c => classIds.Contains(c.Id)).Select(c => c.Name).ToList();
                e.ClassInfo = string.Join(", ", classNames);
                db.SaveChanges();

                var existing = db.TrainingTrackClasses.Where(l => l.TrainingTrackId == e.Id).ToList();
                db.TrainingTrackClasses.RemoveRange(existing);
                int sort = 1;
                foreach (var cid in classIds)
                {
                    db.TrainingTrackClasses.Add(new TrainingTrackClass
                    {
                        TrainingTrackId = e.Id,
                        TrainingClassId = cid,
                        SortOrder = sort++
                    });
                }
                db.SaveChanges();
                return Json(new { success = true, id = e.Id });
            }
        }

        [HttpPost]
        public JsonResult DeleteTracks(string idsJson)
        {
            var ids = DeserializeIds(idsJson);
            using (var db = OpenDb())
            {
                var links = db.TrainingTrackClasses.Where(l => ids.Contains(l.TrainingTrackId)).ToList();
                db.TrainingTrackClasses.RemoveRange(links);
                var rows = db.TrainingTracks.Where(t => ids.Contains(t.Id)).ToList();
                db.TrainingTracks.RemoveRange(rows);
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        [HttpPost]
        public JsonResult CopyTrack(int id)
        {
            using (var db = OpenDb())
            {
                var src = db.TrainingTracks.FirstOrDefault(t => t.Id == id);
                if (src == null) return Json(new { success = false, message = "Not found." });
                var copy = new TrainingTrack
                {
                    Name = src.Name + " (Copy)",
                    Description = src.Description,
                    ClassInfo = src.ClassInfo,
                    TotalHours = src.TotalHours,
                    ExpirationDate = src.ExpirationDate,
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now
                };
                db.TrainingTracks.Add(copy);
                db.SaveChanges();
                var links = db.TrainingTrackClasses.Where(l => l.TrainingTrackId == id).ToList();
                foreach (var l in links)
                {
                    db.TrainingTrackClasses.Add(new TrainingTrackClass
                    {
                        TrainingTrackId = copy.Id,
                        TrainingClassId = l.TrainingClassId,
                        SortOrder = l.SortOrder
                    });
                }
                db.SaveChanges();
                return Json(new { success = true, id = copy.Id });
            }
        }
        #endregion

        #region Schedules
        [HttpGet]
        public JsonResult GetSchedules(int? classId)
        {
            using (var db = OpenDb())
            {
                var classes = db.TrainingClasses.ToDictionary(c => c.Id, c => c.Name);
                var q = db.TrainingClassSchedules.AsQueryable();
                if (classId.HasValue) q = q.Where(s => s.TrainingClassId == classId.Value);
                var list = q.OrderByDescending(s => s.StartDate).ToList().Select(s => new TrainingScheduleVm
                {
                    Id = s.Id,
                    TrainingClassId = s.TrainingClassId,
                    ClassName = classes.ContainsKey(s.TrainingClassId) ? classes[s.TrainingClassId] : "",
                    ScheduleName = s.ScheduleName,
                    StartDate = s.StartDate.ToString("MM/dd/yyyy"),
                    EndDate = s.EndDate.ToString("MM/dd/yyyy"),
                    Location = s.Location,
                    IsActive = s.IsActive
                }).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveSchedule(int id, int trainingClassId, string scheduleName, string startDate, string endDate, string location, bool isActive)
        {
            var start = ParseDate(startDate);
            var end = ParseDate(endDate);
            if (!start.HasValue || !end.HasValue)
                return Json(new { success = false, message = "Start and End dates are required." });
            if (end.Value < start.Value)
                return Json(new { success = false, message = "End date must be on or after start date." });

            using (var db = OpenDb())
            {
                if (!db.TrainingClasses.Any(c => c.Id == trainingClassId))
                    return Json(new { success = false, message = "Invalid class." });

                TrainingClassSchedule e;
                if (id > 0)
                {
                    e = db.TrainingClassSchedules.FirstOrDefault(x => x.Id == id);
                    if (e == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    e = new TrainingClassSchedule();
                    db.TrainingClassSchedules.Add(e);
                }
                e.TrainingClassId = trainingClassId;
                e.ScheduleName = string.IsNullOrWhiteSpace(scheduleName)
                    ? start.Value.ToString("MM/dd/yyyy") + " - " + end.Value.ToString("MM/dd/yyyy")
                    : scheduleName;
                e.StartDate = start.Value;
                e.EndDate = end.Value;
                e.Location = location;
                e.IsActive = isActive;
                db.SaveChanges();
                return Json(new { success = true, id = e.Id });
            }
        }

        [HttpPost]
        public JsonResult DeleteSchedules(string idsJson)
        {
            var ids = DeserializeIds(idsJson);
            using (var db = OpenDb())
            {
                var rows = db.TrainingClassSchedules.Where(s => ids.Contains(s.Id)).ToList();
                db.TrainingClassSchedules.RemoveRange(rows);
                db.SaveChanges();
                return Json(new { success = true });
            }
        }
        #endregion

        private static DateTime? ParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            DateTime d;
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) return d.Date;
            if (DateTime.TryParse(value, out d)) return d.Date;
            return null;
        }

        private static List<int> DeserializeIds(string idsJson)
        {
            if (string.IsNullOrWhiteSpace(idsJson)) return new List<int>();
            try
            {
                return new JavaScriptSerializer().Deserialize<List<int>>(idsJson) ?? new List<int>();
            }
            catch
            {
                return new List<int>();
            }
        }

        private ClientDbContext OpenDb()
        {
            string conn = User.Identity.GetClientConnectionString();
            var db = new ClientDbContext(conn);
            TrainingModuleSchemaHelper.EnsureSchema(db);
            return db;
        }

        private void Ensure()
        {
            using (OpenDb()) { }
        }
    }
}
