(function () {
    var $root = $("#trEnrollRoot");
    if (!$root.length) return;

    function url(n) { return $root.attr("data-" + n) || ""; }
    function isSelf() { return $root.attr("data-self") === "1"; }
    function loggedInEmployeeId() {
        return selfEmployeeId || $root.attr("data-employee-id") || "";
    }
    function esc(s) {
        return String(s == null ? "" : s).replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;");
    }
    function toDateInput(val) {
        if (!val) return "";
        var s = String(val).trim();
        var mdy = s.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})$/);
        if (mdy) return mdy[3] + "-" + ("0" + mdy[1]).slice(-2) + "-" + ("0" + mdy[2]).slice(-2);
        var iso = s.match(/^(\d{4})-(\d{2})-(\d{2})/);
        if (iso) return iso[1] + "-" + iso[2] + "-" + iso[3];
        var d = new Date(s);
        if (!isNaN(d.getTime())) {
            return d.getFullYear() + "-" + ("0" + (d.getMonth() + 1)).slice(-2) + "-" + ("0" + d.getDate()).slice(-2);
        }
        return "";
    }

    var lookups = { types: [], statuses: [], classes: [], tracks: [], courseCodes: [], courseCategories: [] };
    var editId = 0;
    var selfEmployeeId = null;

    function opt(list, valueKey, textKey, selected) {
        var html = "<option value=''>--Select--</option>";
        (list || []).forEach(function (x) {
            html += "<option value='" + x[valueKey] + "'" + (String(x[valueKey]) === String(selected) ? " selected" : "") + ">" + esc(x[textKey]) + "</option>";
        });
        return html;
    }

    function loadLookups(done) {
        $.getJSON(url("lookups-url"), { self: isSelf() ? "1" : "0" }, function (r) {
            if (!r || !r.success) return;
            lookups.types = r.types || [];
            lookups.statuses = r.statuses || [];
            lookups.classes = r.classes || [];
            lookups.tracks = r.tracks || [];
            lookups.courseCodes = r.courseCodes || [];
            lookups.courseCategories = r.courseCategories || [];
            if (r.currentEmployeeId) selfEmployeeId = r.currentEmployeeId;
            if (!selfEmployeeId) selfEmployeeId = $root.attr("data-employee-id") || null;
            window._trCurrentEmployeeName = r.currentEmployeeName || "You";
            if (done) done();
        });
    }

    function loadGrid() {
        var params = { self: isSelf() ? "1" : "0" };
        $.getJSON(url("list-url"), params, function (r) {
            var $tb = $("#trEnrollTable tbody").empty();
            var rows = (r && r.data) || [];
            if (!rows.length) {
                $tb.append("<tr><td colspan='" + (isSelf() ? 10 : 11) + "' class='text-muted'>No training enrollments.</td></tr>");
                return;
            }
            rows.forEach(function (row) {
                var chk = isSelf() ? "" : "<td><input type='checkbox' class='tr-enroll-chk' value='" + row.Id + "'/></td>";
                $tb.append("<tr data-id='" + row.Id + "'>" + chk +
                    "<td>" + esc(row.EmployeeName) + "</td>" +
                    "<td>" + esc(row.ClassName || "") + "</td>" +
                    "<td>" + esc(row.CourseCode || "") + "</td>" +
                    "<td>" + esc(row.Status || "") + "</td>" +
                    "<td><a href='javascript:void(0)' class='tr-toggle-enroll' data-id='" + row.Id + "' title='" + (row.IsEnrolled ? "Click to unenroll" : "Click to enroll") + "'><span class='label " + (row.IsEnrolled ? "label-success" : "label-danger") + "'>" + (row.IsEnrolled ? "Yes" : "No") + "</span></a></td>" +
                    "<td><span class='label " + (row.IsCompleted ? "label-success" : "label-danger") + "'>" + (row.IsCompleted ? "Yes" : "No") + "</span></td>" +
                    "<td>" + esc(row.StartDate || "") + "</td>" +
                    "<td>" + esc(row.EndDate || "") + "</td>" +
                    "<td>" + esc(row.ExpirationDate || "") + "</td>" +
                    "<td><a href='javascript:void(0)' class='tr-view-enroll' data-id='" + row.Id + "'>View</a></td></tr>");
            });
        });
    }

    function formHtml(d) {
        d = d || {};
        var empField;
        if (isSelf()) {
            empField = "<input type='hidden' id='trEmpId' value='" + (d.EmployeeId || loggedInEmployeeId()) + "'/>";
        } else {
            empField = "<div class='form-group'><label>Select Employee(s)</label>" +
                "<select class='form-control' id='trEmpId'><option value='" + (d.EmployeeId || "") + "'>" + esc(d.EmployeeName || "--Select--") + "</option></select>" +
                "<input class='form-control m-t-5' id='trEmpSearch' placeholder='Type employment # to search...'/></div>";
        }

        return "<div class='row'>" +
            "<div class='col-md-6'>" + empField +
            "<div class='form-group'><label>Training Type</label><select class='form-control' id='trType'>" + opt(lookups.types, "Id", "Name", d.TrainingTypeId) + "</select></div>" +
            "<div class='form-group'><label>Class Name</label><select class='form-control' id='trClass'>" + opt(lookups.classes, "Id", "Name", d.TrainingClassId) + "</select></div>" +
            "<div class='form-group'><label>Class Schedule</label><select class='form-control' id='trSchedule'><option value=''>--Select--</option></select></div>" +
            "<div class='form-group'><label>Cost</label><input type='number' step='0.01' class='form-control' id='trCost' value='" + (d.Cost != null ? d.Cost : "") + "'/></div>" +
            "<div class='form-group'><label>Enrollment Start Date</label><input type='date' class='form-control' id='trEnrollStart' value='" + toDateInput(d.EnrollmentStartDate) + "'/></div>" +
            "<div class='form-group'><label>Enrollment Date</label><input type='date' class='form-control' id='trEnrollDate' value='" + toDateInput(d.EnrollmentDate) + "'/></div>" +
            "<div class='form-group'><label>Start Date</label><input type='date' class='form-control' id='trStart' value='" + toDateInput(d.StartDate) + "'/></div>" +
            "<div class='form-group'><label>End Date</label><input type='date' class='form-control' id='trEnd' value='" + toDateInput(d.EndDate || d.EnrollmentEndDate) + "'/></div>" +
            "<div class='form-group'><label>Status</label><select class='form-control' id='trStatus'>" + opt(lookups.statuses, "Id", "Name", d.StatusId) + "</select></div>" +
            "<div class='form-group'><label>Location</label><input class='form-control' id='trLoc' value='" + esc(d.Location || "") + "'/></div>" +
            "<div class='form-group'><label>Hours/Units</label><input type='number' step='0.01' class='form-control' id='trHours' value='" + (d.Hours != null ? d.Hours : "") + "'/></div>" +
            "</div><div class='col-md-6'>" +
            "<div class='form-group'><label>Assigned Track</label><select class='form-control' id='trTrack'>" + opt(lookups.tracks, "Id", "Name", d.TrainingTrackId) + "</select></div>" +
            "<div class='form-group'><label>Notes</label><textarea class='form-control' id='trNotes' rows='4'>" + esc(d.Notes || "") + "</textarea></div>" +
            "<div class='form-group'><label>Course Code</label><input class='form-control' id='trCourseCode' readonly value='" + esc(d.CourseCode || "") + "'/></div>" +
            "<div class='form-group'><label>Course Category</label><input class='form-control' id='trCourseCat' readonly value='" + esc(d.CourseCategory || "") + "'/></div>" +
            "<div class='form-group'><label>Completion Date</label><input type='date' class='form-control' id='trCompleteDate' value='" + toDateInput(d.CompletionDate) + "'/></div>" +
            "<div class='form-group'><label>Completion Status</label><select class='form-control' id='trCompleteStatus'>" + opt(lookups.statuses, "Id", "Name", d.CompletionStatusId) + "</select></div>" +
            "<div class='form-group'><label>Expiration Date</label><input type='date' class='form-control' id='trExp' value='" + toDateInput(d.ExpirationDate) + "'/></div>" +
            "<div class='form-group'><label>Instructor</label><input class='form-control' id='trInstructor' value='" + esc(d.Instructor || "") + "'/></div>" +
            "</div></div>";
    }

    function fillSchedules(classId, selectedId, thenApplyDefaults) {
        var $sched = $("#trSchedule").html("<option value=''>--Select--</option>");
        if (!classId) return;
        $.getJSON(url("schedules-url"), { classId: classId }, function (r) {
            ((r && r.data) || []).forEach(function (s) {
                $sched.append("<option value='" + s.Id + "' data-start='" + esc(s.StartDate) + "' data-end='" + esc(s.EndDate) + "' data-loc='" + esc(s.Location || "") + "'" +
                    (String(s.Id) === String(selectedId) ? " selected" : "") + ">" + esc(s.Name) + "</option>");
            });
            if (thenApplyDefaults) applyClassDefaults(classId);
        });
    }

    function applyClassDefaults(classId) {
        $.getJSON(url("defaults-url"), { classId: classId }, function (r) {
            if (!r || !r.success) return;
            var d = r.data;
            if ($("#trCost").val() === "") $("#trCost").val(d.Cost != null ? d.Cost : "");
            if ($("#trHours").val() === "") $("#trHours").val(d.Hours != null ? d.Hours : "");
            if (!$("#trLoc").val()) $("#trLoc").val(d.Location || "");
            if (!$("#trExp").val()) $("#trExp").val(toDateInput(d.ExpirationDate));
            if (!$("#trEnrollStart").val()) $("#trEnrollStart").val(toDateInput(d.EnrollmentStartDate));
            if (!$("#trEnd").val()) $("#trEnd").val(toDateInput(d.EnrollmentEndDate));
            if (!$("#trStart").val() && d.EnrollmentStartDate) $("#trStart").val(toDateInput(d.EnrollmentStartDate));
            if (d.TrainingTrackId) $("#trTrack").val(d.TrainingTrackId);
            var code = (lookups.courseCodes.filter(function (c) { return c.Id === d.CourseCodeId; })[0] || {}).Code || "";
            var cat = (lookups.courseCategories.filter(function (c) { return c.Id === d.CourseCategoryId; })[0] || {}).Name || "";
            $("#trCourseCode").val(code);
            $("#trCourseCat").val(cat);
        });
    }

    function openForm(data) {
        editId = data && data.Id ? data.Id : 0;
        $("#trEnrollModalBody").html(formHtml(data || {}));
        $("#trEnrollSave").text(editId ? "Save" : "Add");
        $("#trEnrollModal").modal("show");
        if (data && data.TrainingClassId) {
            fillSchedules(data.TrainingClassId, data.TrainingClassScheduleId, false);
        }
    }

    $(document).off(".trEnroll");
    $(document).on("click.trEnroll", "#btnTrAdd", function () {
        loadLookups(function () {
            var enrolledStatus = (lookups.statuses.filter(function (s) { return /enrolled/i.test(s.Name) && !/not/i.test(s.Name); })[0] || {}).Id;
            openForm({
                EmployeeId: loggedInEmployeeId(),
                EmployeeName: window._trCurrentEmployeeName,
                StatusId: enrolledStatus,
                EnrollmentDate: (function () {
                    var d = new Date();
                    return ("0" + (d.getMonth() + 1)).slice(-2) + "/" + ("0" + d.getDate()).slice(-2) + "/" + d.getFullYear();
                })()
            });
        });
    });
    $(document).on("click.trEnroll", ".tr-toggle-enroll", function () {
        var id = $(this).attr("data-id");
        $.post(url("toggle-url"), { id: id }, function (res) {
            if (!res || !res.success) { alert((res && res.message) || "Unable to update enrollment"); return; }
            loadGrid();
        }).fail(function () { alert("Unable to update enrollment"); });
    });
    $(document).on("click.trEnroll", ".tr-view-enroll", function () {
        var id = $(this).attr("data-id");
        loadLookups(function () {
            $.getJSON(url("get-url"), { id: id }, function (r) {
                if (!r || !r.success) { alert((r && r.message) || "Unable to load"); return; }
                openForm(r.data);
            });
        });
    });
    $(document).on("click.trEnroll", "#btnTrDel", function () {
        var ids = $(".tr-enroll-chk:checked").map(function () { return parseInt($(this).val(), 10); }).get();
        if (!ids.length || !confirm("Delete selected enrollments?")) return;
        $.post(url("del-url"), { idsJson: JSON.stringify(ids) }, function (res) {
            if (!res || !res.success) { alert((res && res.message) || "Failed"); return; }
            loadGrid();
        });
    });

    $(document).on("change.trEnroll", "#trClass", function () {
        var classId = $(this).val();
        fillSchedules(classId, null, true);
    });
    $(document).on("change.trEnroll", "#trSchedule", function () {
        var $opt = $(this).find("option:selected");
        if ($opt.attr("data-start")) $("#trStart").val($opt.attr("data-start"));
        if ($opt.attr("data-end")) $("#trEnd").val($opt.attr("data-end"));
        if ($opt.attr("data-loc")) $("#trLoc").val($opt.attr("data-loc"));
    });

    var searchTimer = null;
    $(document).on("keyup.trEnroll", "#trEmpSearch", function () {
        var q = $(this).val();
        clearTimeout(searchTimer);
        searchTimer = setTimeout(function () {
            $.getJSON(url("search-url"), { q: q }, function (r) {
                var html = "<option value=''>--Select--</option>";
                ((r && r.data) || []).forEach(function (e) {
                    html += "<option value='" + e.EmployeeId + "'>" + esc(e.Name) + "</option>";
                });
                $("#trEmpId").html(html);
            });
        }, 300);
    });

    $(document).on("click.trEnroll", "#trEnrollSave", function () {
        var payload = {
            Id: editId,
            EmployeeId: isSelf() ? loggedInEmployeeId() : $("#trEmpId").val(),
            self: isSelf() ? "1" : "0",
            TrainingTypeId: $("#trType").val() || null,
            TrainingClassId: $("#trClass").val() || null,
            TrainingClassScheduleId: $("#trSchedule").val() || null,
            TrainingTrackId: $("#trTrack").val() || null,
            EnrollmentDate: $("#trEnrollDate").val(),
            EnrollmentStartDate: $("#trEnrollStart").val(),
            StartDate: $("#trStart").val(),
            EndDate: $("#trEnd").val(),
            Location: $("#trLoc").val(),
            Hours: $("#trHours").val() || null,
            Cost: $("#trCost").val() || null,
            StatusId: $("#trStatus").val() || null,
            CompletionDate: $("#trCompleteDate").val(),
            CompletionStatusId: $("#trCompleteStatus").val() || null,
            ExpirationDate: $("#trExp").val(),
            Notes: $("#trNotes").val(),
            Instructor: $("#trInstructor").val()
        };
        $.ajax({
            url: url("save-url"),
            type: "POST",
            data: payload,
            success: function (res) {
                if (!res || !res.success) { alert((res && res.message) || "Save failed"); return; }
                $("#trEnrollModal").modal("hide");
                loadGrid();
            },
            error: function () { alert("Save failed"); }
        });
    });

    loadLookups(function () { loadGrid(); });
})();
