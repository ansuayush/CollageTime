using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    [Authorize]
    public class TrainingEnrollmentController : Controller
    {
        public PartialViewResult AssignPartial()
        {
            try { Ensure(); }
            catch (Exception ex) { ViewBag.SchemaError = ex.GetBaseException().Message; }
            ViewBag.IsSelfService = false;
            return PartialView("EnrollmentPartial");
        }

        public PartialViewResult MyTrainingPartial()
        {
            try { Ensure(); }
            catch (Exception ex) { ViewBag.SchemaError = ex.GetBaseException().Message; }
            ViewBag.IsSelfService = true;
            return PartialView("EnrollmentPartial");
        }

        [HttpGet]
        public JsonResult GetLookups()
        {
            using (var db = OpenDb())
            {
                int? currentEmployeeId = ResolveTargetEmployeeId(db, forceSelf: User.Identity.GetRequestType() == "IsSelfService");
                string currentEmployeeName = null;
                if (currentEmployeeId.HasValue)
                {
                    var emp = db.Employees.FirstOrDefault(e => e.EmployeeId == currentEmployeeId.Value);
                    if (emp != null)
                    {
                        var person = db.Persons.FirstOrDefault(p => p.PersonId == emp.PersonId);
                        currentEmployeeName = person != null
                            ? ((person.Lastname + ", " + person.Firstname).Trim() + " - " + emp.EmploymentNumber)
                            : ("Employee - " + emp.EmploymentNumber);
                    }
                }

                return Json(new
                {
                    success = true,
                    currentEmployeeId,
                    currentEmployeeName,
                    types = db.TrainingTypes.Where(t => t.IsActive).OrderBy(t => t.Name)
                        .Select(t => new { t.Id, t.Name }).ToList(),
                    statuses = db.TrainingStatuses.Where(s => s.IsActive).OrderBy(s => s.SortOrder)
                        .Select(s => new { s.Id, s.Name }).ToList(),
                    classes = db.TrainingClasses.Where(c => c.IsActive).OrderBy(c => c.Name)
                        .Select(c => new
                        {
                            c.Id,
                            c.Name,
                            c.Cost,
                            c.Hours,
                            c.Location,
                            c.CourseCodeId,
                            c.CourseCategoryId,
                            ExpirationDate = c.ExpirationDate
                        }).ToList()
                        .Select(c => new
                        {
                            c.Id,
                            c.Name,
                            c.Cost,
                            c.Hours,
                            c.Location,
                            c.CourseCodeId,
                            c.CourseCategoryId,
                            ExpirationDate = c.ExpirationDate.HasValue ? c.ExpirationDate.Value.ToString("MM/dd/yyyy") : null
                        }).ToList(),
                    tracks = db.TrainingTracks.OrderBy(t => t.Name)
                        .Select(t => new { t.Id, t.Name }).ToList(),
                    courseCodes = db.TrainingCourseCodes.Where(c => c.IsActive).OrderBy(c => c.Code)
                        .Select(c => new { c.Id, c.Code }).ToList(),
                    courseCategories = db.TrainingCourseCategories.Where(c => c.IsActive).OrderBy(c => c.Name)
                        .Select(c => new { c.Id, c.Name }).ToList()
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetSchedulesForClass(int classId)
        {
            using (var db = OpenDb())
            {
                var list = db.TrainingClassSchedules
                    .Where(s => s.TrainingClassId == classId && s.IsActive)
                    .OrderBy(s => s.StartDate)
                    .ToList()
                    .Select(s => new
                    {
                        s.Id,
                        Name = !string.IsNullOrWhiteSpace(s.ScheduleName)
                            ? s.ScheduleName
                            : (s.StartDate.ToString("MM/dd/yyyy") + " - " + s.EndDate.ToString("MM/dd/yyyy")),
                        StartDate = s.StartDate.ToString("MM/dd/yyyy"),
                        EndDate = s.EndDate.ToString("MM/dd/yyyy"),
                        s.Location
                    }).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetClassDefaults(int classId)
        {
            using (var db = OpenDb())
            {
                var c = db.TrainingClasses.FirstOrDefault(x => x.Id == classId);
                if (c == null) return Json(new { success = false, message = "Class not found." }, JsonRequestBehavior.AllowGet);

                var trackLink = db.TrainingTrackClasses.Where(l => l.TrainingClassId == classId)
                    .OrderBy(l => l.SortOrder).FirstOrDefault();
                int? trackId = trackLink != null ? trackLink.TrainingTrackId : (int?)null;
                string trackName = null;
                if (trackId.HasValue)
                {
                    var track = db.TrainingTracks.FirstOrDefault(t => t.Id == trackId.Value);
                    trackName = track != null ? track.Name : null;
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        c.Cost,
                        c.Hours,
                        c.Location,
                        c.CourseCodeId,
                        c.CourseCategoryId,
                        ExpirationDate = c.ExpirationDate.HasValue ? c.ExpirationDate.Value.ToString("MM/dd/yyyy") : null,
                        EnrollmentStartDate = c.EnrollmentStartDate.HasValue ? c.EnrollmentStartDate.Value.ToString("MM/dd/yyyy") : null,
                        EnrollmentEndDate = c.EnrollmentEndDate.HasValue ? c.EnrollmentEndDate.Value.ToString("MM/dd/yyyy") : null,
                        TrainingTrackId = trackId,
                        TrainingTrackName = trackName
                    }
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult SearchEmployees(string q)
        {
            using (var db = OpenDb())
            {
                q = (q ?? "").Trim();
                var emps = db.Employees.AsQueryable();
                if (!string.IsNullOrEmpty(q))
                {
                    int empNum;
                    if (int.TryParse(q, out empNum))
                        emps = emps.Where(x => x.EmploymentNumber == empNum || x.EmployeeId == empNum);
                    else
                    {
                        // Match by person name when non-numeric
                        var matchingPersonIds = db.Persons
                            .Where(p => (p.Firstname + " " + p.Lastname).Contains(q)
                                || (p.Lastname + ", " + p.Firstname).Contains(q)
                                || p.Lastname.Contains(q)
                                || p.Firstname.Contains(q))
                            .Select(p => p.PersonId)
                            .Take(100)
                            .ToList();
                        emps = emps.Where(x => matchingPersonIds.Contains(x.PersonId));
                    }
                }
                var list = emps.OrderByDescending(x => x.EmploymentNumber).Take(50).ToList();
                var personIds = list.Where(x => x.PersonId > 0).Select(x => x.PersonId).Distinct().ToList();
                var people = db.Persons.Where(p => personIds.Contains(p.PersonId))
                    .ToDictionary(p => p.PersonId, p => (p.Lastname + ", " + p.Firstname).Trim());

                var data = list.Select(x => new
                {
                    x.EmployeeId,
                    Name = (people.ContainsKey(x.PersonId) ? people[x.PersonId] : "Employee") + " - " + x.EmploymentNumber
                }).ToList();
                return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetEnrollments(int? employeeId, int? classId)
        {
            using (var db = OpenDb())
            {
                bool isSelf = User.Identity.GetRequestType() == "IsSelfService" || Request["self"] == "1";
                int? targetEmployeeId = ResolveTargetEmployeeId(db, forceSelf: isSelf);
                if (isSelf)
                    employeeId = targetEmployeeId;

                var q = db.TrainingEmployees.AsQueryable();
                if (employeeId.HasValue) q = q.Where(e => e.EmployeeId == employeeId.Value);
                else if (!(User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators") || User.IsInRole("ClientManagers") || User.IsInRole("ClientAdminsMultipleCompanies")))
                {
                    if (targetEmployeeId.HasValue)
                        q = q.Where(e => e.EmployeeId == targetEmployeeId.Value);
                    else
                        q = q.Where(e => false);
                }

                if (classId.HasValue) q = q.Where(e => e.TrainingClassId == classId.Value);

                var rows = q.OrderByDescending(e => e.EnrollmentDate ?? e.CreatedOn).Take(500).ToList();
                var list = MapEnrollments(db, rows);
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetEnrollment(int id)
        {
            using (var db = OpenDb())
            {
                var row = db.TrainingEmployees.FirstOrDefault(e => e.Id == id);
                if (row == null) return Json(new { success = false, message = "Not found." }, JsonRequestBehavior.AllowGet);
                if (!CanAccessEnrollment(db, row))
                    return Json(new { success = false, message = "Not authorized." }, JsonRequestBehavior.AllowGet);
                return Json(new { success = true, data = MapEnrollments(db, new List<TrainingEmployee> { row }).First() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveEnrollment(TrainingEnrollmentSaveVm model)
        {
            if (model == null || model.EmployeeId <= 0)
                return Json(new { success = false, message = "Employee is required." });
            if (!model.TrainingClassId.HasValue || model.TrainingClassId.Value <= 0)
                return Json(new { success = false, message = "Class is required." });

            using (var db = OpenDb())
            {
                bool isSelf = User.Identity.GetRequestType() == "IsSelfService";
                int? selfEmp = ResolveTargetEmployeeId(db, forceSelf: true);
                if (isSelf)
                {
                    if (!selfEmp.HasValue || selfEmp.Value != model.EmployeeId)
                        return Json(new { success = false, message = "You can only enroll yourself." });
                }

                var cls = db.TrainingClasses.FirstOrDefault(c => c.Id == model.TrainingClassId.Value);
                if (cls == null) return Json(new { success = false, message = "Class not found." });

                TrainingClassSchedule schedule = null;
                if (model.TrainingClassScheduleId.HasValue && model.TrainingClassScheduleId.Value > 0)
                {
                    schedule = db.TrainingClassSchedules.FirstOrDefault(s => s.Id == model.TrainingClassScheduleId.Value);
                    if (schedule == null || schedule.TrainingClassId != cls.Id)
                        return Json(new { success = false, message = "Selected schedule does not belong to the selected class." });
                }

                var emp = db.Employees.FirstOrDefault(x => x.EmployeeId == model.EmployeeId);
                if (emp == null) return Json(new { success = false, message = "Employee not found." });

                TrainingEmployee row;
                if (model.Id > 0)
                {
                    row = db.TrainingEmployees.FirstOrDefault(x => x.Id == model.Id);
                    if (row == null) return Json(new { success = false, message = "Not found." });
                    if (!CanAccessEnrollment(db, row)) return Json(new { success = false, message = "Not authorized." });
                    row.ModifiedBy = User.Identity.Name;
                    row.ModifiedOn = DateTime.Now;
                }
                else
                {
                    row = new TrainingEmployee
                    {
                        CreatedBy = User.Identity.Name,
                        CreatedOn = DateTime.Now
                    };
                    db.TrainingEmployees.Add(row);
                }

                row.EmployeeId = model.EmployeeId;
                row.PersonId = emp.PersonId > 0 ? emp.PersonId : (int?)null;
                row.TrainingTypeId = model.TrainingTypeId;
                row.TrainingClassId = cls.Id;
                row.ClassName = cls.Name;
                row.TrainingClassScheduleId = schedule != null ? schedule.Id : model.TrainingClassScheduleId;
                row.TrainingTrackId = model.TrainingTrackId;
                if (model.TrainingTrackId.HasValue)
                {
                    var track = db.TrainingTracks.FirstOrDefault(t => t.Id == model.TrainingTrackId.Value);
                    row.TrainingTrackName = track != null ? track.Name : null;
                }
                else row.TrainingTrackName = null;

                row.EnrollmentDate = ParseDate(model.EnrollmentDate) ?? DateTime.Today;
                row.EnrollmentStartDate = ParseDate(model.EnrollmentStartDate);
                row.StartDate = ParseDate(model.StartDate) ?? (schedule != null ? schedule.StartDate : (DateTime?)null);
                row.EndDate = ParseDate(model.EndDate) ?? (schedule != null ? schedule.EndDate : (DateTime?)null);
                row.Location = !string.IsNullOrWhiteSpace(model.Location) ? model.Location : (schedule != null ? schedule.Location : cls.Location);
                row.Hours = model.Hours ?? cls.Hours;
                row.Cost = model.Cost ?? cls.Cost;
                row.StatusId = model.StatusId;
                row.CompletionDate = ParseDate(model.CompletionDate);
                row.CompletionStatusId = model.CompletionStatusId;
                row.ExpirationDate = ParseDate(model.ExpirationDate) ?? cls.ExpirationDate;
                row.Notes = model.Notes;
                row.Instructor = model.Instructor;
                row.CourseCodeId = cls.CourseCodeId;
                row.CourseCategoryId = cls.CourseCategoryId;

                db.SaveChanges();
                return Json(new { success = true, id = row.Id });
            }
        }

        [HttpPost]
        public JsonResult DeleteEnrollments(string idsJson)
        {
            if (User.Identity.GetRequestType() == "IsSelfService")
                return Json(new { success = false, message = "Not authorized." });

            var ids = new List<int>();
            try
            {
                ids = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<int>>(idsJson) ?? new List<int>();
            }
            catch { }

            using (var db = OpenDb())
            {
                var rows = db.TrainingEmployees.Where(e => ids.Contains(e.Id)).ToList();
                db.TrainingEmployees.RemoveRange(rows);
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        private List<TrainingEnrollmentVm> MapEnrollments(ClientDbContext db, List<TrainingEmployee> rows)
        {
            if (!rows.Any()) return new List<TrainingEnrollmentVm>();
            var empIds = rows.Select(r => r.EmployeeId).Distinct().ToList();
            var emps = db.Employees.Where(e => empIds.Contains(e.EmployeeId)).ToList();
            var personIds = emps.Where(e => e.PersonId > 0).Select(e => e.PersonId).Distinct().ToList();
            var people = db.Persons.Where(p => personIds.Contains(p.PersonId))
                .ToDictionary(p => p.PersonId, p => (p.Lastname + ", " + p.Firstname).Trim());
            var empNames = emps.ToDictionary(e => e.EmployeeId,
                e => (people.ContainsKey(e.PersonId) ? people[e.PersonId] : "Employee") + " - " + e.EmploymentNumber);

            var types = db.TrainingTypes.ToDictionary(t => t.Id, t => t.Name);
            var statuses = db.TrainingStatuses.ToDictionary(s => s.Id, s => s.Name);
            var codes = db.TrainingCourseCodes.ToDictionary(c => c.Id, c => c.Code);
            var cats = db.TrainingCourseCategories.ToDictionary(c => c.Id, c => c.Name);
            var schedules = db.TrainingClassSchedules.ToDictionary(s => s.Id, s => s.ScheduleName);

            return rows.Select(r =>
            {
                string statusName = r.StatusId.HasValue && statuses.ContainsKey(r.StatusId.Value) ? statuses[r.StatusId.Value] : null;
                bool enrolled = !string.IsNullOrEmpty(statusName) &&
                    (statusName.IndexOf("Enroll", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     statusName.IndexOf("Complete", StringComparison.OrdinalIgnoreCase) >= 0);
                bool completed = !string.IsNullOrEmpty(statusName) &&
                    statusName.IndexOf("Complete", StringComparison.OrdinalIgnoreCase) >= 0;

                return new TrainingEnrollmentVm
                {
                    Id = r.Id,
                    EmployeeId = r.EmployeeId,
                    EmployeeName = empNames.ContainsKey(r.EmployeeId) ? empNames[r.EmployeeId] : ("#" + r.EmployeeId),
                    TrainingTypeId = r.TrainingTypeId,
                    TrainingType = r.TrainingTypeId.HasValue && types.ContainsKey(r.TrainingTypeId.Value) ? types[r.TrainingTypeId.Value] : null,
                    TrainingClassId = r.TrainingClassId,
                    ClassName = r.ClassName,
                    TrainingClassScheduleId = r.TrainingClassScheduleId,
                    ScheduleName = r.TrainingClassScheduleId.HasValue && schedules.ContainsKey(r.TrainingClassScheduleId.Value) ? schedules[r.TrainingClassScheduleId.Value] : null,
                    TrainingTrackId = r.TrainingTrackId,
                    TrainingTrackName = r.TrainingTrackName,
                    EnrollmentDate = Fmt(r.EnrollmentDate),
                    EnrollmentStartDate = Fmt(r.EnrollmentStartDate),
                    StartDate = Fmt(r.StartDate),
                    EndDate = Fmt(r.EndDate),
                    Location = r.Location,
                    Hours = r.Hours,
                    Cost = r.Cost,
                    StatusId = r.StatusId,
                    Status = statusName,
                    CompletionDate = Fmt(r.CompletionDate),
                    CompletionStatusId = r.CompletionStatusId,
                    CompletionStatus = r.CompletionStatusId.HasValue && statuses.ContainsKey(r.CompletionStatusId.Value) ? statuses[r.CompletionStatusId.Value] : null,
                    ExpirationDate = Fmt(r.ExpirationDate),
                    Notes = r.Notes,
                    Instructor = r.Instructor,
                    CourseCodeId = r.CourseCodeId,
                    CourseCode = r.CourseCodeId.HasValue && codes.ContainsKey(r.CourseCodeId.Value) ? codes[r.CourseCodeId.Value] : null,
                    CourseCategoryId = r.CourseCategoryId,
                    CourseCategory = r.CourseCategoryId.HasValue && cats.ContainsKey(r.CourseCategoryId.Value) ? cats[r.CourseCategoryId.Value] : null,
                    IsEnrolled = enrolled,
                    IsCompleted = completed,
                    DocumentName = r.DocumentName
                };
            }).ToList();
        }

        private bool CanAccessEnrollment(ClientDbContext db, TrainingEmployee row)
        {
            if (User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators")
                || User.IsInRole("ClientManagers") || User.IsInRole("ClientAdminsMultipleCompanies"))
                return true;
            int? self = ResolveTargetEmployeeId(db, forceSelf: true);
            return self.HasValue && self.Value == row.EmployeeId;
        }

        private int? ResolveTargetEmployeeId(ClientDbContext db, bool forceSelf)
        {
            int? personId = null;
            if (forceSelf || User.Identity.GetRequestType() == "IsSelfService")
            {
                string userName = User.Identity.Name ?? "";
                var byUser = db.UserNamesPersons.FirstOrDefault(u => u.UserName == userName);
                if (byUser != null) personId = byUser.PersonID;
                else
                {
                    var person = db.Persons.FirstOrDefault(p => p.eMail == userName);
                    if (person != null) personId = person.PersonId;
                }
            }
            else
            {
                object selected = SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID);
                if (selected != null)
                {
                    try { personId = Convert.ToInt32(selected); }
                    catch { personId = null; }
                }
            }
            if (!personId.HasValue || personId.Value <= 0) return null;
            var emp = db.Employees.Where(e => e.PersonId == personId.Value)
                .OrderByDescending(e => e.EmploymentNumber).FirstOrDefault();
            return emp != null ? emp.EmployeeId : (int?)null;
        }

        private static string Fmt(DateTime? d)
        {
            return d.HasValue ? d.Value.ToString("MM/dd/yyyy") : null;
        }

        private static DateTime? ParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            DateTime d;
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) return d.Date;
            if (DateTime.TryParse(value, out d)) return d.Date;
            return null;
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
