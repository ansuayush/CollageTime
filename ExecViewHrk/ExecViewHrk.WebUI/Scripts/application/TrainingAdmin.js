(function () {
    var $root = $("#trAdminRoot");
    if (!$root.length) return;

    function url(n) { return $root.attr("data-" + n) || ""; }
    function esc(s) {
        return String(s == null ? "" : s).replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;");
    }
    function money(n) {
        if (n == null || n === "") return "$0.00";
        return "$" + (parseFloat(n) || 0).toFixed(2);
    }
    function toDateInput(val) {
        if (!val) return "";
        var s = String(val).trim();
        var mdy = s.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})$/);
        if (mdy) return mdy[3] + "-" + ("0" + mdy[1]).slice(-2) + "-" + ("0" + mdy[2]).slice(-2);
        var iso = s.match(/^(\d{4})-(\d{2})-(\d{2})/);
        if (iso) return iso[1] + "-" + iso[2] + "-" + iso[3];
        var d = new Date(s);
        if (isNaN(d.getTime())) return "";
        return d.getFullYear() + "-" + ("0" + (d.getMonth() + 1)).slice(-2) + "-" + ("0" + d.getDate()).slice(-2);
    }

    var modalMode = null;
    var editId = 0;
    var codes = [], cats = [], classesCache = [];

    function openModal(title, bodyHtml, mode, id) {
        modalMode = mode;
        editId = id || 0;
        $("#trModalTitle").text(title);
        $("#trModalBody").html(bodyHtml);
        $("#trModal").modal("show");
    }

    function loadCodes(cb) {
        $.getJSON(url("codes-url"), function (r) {
            codes = (r && r.data) || [];
            var $tb = $("#trCodeTable tbody").empty();
            codes.forEach(function (c) {
                $tb.append("<tr><td>" + esc(c.Code) + "</td><td>" + esc(c.Description || "") + "</td><td>" + (c.IsActive ? "Yes" : "No") +
                    "</td><td><a href='javascript:void(0)' class='tr-edit-code' data-id='" + c.Id + "'>Edit</a></td></tr>");
            });
            if (cb) cb();
        });
    }
    function loadCats(cb) {
        $.getJSON(url("cats-url"), function (r) {
            cats = (r && r.data) || [];
            var $tb = $("#trCatTable tbody").empty();
            cats.forEach(function (c) {
                $tb.append("<tr><td>" + esc(c.Name) + "</td><td>" + (c.IsActive ? "Yes" : "No") +
                    "</td><td><a href='javascript:void(0)' class='tr-edit-cat' data-id='" + c.Id + "'>Edit</a></td></tr>");
            });
            if (cb) cb();
        });
    }
    function loadTypes() {
        $.getJSON(url("types-url"), function (r) {
            var $tb = $("#trTypeTable tbody").empty();
            ((r && r.data) || []).forEach(function (c) {
                $tb.append("<tr><td>" + esc(c.Name) + "</td><td>" + (c.IsActive ? "Yes" : "No") +
                    "</td><td><a href='javascript:void(0)' class='tr-edit-type' data-id='" + c.Id + "' data-name='" + esc(c.Name) + "' data-active='" + c.IsActive + "'>Edit</a></td></tr>");
            });
        });
    }
    function loadStatuses() {
        $.getJSON(url("status-url"), function (r) {
            var $tb = $("#trStatusTable tbody").empty();
            ((r && r.data) || []).forEach(function (c) {
                $tb.append("<tr><td>" + esc(c.Name) + "</td><td>" + c.SortOrder + "</td><td>" + (c.IsActive ? "Yes" : "No") +
                    "</td><td><a href='javascript:void(0)' class='tr-edit-status' data-id='" + c.Id + "' data-name='" + esc(c.Name) + "' data-sort='" + c.SortOrder + "' data-active='" + c.IsActive + "'>Edit</a></td></tr>");
            });
        });
    }

    function optionsHtml(list, valueKey, textKey, selected) {
        var html = "<option value=''>--Select--</option>";
        (list || []).forEach(function (x) {
            html += "<option value='" + x[valueKey] + "'" + (String(x[valueKey]) === String(selected) ? " selected" : "") + ">" + esc(x[textKey]) + "</option>";
        });
        return html;
    }

    function loadClasses() {
        $.getJSON(url("classes-url"), function (r) {
            if (!r || !r.success) {
                $("#trClassTable tbody").html("<tr><td colspan='8' class='text-danger'>" + esc((r && r.message) || "Unable to load classes") + "</td></tr>");
                return;
            }
            classesCache = (r && r.data) || [];
            var $tb = $("#trClassTable tbody").empty();
            if (!classesCache.length) {
                $tb.append("<tr><td colspan='8' class='text-muted'>No classes yet.</td></tr>");
            } else {
                classesCache.forEach(function (c) {
                    $tb.append("<tr data-id='" + c.Id + "'>" +
                        "<td><input type='checkbox' class='tr-class-chk' value='" + c.Id + "'/></td>" +
                        "<td><a href='javascript:void(0)' class='tr-edit-class' data-id='" + c.Id + "'>" + esc(c.Name) + "</a></td>" +
                        "<td>" + esc(c.Description || "") + "</td>" +
                        "<td class='text-right'>" + money(c.Cost) + "</td>" +
                        "<td>" + (c.Hours != null ? c.Hours : "") + "</td>" +
                        "<td>" + esc(c.CourseCode || "") + "</td>" +
                        "<td>" + esc(c.CourseCategory || "") + "</td>" +
                        "<td>" + (c.IsActive ? "Yes" : "No") + "</td></tr>");
                });
            }
            var filterHtml = "<option value=''>All classes</option>" + classesCache.map(function (c) {
                return "<option value='" + c.Id + "'>" + esc(c.Name) + "</option>";
            }).join("");
            $("#trSchedClassFilter").html(filterHtml);
        });
    }

    function classForm(d) {
        d = d || {};
        return "<div class='row'>" +
            "<div class='col-md-6'>" +
            "<div class='form-group'><label>Class Name <span class='text-danger'>*</span></label><input class='form-control' id='trClassName' value='" + esc(d.Name || "") + "'/></div>" +
            "<div class='form-group'><label>Display description</label><textarea class='form-control' id='trClassDesc' rows='4'>" + esc(d.Description || "") + "</textarea></div>" +
            "<div class='form-group'><label>Location/URL</label><input class='form-control' id='trClassLoc' value='" + esc(d.Location || "") + "'/></div>" +
            "<div class='form-group'><label>Hours/Credits</label><input type='number' step='0.01' class='form-control' id='trClassHours' value='" + (d.Hours != null ? d.Hours : "") + "'/></div>" +
            "<div class='checkbox'><label><input type='checkbox' id='trClassActive'" + (d.IsActive !== false ? " checked" : "") + "/> Active</label></div>" +
            "</div><div class='col-md-6'>" +
            "<div class='form-group'><label>Course Code</label><select class='form-control' id='trClassCode'>" + optionsHtml(codes, "Id", "Code", d.CourseCodeId) + "</select></div>" +
            "<div class='form-group'><label>Course Category</label><select class='form-control' id='trClassCat'>" + optionsHtml(cats, "Id", "Name", d.CourseCategoryId) + "</select></div>" +
            "<div class='form-group'><label>Cost <span class='text-danger'>*</span></label><div class='input-group'><span class='input-group-addon'>$</span><input type='number' step='0.01' class='form-control' id='trClassCost' value='" + (d.Cost != null ? d.Cost : "0.00") + "'/></div></div>" +
            "<div class='form-group'><label>Enrollment Start Date</label><input type='date' class='form-control' id='trClassEnrollStart' value='" + toDateInput(d.EnrollmentStartDate) + "'/></div>" +
            "<div class='form-group'><label>Enrollment End Date</label><input type='date' class='form-control' id='trClassEnrollEnd' value='" + toDateInput(d.EnrollmentEndDate) + "'/></div>" +
            "<div class='form-group'><label>Expiration Date</label><input type='date' class='form-control' id='trClassExp' value='" + toDateInput(d.ExpirationDate) + "'/></div>" +
            "</div></div>";
    }

    function loadTracks() {
        $.getJSON(url("tracks-url"), function (r) {
            var $tb = $("#trTrackTable tbody").empty();
            var rows = (r && r.data) || [];
            if (!rows.length) {
                $tb.append("<tr><td colspan='5' class='text-muted'>No tracks yet.</td></tr>");
                return;
            }
            rows.forEach(function (t) {
                var classesHtml = (t.ClassNames || []).map(function (n) { return "<div>" + esc(n) + "</div>"; }).join("") || "<span class='text-muted'>No classes</span>";
                $tb.append("<tr data-id='" + t.Id + "'>" +
                    "<td><input type='checkbox' class='tr-track-chk' value='" + t.Id + "'/></td>" +
                    "<td><a href='javascript:void(0)' class='tr-track-toggle' data-id='" + t.Id + "'><i class='fa fa-chevron-right'></i></a></td>" +
                    "<td><a href='javascript:void(0)' class='tr-edit-track' data-id='" + t.Id + "'>" + esc(t.Name) + "</a>" +
                    "<div class='tr-track-classes' id='trTrackClasses_" + t.Id + "' style='display:none;margin-top:8px;'><div class='tr-track-classes-head'>Class Name</div>" + classesHtml + "</div></td>" +
                    "<td>" + t.TotalHours + "</td>" +
                    "<td><a href='javascript:void(0)' class='tr-copy-track' data-id='" + t.Id + "'>Copy</a></td></tr>");
            });
            window._trTracks = rows;
        });
    }

    function trackForm(d) {
        d = d || { ClassIds: [] };
        var checks = classesCache.map(function (c) {
            var on = (d.ClassIds || []).indexOf(c.Id) >= 0 || (d.ClassIds || []).indexOf(String(c.Id)) >= 0;
            return "<div class='checkbox'><label><input type='checkbox' class='tr-track-class' value='" + c.Id + "'" + (on ? " checked" : "") + "/> " + esc(c.Name) + "</label></div>";
        }).join("");
        return "<div class='form-group'><label>Track Name <span class='text-danger'>*</span></label><input class='form-control' id='trTrackName' value='" + esc(d.Name || "") + "'/></div>" +
            "<div class='form-group'><label>Description</label><textarea class='form-control' id='trTrackDesc' rows='3'>" + esc(d.Description || "") + "</textarea></div>" +
            "<div class='form-group'><label>Total Hours</label><input type='number' step='0.01' class='form-control' id='trTrackHours' value='" + (d.TotalHours != null ? d.TotalHours : "0") + "'/></div>" +
            "<div class='form-group'><label>Expiration Date</label><input type='date' class='form-control' id='trTrackExp' value='" + toDateInput(d.ExpirationDate) + "'/></div>" +
            "<div class='form-group'><label>Classes in Track</label><div class='tr-track-class-list'>" + (checks || "<span class='text-muted'>Create classes first.</span>") + "</div></div>";
    }

    function loadSchedules() {
        var classId = $("#trSchedClassFilter").val() || null;
        $.getJSON(url("sched-url"), { classId: classId }, function (r) {
            var $tb = $("#trSchedTable tbody").empty();
            var rows = (r && r.data) || [];
            if (!rows.length) {
                $tb.append("<tr><td colspan='7' class='text-muted'>No schedules.</td></tr>");
                return;
            }
            rows.forEach(function (s) {
                $tb.append("<tr data-id='" + s.Id + "'>" +
                    "<td><input type='checkbox' class='tr-sched-chk' value='" + s.Id + "'/></td>" +
                    "<td>" + esc(s.ClassName) + "</td>" +
                    "<td><a href='javascript:void(0)' class='tr-edit-sched' data-id='" + s.Id + "'>" + esc(s.ScheduleName) + "</a></td>" +
                    "<td>" + esc(s.StartDate) + "</td><td>" + esc(s.EndDate) + "</td>" +
                    "<td>" + esc(s.Location || "") + "</td><td>" + (s.IsActive ? "Yes" : "No") + "</td></tr>");
            });
            window._trSchedules = rows;
        });
    }

    function schedForm(d) {
        d = d || {};
        return "<div class='form-group'><label>Class <span class='text-danger'>*</span></label><select class='form-control' id='trSchedClass'>" + optionsHtml(classesCache, "Id", "Name", d.TrainingClassId) + "</select></div>" +
            "<div class='form-group'><label>Schedule Name</label><input class='form-control' id='trSchedName' value='" + esc(d.ScheduleName || "") + "'/></div>" +
            "<div class='row'><div class='col-md-6'><div class='form-group'><label>Start Date <span class='text-danger'>*</span></label><input type='date' class='form-control' id='trSchedStart' value='" + toDateInput(d.StartDate) + "'/></div></div>" +
            "<div class='col-md-6'><div class='form-group'><label>End Date <span class='text-danger'>*</span></label><input type='date' class='form-control' id='trSchedEnd' value='" + toDateInput(d.EndDate) + "'/></div></div></div>" +
            "<div class='form-group'><label>Location</label><input class='form-control' id='trSchedLoc' value='" + esc(d.Location || "") + "'/></div>" +
            "<div class='checkbox'><label><input type='checkbox' id='trSchedActive'" + (d.IsActive !== false ? " checked" : "") + "/> Active</label></div>";
    }

    function selectedIds(sel) {
        return $(sel).map(function () { return parseInt($(this).val(), 10); }).get();
    }

    $(document).off(".trAdmin");
    $(document).on("click.trAdmin", "#btnNewClass", function () {
        loadCodes(function () { loadCats(function () { openModal("Add New Class", classForm({ IsActive: true, Cost: 0 }), "class", 0); }); });
    });
    $(document).on("click.trAdmin", ".tr-edit-class", function () {
        var id = $(this).attr("data-id");
        $.getJSON(url("class-url"), { id: id }, function (r) {
            if (!r || !r.success) { alert((r && r.message) || "Failed"); return; }
            loadCodes(function () { loadCats(function () { openModal("Edit Class", classForm(r.data), "class", id); }); });
        });
    });
    $(document).on("click.trAdmin", "#btnDelClass", function () {
        var ids = selectedIds(".tr-class-chk:checked");
        if (!ids.length || !confirm("Delete selected classes?")) return;
        $.post(url("class-del"), { idsJson: JSON.stringify(ids) }, function () { loadClasses(); loadTracks(); });
    });

    $(document).on("click.trAdmin", "#btnNewTrack", function () {
        openModal("Add Training Track", trackForm({}), "track", 0);
    });
    $(document).on("click.trAdmin", ".tr-edit-track", function () {
        var id = parseInt($(this).attr("data-id"), 10);
        var t = (window._trTracks || []).filter(function (x) { return x.Id === id; })[0] || { Id: id };
        openModal("Edit Training Track", trackForm(t), "track", id);
    });
    $(document).on("click.trAdmin", ".tr-track-toggle", function () {
        var id = $(this).attr("data-id");
        $("#trTrackClasses_" + id).toggle();
        $(this).find("i").toggleClass("fa-chevron-right fa-chevron-down");
    });
    $(document).on("click.trAdmin", ".tr-copy-track", function () {
        $.post(url("track-copy"), { id: $(this).attr("data-id") }, function () { loadTracks(); });
    });
    $(document).on("click.trAdmin", "#btnDelTrack", function () {
        var ids = selectedIds(".tr-track-chk:checked");
        if (!ids.length || !confirm("Delete selected tracks?")) return;
        $.post(url("track-del"), { idsJson: JSON.stringify(ids) }, function () { loadTracks(); });
    });

    $(document).on("change.trAdmin", "#trSchedClassFilter", loadSchedules);
    $(document).on("click.trAdmin", "#btnNewSched", function () {
        openModal("Add Class Schedule", schedForm({ TrainingClassId: $("#trSchedClassFilter").val(), IsActive: true }), "sched", 0);
    });
    $(document).on("click.trAdmin", ".tr-edit-sched", function () {
        var id = parseInt($(this).attr("data-id"), 10);
        var s = (window._trSchedules || []).filter(function (x) { return x.Id === id; })[0];
        openModal("Edit Class Schedule", schedForm(s), "sched", id);
    });
    $(document).on("click.trAdmin", "#btnDelSched", function () {
        var ids = selectedIds(".tr-sched-chk:checked");
        if (!ids.length || !confirm("Delete selected schedules?")) return;
        $.post(url("sched-del"), { idsJson: JSON.stringify(ids) }, function () { loadSchedules(); });
    });

    $(document).on("click.trAdmin", "#btnNewCode", function () {
        openModal("Course Code", "<div class='form-group'><label>Code</label><input class='form-control' id='trLkCode'/></div><div class='form-group'><label>Description</label><input class='form-control' id='trLkDesc'/></div><div class='checkbox'><label><input type='checkbox' id='trLkActive' checked/> Active</label></div>", "code", 0);
    });
    $(document).on("click.trAdmin", "#btnNewCat", function () {
        openModal("Course Category", "<div class='form-group'><label>Name</label><input class='form-control' id='trLkName'/></div><div class='form-group'><label>Description</label><input class='form-control' id='trLkDesc'/></div><div class='checkbox'><label><input type='checkbox' id='trLkActive' checked/> Active</label></div>", "cat", 0);
    });
    $(document).on("click.trAdmin", "#btnNewType", function () {
        openModal("Training Type", "<div class='form-group'><label>Name</label><input class='form-control' id='trLkName'/></div><div class='checkbox'><label><input type='checkbox' id='trLkActive' checked/> Active</label></div>", "type", 0);
    });
    $(document).on("click.trAdmin", "#btnNewStatus", function () {
        openModal("Status", "<div class='form-group'><label>Name</label><input class='form-control' id='trLkName'/></div><div class='form-group'><label>Sort Order</label><input type='number' class='form-control' id='trLkSort' value='0'/></div><div class='checkbox'><label><input type='checkbox' id='trLkActive' checked/> Active</label></div>", "status", 0);
    });
    $(document).on("click.trAdmin", ".tr-edit-type", function () {
        openModal("Training Type", "<div class='form-group'><label>Name</label><input class='form-control' id='trLkName' value='" + $(this).attr("data-name") + "'/></div><div class='checkbox'><label><input type='checkbox' id='trLkActive'" + ($(this).attr("data-active") === "true" ? " checked" : "") + "/> Active</label></div>", "type", $(this).attr("data-id"));
    });
    $(document).on("click.trAdmin", ".tr-edit-status", function () {
        openModal("Status", "<div class='form-group'><label>Name</label><input class='form-control' id='trLkName' value='" + $(this).attr("data-name") + "'/></div><div class='form-group'><label>Sort Order</label><input type='number' class='form-control' id='trLkSort' value='" + $(this).attr("data-sort") + "'/></div><div class='checkbox'><label><input type='checkbox' id='trLkActive'" + ($(this).attr("data-active") === "true" ? " checked" : "") + "/> Active</label></div>", "status", $(this).attr("data-id"));
    });

    $(document).on("click.trAdmin", "#trModalSave", function () {
        if (modalMode === "class") {
            $.post(url("class-save"), {
                id: editId, name: $("#trClassName").val(), description: $("#trClassDesc").val(),
                cost: $("#trClassCost").val(), hours: $("#trClassHours").val(), location: $("#trClassLoc").val(),
                courseCodeId: $("#trClassCode").val() || null, courseCategoryId: $("#trClassCat").val() || null,
                isActive: $("#trClassActive").is(":checked"),
                expirationDate: $("#trClassExp").val(),
                enrollmentStartDate: $("#trClassEnrollStart").val(),
                enrollmentEndDate: $("#trClassEnrollEnd").val()
            }, function (res) {
                if (!res || !res.success) { alert((res && res.message) || "Failed"); return; }
                $("#trModal").modal("hide"); loadClasses();
            });
        } else if (modalMode === "track") {
            var classIds = $(".tr-track-class:checked").map(function () { return parseInt($(this).val(), 10); }).get();
            $.post(url("track-save"), {
                id: editId, name: $("#trTrackName").val(), description: $("#trTrackDesc").val(),
                totalHours: $("#trTrackHours").val() || 0, expirationDate: $("#trTrackExp").val(),
                classIdsJson: JSON.stringify(classIds)
            }, function (res) {
                if (!res || !res.success) { alert((res && res.message) || "Failed"); return; }
                $("#trModal").modal("hide"); loadTracks();
            });
        } else if (modalMode === "sched") {
            $.post(url("sched-save"), {
                id: editId, trainingClassId: $("#trSchedClass").val(), scheduleName: $("#trSchedName").val(),
                startDate: $("#trSchedStart").val(), endDate: $("#trSchedEnd").val(),
                location: $("#trSchedLoc").val(), isActive: $("#trSchedActive").is(":checked")
            }, function (res) {
                if (!res || !res.success) { alert((res && res.message) || "Failed"); return; }
                $("#trModal").modal("hide"); loadSchedules();
            });
        } else if (modalMode === "code") {
            $.post(url("codes-save"), { id: editId, code: $("#trLkCode").val(), description: $("#trLkDesc").val(), isActive: $("#trLkActive").is(":checked") }, function () { $("#trModal").modal("hide"); loadCodes(); });
        } else if (modalMode === "cat") {
            $.post(url("cats-save"), { id: editId, name: $("#trLkName").val(), description: $("#trLkDesc").val(), isActive: $("#trLkActive").is(":checked") }, function () { $("#trModal").modal("hide"); loadCats(); });
        } else if (modalMode === "type") {
            $.post(url("types-save"), { id: editId, name: $("#trLkName").val(), isActive: $("#trLkActive").is(":checked") }, function () { $("#trModal").modal("hide"); loadTypes(); });
        } else if (modalMode === "status") {
            $.post(url("status-save"), { id: editId, name: $("#trLkName").val(), isActive: $("#trLkActive").is(":checked"), sortOrder: $("#trLkSort").val() || 0 }, function () { $("#trModal").modal("hide"); loadStatuses(); });
        }
    });

    loadCodes(); loadCats(); loadTypes(); loadStatuses(); loadClasses(); loadTracks(); loadSchedules();
})();
