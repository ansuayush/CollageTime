(function () {
    var $root = $("#prSetupRoot");
    if (!$root.length) return;
    function url(n) { return $root.attr("data-" + n) || ""; }

    var scores = [], types = [], respTypes = [], sections = [], criteria = [], reviews = [];
    var modalMode = null, editId = 0;
    var selectedEmps = [];
    var employeeOptions = [];
    var searchTimer = null;

    function loadEmployeeOptions(done) {
        if (employeeOptions.length) {
            fillEmpSelect($("#mEmpFilter").val());
            if (done) done();
            return;
        }
        $.getJSON(url("emps-url"), function (r) {
            employeeOptions = (r && r.data) || [];
            fillEmpSelect($("#mEmpFilter").val());
            if (done) done();
        });
    }

    function fillEmpSelect(filter) {
        var $s = $("#mEmpPick");
        if (!$s.length) return;
        var q = $.trim(filter || "").toLowerCase();
        var prev = $s.val();
        $s.empty().append($("<option/>").val("").text("-- Select employee --"));
        var count = 0;
        employeeOptions.forEach(function (e) {
            var text = e.text || e.name || ("#" + e.id);
            var hay = (text + " " + (e.fileNumber || "")).toLowerCase();
            if (q && hay.indexOf(q) < 0) return;
            if (selectedEmps.some(function (x) { return String(x.id) === String(e.id); })) return;
            $s.append($("<option/>").val(e.id).text(text));
            count++;
        });
        if (!count) $s.append($("<option disabled/>").val("").text(q ? "No matches" : "No employees available"));
        if (prev && $s.find("option[value='" + prev + "']").length) $s.val(prev);
        else $s.val("");
    }

    function addSelectedFromDropdown() {
        var $opt = $("#mEmpPick option:selected");
        if (!$opt.length || $opt.is(":disabled") || !$opt.val()) {
            alert("Select an employee from the dropdown first, then click Add.");
            return;
        }
        var id = String($opt.val());
        var text = $opt.text();
        if (!selectedEmps.some(function (e) { return String(e.id) === id; }))
            selectedEmps.push({ id: id, text: text });
        renderSelectedEmps();
        fillEmpSelect($("#mEmpFilter").val());
        $("#mEmpPick").val("");
    }

    function loadAll() {
        $.getJSON(url("score-url"), function (r) { scores = (r && r.data) || []; renderScores(); });
        $.getJSON(url("type-url"), function (r) { types = (r && r.data) || []; renderTypes(); });
        $.getJSON(url("resp-url"), function (r) { respTypes = (r && r.data) || []; renderRespTypes(); });
        $.getJSON(url("section-url"), function (r) { sections = (r && r.data) || []; renderSections(); });
        $.getJSON(url("crit-url"), function (r) { criteria = (r && r.data) || []; renderCriteria(); });
        $.getJSON(url("review-url"), function (r) { reviews = (r && r.data) || []; renderReviews(); });
    }

    function renderScores() {
        var $tb = $("#prScoreTable tbody").empty();
        scores.forEach(function (x) {
            $tb.append("<tr><td>" + (x.ItemName || "") + "</td><td>" + x.ItemValue + "</td><td>" + x.SortOrder +
                "</td><td>" + (x.IsActive ? "Yes" : "No") + "</td><td><button type='button' class='btn btn-xs btn-info btn-edit-score' data-id='" + x.Id + "'>Edit</button></td></tr>");
        });
    }
    function renderTypes() {
        var $tb = $("#prTypeTable tbody").empty();
        types.forEach(function (x) {
            $tb.append("<tr><td>" + (x.Description || "") + "</td><td>" + (x.IsActive ? "Yes" : "No") +
                "</td><td><button type='button' class='btn btn-xs btn-info btn-edit-type' data-id='" + x.ReviewCriteriaTypeId + "'>Edit</button></td></tr>");
        });
    }
    function renderRespTypes() {
        var $tb = $("#prRespTable tbody").empty();
        respTypes.forEach(function (x) {
            $tb.append("<tr><td>" + (x.Code || "") + "</td><td>" + (x.Description || "") + "</td><td>" + x.SortOrder +
                "</td><td>" + (x.IsDefault ? "Yes" : "No") + "</td><td>" + (x.IsActive ? "Yes" : "No") +
                "</td><td><button type='button' class='btn btn-xs btn-info btn-edit-resp' data-id='" + x.ResponseTypeId + "'>Edit</button></td></tr>");
        });
    }
    function renderSections() {
        var $tb = $("#prSectionTable tbody").empty();
        sections.forEach(function (x) {
            $tb.append("<tr><td>" + (x.SectionName || "") + "</td><td>" + x.SortOrder + "</td><td>" + (x.IsActive ? "Yes" : "No") +
                "</td><td><button type='button' class='btn btn-xs btn-info btn-edit-section' data-id='" + x.SectionId + "'>Edit</button></td></tr>");
        });
    }
    function renderCriteria() {
        var $tb = $("#prCritTable tbody").empty();
        criteria.forEach(function (x) {
            $tb.append("<tr><td>" + (x.Description || "") + "</td><td>" + (x.CriteriaTypeName || "") + "</td><td>" + (x.SectionName || "") +
                "</td><td>" + (x.ResponseTypeName || x.ResponseTypeCode || "") + "</td><td>" + x.SequenceNumber + "</td><td>" + (x.IsActive ? "Yes" : "No") +
                "</td><td><button type='button' class='btn btn-xs btn-info btn-edit-crit' data-id='" + x.ReviewCriteriaId + "'>Edit</button></td></tr>");
        });
    }
    function renderReviews() {
        var $tb = $("#prReviewTable tbody").empty();
        reviews.forEach(function (x) {
            $tb.append("<tr><td>" + (x.ReviewName || "") + "</td><td>" + (x.RevieweeMode || "") + "</td><td>" + (x.Status || "") +
                "</td><td>" + ((x.EmployeeNames || []).join(", ")) +
                "</td><td><button type='button' class='btn btn-xs btn-info btn-edit-review' data-id='" + x.ReviewId + "'>Edit</button></td></tr>");
        });
    }

    function openModal(title, html, mode, id) {
        modalMode = mode; editId = id || 0;
        $("#prModalTitle").text(title);
        $("#prModalBody").html(html);
        $("#prModal").modal("show");
    }

    $("#btnNewScore").on("click", function () {
        openModal("Rating Scale Item", "<div class='form-group'><label>Name</label><input id='mName' class='form-control'/></div>" +
            "<div class='row'><div class='col-md-6'><div class='form-group'><label>Value</label><input id='mValue' type='number' step='0.01' class='form-control' value='1'/></div></div>" +
            "<div class='col-md-6'><div class='form-group'><label>Order</label><input id='mOrder' type='number' class='form-control' value='1'/></div></div></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' checked/> Active</label></div>", "score", 0);
    });
    $(document).on("click", ".btn-edit-score", function () {
        var id = $(this).attr("data-id");
        var x = scores.filter(function (s) { return String(s.Id) === String(id); })[0];
        if (!x) return;
        openModal("Rating Scale Item", "<div class='form-group'><label>Name</label><input id='mName' class='form-control' value='" + (x.ItemName || "") + "'/></div>" +
            "<div class='row'><div class='col-md-6'><div class='form-group'><label>Value</label><input id='mValue' type='number' step='0.01' class='form-control' value='" + x.ItemValue + "'/></div></div>" +
            "<div class='col-md-6'><div class='form-group'><label>Order</label><input id='mOrder' type='number' class='form-control' value='" + x.SortOrder + "'/></div></div></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' " + (x.IsActive ? "checked" : "") + "/> Active</label></div>", "score", x.Id);
    });

    $("#btnNewType").on("click", function () {
        openModal("Criteria Type", "<div class='form-group'><label>Description</label><input id='mName' class='form-control'/></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' checked/> Active</label></div>", "type", 0);
    });
    $(document).on("click", ".btn-edit-type", function () {
        var id = $(this).attr("data-id");
        var x = types.filter(function (s) { return String(s.ReviewCriteriaTypeId) === String(id); })[0];
        if (!x) return;
        openModal("Criteria Type", "<div class='form-group'><label>Description</label><input id='mName' class='form-control' value='" + (x.Description || "") + "'/></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' " + (x.IsActive ? "checked" : "") + "/> Active</label></div>", "type", x.ReviewCriteriaTypeId);
    });

    $("#btnNewResp").on("click", function () {
        openModal("Response Type", "<div class='form-group'><label>Code</label><input id='mCode' class='form-control' placeholder='Rating / Text'/></div>" +
            "<div class='form-group'><label>Description</label><input id='mName' class='form-control'/></div>" +
            "<div class='form-group'><label>Order</label><input id='mOrder' type='number' class='form-control' value='1'/></div>" +
            "<div class='checkbox'><label><input id='mDefault' type='checkbox'/> Default</label></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' checked/> Active</label></div>", "resp", 0);
    });
    $(document).on("click", ".btn-edit-resp", function () {
        var id = $(this).attr("data-id");
        var x = respTypes.filter(function (s) { return String(s.ResponseTypeId) === String(id); })[0];
        if (!x) return;
        openModal("Response Type", "<div class='form-group'><label>Code</label><input id='mCode' class='form-control' value='" + (x.Code || "") + "'/></div>" +
            "<div class='form-group'><label>Description</label><input id='mName' class='form-control' value='" + (x.Description || "") + "'/></div>" +
            "<div class='form-group'><label>Order</label><input id='mOrder' type='number' class='form-control' value='" + x.SortOrder + "'/></div>" +
            "<div class='checkbox'><label><input id='mDefault' type='checkbox' " + (x.IsDefault ? "checked" : "") + "/> Default</label></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' " + (x.IsActive ? "checked" : "") + "/> Active</label></div>", "resp", x.ResponseTypeId);
    });

    $("#btnNewSection").on("click", function () {
        openModal("Section", "<div class='form-group'><label>Name</label><input id='mName' class='form-control'/></div>" +
            "<div class='form-group'><label>Description</label><input id='mDesc' class='form-control'/></div>" +
            "<div class='form-group'><label>Order</label><input id='mOrder' type='number' class='form-control' value='1'/></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' checked/> Active</label></div>", "section", 0);
    });
    $(document).on("click", ".btn-edit-section", function () {
        var id = $(this).attr("data-id");
        var x = sections.filter(function (s) { return String(s.SectionId) === String(id); })[0];
        if (!x) return;
        openModal("Section", "<div class='form-group'><label>Name</label><input id='mName' class='form-control' value='" + (x.SectionName || "") + "'/></div>" +
            "<div class='form-group'><label>Description</label><input id='mDesc' class='form-control' value='" + (x.Description || "") + "'/></div>" +
            "<div class='form-group'><label>Order</label><input id='mOrder' type='number' class='form-control' value='" + x.SortOrder + "'/></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' " + (x.IsActive ? "checked" : "") + "/> Active</label></div>", "section", x.SectionId);
    });

    function critForm(c) {
        c = c || { SequenceNumber: 1, IsActive: true };
        var defaultResp = respTypes.filter(function (r) { return r.IsDefault && r.IsActive; })[0]
            || respTypes.filter(function (r) { return r.IsActive; })[0];
        var selectedResp = c.ResponseTypeId || (defaultResp ? defaultResp.ResponseTypeId : "");
        var tOpts = types.map(function (t) {
            return "<option value='" + t.ReviewCriteriaTypeId + "'" + (String(t.ReviewCriteriaTypeId) === String(c.CriteriaTypeId) ? " selected" : "") + ">" + t.Description + "</option>";
        }).join("");
        var sOpts = sections.map(function (s) {
            return "<option value='" + s.SectionId + "'" + (String(s.SectionId) === String(c.SectionId) ? " selected" : "") + ">" + s.SectionName + "</option>";
        }).join("");
        var rOpts = respTypes.filter(function (r) { return r.IsActive || String(r.ResponseTypeId) === String(selectedResp); }).map(function (r) {
            return "<option value='" + r.ResponseTypeId + "'" + (String(r.ResponseTypeId) === String(selectedResp) ? " selected" : "") + ">" + (r.Description || r.Code) + "</option>";
        }).join("");
        return "<div class='form-group'><label>Description</label><textarea id='mName' class='form-control' rows='2'>" + (c.Description || "") + "</textarea></div>" +
            "<div class='row'><div class='col-md-6'><div class='form-group'><label>Type</label><select id='mType' class='form-control'><option value=''>--None--</option>" + tOpts + "</select></div></div>" +
            "<div class='col-md-6'><div class='form-group'><label>Section</label><select id='mSection' class='form-control'><option value=''>--None--</option>" + sOpts + "</select></div></div></div>" +
            "<div class='row'><div class='col-md-6'><div class='form-group'><label>Response</label><select id='mResp' class='form-control'>" + rOpts + "</select></div></div>" +
            "<div class='col-md-6'><div class='form-group'><label>Sequence</label><input id='mOrder' type='number' class='form-control' value='" + (c.SequenceNumber || 1) + "'/></div></div></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' " + (c.IsActive !== false ? "checked" : "") + "/> Active</label></div>";
    }
    $("#btnNewCrit").on("click", function () { openModal("Criteria", critForm(), "crit", 0); });
    $(document).on("click", ".btn-edit-crit", function () {
        var id = $(this).attr("data-id");
        var x = criteria.filter(function (s) { return String(s.ReviewCriteriaId) === String(id); })[0];
        if (!x) return;
        openModal("Criteria", critForm(x), "crit", x.ReviewCriteriaId);
    });

    function renderSelectedEmps() {
        var $box = $("#mEmpSelected").empty();
        var ids = selectedEmps.map(function (e) { return e.id; }).join(",");
        $("#mEmpIdsCsv").val(ids);
        if (!selectedEmps.length) {
            $box.hide();
            $("#mEmpCount").text("No employees added yet — select from the dropdown and click Add.");
            return;
        }
        selectedEmps.forEach(function (e) {
            $box.append("<span class='label label-primary' style='display:inline-block;margin:2px;padding:5px 8px;' data-id='" + e.id + "'>" + e.text +
                " <a href='#' class='pr-remove-emp' style='color:#fff;'>&times;</a></span>");
        });
        $box.show();
        $("#mEmpCount").text(selectedEmps.length + " selected");
    }

    function isHrRole(role) { return String(role || "").toUpperCase() === "HR"; }

    function isSkippedRole(role) {
        var r = String(role || "").trim().toUpperCase();
        return !r || r === "NONE" || r === "(NONE)" || r === "SKIP" || r === "(SKIP)";
    }

    var STEP_ROLE_OPTIONS = ["(None)", "Approver1", "Approver2", "Approver3", "HR", "Other", "Employee"];

    function defaultReviewSteps() {
        return [
            { StepOrder: 1, ReviewerRole: "Approver1", IsViewPriorResponses: false, CriteriaIds: [] },
            { StepOrder: 2, ReviewerRole: "(None)", IsViewPriorResponses: true, CriteriaIds: [] },
            { StepOrder: 3, ReviewerRole: "(None)", IsViewPriorResponses: true, CriteriaIds: [] },
            { StepOrder: 4, ReviewerRole: "HR", IsViewPriorResponses: true, CriteriaIds: [], OtherPersonIds: [] }
        ];
    }

    function expandStepsForDisplay(steps) {
        var defaults = defaultReviewSteps();
        if (!steps || !steps.length) return defaults;
        return [1, 2, 3, 4].map(function (order) {
            var atOrder = steps.filter(function (s) { return (s.StepOrder || 0) === order; })[0];
            if (atOrder) return atOrder;
            if (order === 1) {
                var a1 = steps.filter(function (s) { return String(s.ReviewerRole || "").toUpperCase() === "APPROVER1"; })[0];
                if (a1) return $.extend({}, defaults[0], a1, { StepOrder: 1 });
            }
            if (order === 4) {
                var hr = steps.filter(function (s) { return isHrRole(s.ReviewerRole); })[0];
                if (hr) return $.extend({}, defaults[3], hr, { StepOrder: 4 });
            }
            return defaults[order - 1];
        });
    }

    function buildRoleOptions(selected) {
        return STEP_ROLE_OPTIONS.map(function (role) {
            var sel = String(selected || "") === role ? " selected" : "";
            return "<option" + sel + ">" + role + "</option>";
        }).join("");
    }

    /** First step (lowest order) has no prior answers — hide View prior and force off. */
    function syncFirstStepPrior() {
        var $rows = $("#mStepsBody .pr-step-row");
        if (!$rows.length) return;
        var minOrder = null;
        $rows.each(function () {
            var o = parseInt($(this).find(".m-step-order").val() || "999", 10);
            var role = $(this).find(".m-step-role").val();
            if (isSkippedRole(role)) return;
            if (minOrder === null || o < minOrder) minOrder = o;
        });
        $rows.each(function () {
            var o = parseInt($(this).find(".m-step-order").val() || "999", 10);
            var role = $(this).find(".m-step-role").val();
            var $wrap = $(this).find(".m-step-prior-wrap");
            var $cb = $(this).find(".m-step-prior");
            if (isSkippedRole(role) || o === minOrder) {
                $cb.prop("checked", false);
                $wrap.hide();
            } else {
                $wrap.show();
            }
        });
    }

    function buildStepAssignHtml(s) {
        if (isSkippedRole(s.ReviewerRole)) {
            return "<div class='pr-step-assign pr-step-skipped text-muted small' style='padding:8px 0;'><em>Step skipped — no approver for this order.</em></div>";
        }
        if (isHrRole(s.ReviewerRole)) {
            var people = [];
            (s.OtherPersonIds || []).forEach(function (id, i) {
                people.push({ id: String(id), text: (s.OtherPersonNames && s.OtherPersonNames[i]) || ("#" + id) });
            });
            if (!people.length && s.OtherPersonId) {
                people.push({ id: String(s.OtherPersonId), text: (s.OtherPersonNames && s.OtherPersonNames[0]) || ("#" + s.OtherPersonId) });
            }
            var chips = people.map(function (p) {
                return "<span class='label label-info' style='display:inline-block;margin:2px;padding:5px 8px;' data-person-id='" + p.id + "'>" + p.text +
                    " <a href='#' class='pr-remove-hr' style='color:#fff;'>&times;</a></span>";
            }).join("");
            return "<div class='pr-step-assign pr-step-hr'>" +
                "<p class='text-muted small' style='margin:0 0 6px;'>HR final: pick one or more HR people. They see prior approver answers and approve — no criteria checklist.</p>" +
                "<input type='text' class='form-control input-sm m-hr-search' placeholder='Search HR person...' autocomplete='off'/>" +
                "<ul class='list-group m-hr-results' style='display:none;max-height:140px;overflow:auto;position:absolute;z-index:110;width:88%;'></ul>" +
                "<div class='m-hr-selected' style='margin-top:6px;'>" + chips + "</div></div>";
        }
        var stepCritIds = (s.CriteriaIds && s.CriteriaIds.length) ? s.CriteriaIds : [];
        var activeCrit = criteria.filter(function (c) { return c.IsActive; });
        var critBlock = activeCrit.length
            ? activeCrit.map(function (c) {
                var checked = stepCritIds.indexOf(c.ReviewCriteriaId) >= 0
                    || stepCritIds.map(String).indexOf(String(c.ReviewCriteriaId)) >= 0;
                var label = (c.Description || "").length > 120 ? (c.Description.substring(0, 120) + "…") : (c.Description || "");
                return "<div class='checkbox' style='margin-top:2px;margin-bottom:2px;'><label style='font-weight:normal;'>" +
                    "<input type='checkbox' class='m-step-crit' value='" + c.ReviewCriteriaId + "' " + (checked ? "checked" : "") + "/> " + label +
                    "</label></div>";
            }).join("")
            : "<p class='text-muted small' style='margin:0;'>Add criteria first on the Criteria tab.</p>";
        return "<div class='pr-step-assign pr-step-crit-list' style='max-height:160px;overflow:auto;border:1px solid #eee;padding:6px 8px;background:#fafafa;'>" + critBlock + "</div>";
    }

    function reviewForm(r) {
        r = r || { RevieweeMode: "Employee", DaysToComplete: 14, IsAverageOfAllQuestions: true, Status: "Draft", Steps: [] };
        selectedEmps = [];
        var ids = r.EmployeeIds || r.employeeIds || [];
        var names = r.EmployeeNames || r.employeeNames || [];
        ids.forEach(function (id, i) {
            selectedEmps.push({ id: String(id), text: names[i] || ("#" + id) });
        });
        var steps = expandStepsForDisplay((r.Steps && r.Steps.length) ? r.Steps : null);
        var minOrder = null;
        steps.forEach(function (s) {
            if (isSkippedRole(s.ReviewerRole)) return;
            var o = s.StepOrder || 999;
            if (minOrder === null || o < minOrder) minOrder = o;
        });
        if (minOrder === null) minOrder = 1;
        var stepHtml = steps.map(function (s, idx) {
            var order = s.StepOrder || (idx + 1);
            var isFirst = !isSkippedRole(s.ReviewerRole) && order === minOrder;
            var priorChecked = !isFirst && s.IsViewPriorResponses !== false;
            return "<tr class='pr-step-row' data-idx='" + idx + "'>" +
                "<td style='width:70px;vertical-align:top;'><input type='number' class='form-control input-sm m-step-order' value='" + order + "'/></td>" +
                "<td style='width:140px;vertical-align:top;'><select class='form-control input-sm m-step-role'>" +
                buildRoleOptions(s.ReviewerRole) + "</select>" +
                "<div class='m-step-prior-wrap' style='margin-top:8px;" + (isFirst ? "display:none;" : "") + "'>" +
                "<label class='checkbox-inline'><input type='checkbox' class='m-step-prior' " + (priorChecked ? "checked" : "") + "/> View prior</label></div></td>" +
                "<td class='pr-step-assign-cell'>" + buildStepAssignHtml(s) + "</td></tr>";
        }).join("");

        setTimeout(function () {
            renderSelectedEmps();
            syncFirstStepPrior();
            loadEmployeeOptions();
        }, 0);

        return "<div class='form-group'><label>Review Name</label><input id='mName' class='form-control' value='" + (r.ReviewName || "") + "'/></div>" +
            "<div class='row'><div class='col-md-4'><div class='form-group'><label>Reviewee Mode</label><select id='mMode' class='form-control'><option>Employee</option><option>Position</option><option>Department</option><option>Supervisor</option></select></div></div>" +
            "<div class='col-md-4'><div class='form-group'><label>Status</label><select id='mStatus' class='form-control'><option>Draft</option><option>Ready</option><option>InProgress</option><option>Completed</option></select></div></div>" +
            "<div class='col-md-4'><div class='form-group'><label>Days to Complete</label><input id='mDays' type='number' class='form-control' value='" + (r.DaysToComplete || 14) + "'/></div></div></div>" +
            "<div class='row'><div class='col-md-4'><div class='form-group'><label>Interval</label><select id='mInterval' class='form-control'><option>Day</option><option>Week</option><option>Month</option></select></div></div>" +
            "<div class='col-md-4'><div class='form-group'><label>From</label><select id='mFromSched' class='form-control'><option>Hire Date</option><option>Custom Date</option></select></div></div>" +
            "<div class='col-md-4'><div class='form-group'><label>Custom Date</label><input id='mFromDate' type='date' class='form-control' value='" + (r.FromDate || "") + "'/></div></div></div>" +
            "<div class='form-group'><label>Scoring</label><div class='checkbox'><label><input type='radio' name='mScoreRule' id='mAvg' value='avg' " + (!r.IsSumOfAllQuestions ? "checked" : "") + "/> Average of questions</label></div>" +
            "<div class='checkbox'><label><input type='radio' name='mScoreRule' id='mSum' value='sum' " + (r.IsSumOfAllQuestions ? "checked" : "") + "/> Sum of questions</label></div></div>" +
            "<div class='form-group'><label>Employees</label>" +
            "<input type='hidden' id='mEmpIdsCsv' value='" + selectedEmps.map(function (e) { return e.id; }).join(",") + "'/>" +
            "<div class='form-inline'>" +
            "<input type='text' class='form-control' id='mEmpFilter' placeholder='Filter...' autocomplete='off' style='width:160px;margin-right:6px;vertical-align:middle;'/>" +
            "<select id='mEmpPick' class='form-control' style='width:280px;max-width:100%;margin-right:6px;vertical-align:middle;'><option value=''>-- Select employee --</option></select>" +
            "<button type='button' class='btn btn-default' id='mEmpAdd' style='vertical-align:middle;'>Add</button></div>" +
            "<div id='mEmpSelected' style='margin-top:8px;'></div><small id='mEmpCount' class='text-muted'></small></div>" +
            "<h5>Approval Steps</h5>" +
            "<p class='text-muted small'>Approver1=Report To, Approver2=Manager2, Approver3=Manager3 — Approver1 is required. Set Approver2/Approver3 to <strong>(None)</strong> if not needed. Other steps are optional. The first active step cannot view prior responses. HR step: assign one or more HR people (no criteria); they review prior answers and final-approve.</p>" +
            "<table class='table table-condensed table-bordered'><thead><tr><th style='width:70px;'>Order</th><th style='width:140px;'>Role</th><th>Assign (criteria or HR people)</th></tr></thead><tbody id='mStepsBody'>" + stepHtml + "</tbody></table>";
    }

    function collectSelectedEmployeeCsv() {
        // Prefer chips in the open modal (avoids stale duplicate #prModal from LoadMenu).
        var $modal = $("#prModal.in, #prModal.show").last();
        if (!$modal.length) $modal = $("#prModal").last();
        var ids = [];
        $modal.find("#mEmpSelected [data-id]").each(function () {
            var id = String($(this).attr("data-id") || "");
            if (id && ids.indexOf(id) < 0) ids.push(id);
        });
        if (!ids.length) {
            selectedEmps.forEach(function (e) {
                var id = String(e.id || "");
                if (id && ids.indexOf(id) < 0) ids.push(id);
            });
        }
        if (!ids.length) {
            var hidden = $.trim($modal.find("#mEmpIdsCsv").val() || "");
            if (hidden) {
                hidden.split(",").forEach(function (p) {
                    p = $.trim(p);
                    if (p && ids.indexOf(p) < 0) ids.push(p);
                });
            }
        }
        return ids.join(",");
    }

    // Prevent stacked handlers when SetupPartial is loaded more than once via LoadMenu.
    $(document).off(".prSetup");
    $(document).off("click", ".btn-edit-review");
    $(document).off("click", "#mEmpAdd");
    $(document).off("click", ".pr-remove-emp");
    $(document).off("click", ".pr-remove-hr");
    $(document).off("click", ".m-hr-results li");
    $(document).off("change", ".m-step-role");
    $(document).off("change input", ".m-step-order");
    $(document).off("keyup input", "#mEmpFilter");
    $(document).off("keyup input", ".m-hr-search");
    $("#btnNewReview").off("click");
    $("#prModalSave").off("click");

    $("#btnNewReview").on("click.prSetup", function () { openModal("Review Master", reviewForm(), "review", 0); });
    $(document).on("click.prSetup", ".btn-edit-review", function () {
        if (!$("#prSetupRoot").length) return;
        var id = $(this).attr("data-id");
        var x = reviews.filter(function (s) { return String(s.ReviewId) === String(id); })[0];
        if (!x) return;
        openModal("Review Master", reviewForm(x), "review", x.ReviewId);
        setTimeout(function () {
            $("#mMode").val(x.RevieweeMode || "Employee");
            $("#mStatus").val(x.Status || "Draft");
            $("#mInterval").val(x.IntervalType || "Day");
            $("#mFromSched").val(x.FromSchedule || "Hire Date");
            renderSelectedEmps();
            syncFirstStepPrior();
            loadEmployeeOptions();
        }, 50);
    });

    $(document).on("change.prSetup input.prSetup", ".m-step-order", function () { syncFirstStepPrior(); });
    $(document).on("keyup.prSetup input.prSetup", "#mEmpFilter", function () {
        fillEmpSelect($(this).val());
    });
    $(document).on("click.prSetup", "#mEmpAdd", function (e) {
        e.preventDefault();
        addSelectedFromDropdown();
    });
    $(document).on("click.prSetup", ".pr-remove-emp", function (e) {
        e.preventDefault();
        var id = String($(this).closest("span").attr("data-id"));
        selectedEmps = selectedEmps.filter(function (x) { return String(x.id) !== id; });
        renderSelectedEmps();
        fillEmpSelect($("#mEmpFilter").val());
    });
    $(document).on("change.prSetup", ".m-step-role", function () {
        var $row = $(this).closest(".pr-step-row");
        var role = $(this).val();
        $row.find(".pr-step-assign-cell").html(buildStepAssignHtml({ ReviewerRole: role, CriteriaIds: [], OtherPersonIds: [] }));
        syncFirstStepPrior();
    });

    var hrSearchTimer = null;
    $(document).on("keyup.prSetup input.prSetup", ".m-hr-search", function () {
        var $input = $(this);
        var $row = $input.closest(".pr-step-row");
        var $list = $row.find(".m-hr-results");
        clearTimeout(hrSearchTimer);
        var q = $.trim($input.val() || "");
        if (q.length < 2) { $list.hide().empty(); return; }
        hrSearchTimer = setTimeout(function () {
            $.getJSON(url("search-url"), { q: q }, function (r) {
                $list.empty();
                ((r && r.data) || []).forEach(function (e) {
                    var pid = e.personId || e.PersonId;
                    if (!pid) return;
                    $list.append($("<li class='list-group-item' style='cursor:pointer'/>")
                        .attr("data-person-id", pid).text(e.text || e.name || ("#" + pid)));
                });
                $list.toggle(!!$list.children().length);
            });
        }, 250);
    });
    $(document).on("click.prSetup", ".m-hr-results li", function () {
        var $li = $(this);
        var $row = $li.closest(".pr-step-row");
        var pid = String($li.attr("data-person-id"));
        var text = $li.text();
        var $box = $row.find(".m-hr-selected");
        if ($box.find("[data-person-id='" + pid + "']").length) {
            $row.find(".m-hr-search").val("");
            $row.find(".m-hr-results").hide().empty();
            return;
        }
        $box.append("<span class='label label-info' style='display:inline-block;margin:2px;padding:5px 8px;' data-person-id='" + pid + "'>" + text +
            " <a href='#' class='pr-remove-hr' style='color:#fff;'>&times;</a></span>");
        $row.find(".m-hr-search").val("");
        $row.find(".m-hr-results").hide().empty();
    });
    $(document).on("click.prSetup", ".pr-remove-hr", function (e) {
        e.preventDefault();
        $(this).closest("span").remove();
    });

    $("#prModalSave").on("click.prSetup", function () {
        if (!$("#prSetupRoot").length) return;
        var payload, postUrl;
        if (modalMode === "score") {
            postUrl = url("score-save");
            payload = { id: editId, itemName: $("#mName").val(), itemValue: $("#mValue").val() || 0, sortOrder: $("#mOrder").val() || 0, isActive: $("#mActive").is(":checked") };
        } else if (modalMode === "type") {
            postUrl = url("type-save");
            payload = { reviewCriteriaTypeId: editId, description: $("#mName").val(), isActive: $("#mActive").is(":checked") };
        } else if (modalMode === "resp") {
            postUrl = url("resp-save");
            payload = {
                responseTypeId: editId,
                code: $("#mCode").val(),
                description: $("#mName").val(),
                sortOrder: $("#mOrder").val() || 0,
                isActive: $("#mActive").is(":checked"),
                isDefault: $("#mDefault").is(":checked")
            };
        } else if (modalMode === "section") {
            postUrl = url("section-save");
            payload = { sectionId: editId, sectionName: $("#mName").val(), description: $("#mDesc").val(), sortOrder: $("#mOrder").val() || 0, isActive: $("#mActive").is(":checked") };
        } else if (modalMode === "crit") {
            postUrl = url("crit-save");
            payload = {
                ReviewCriteriaId: editId,
                Description: $("#mName").val(),
                CriteriaTypeId: $("#mType").val() || null,
                SectionId: $("#mSection").val() || null,
                ResponseTypeId: $("#mResp").val() || 1,
                SequenceNumber: $("#mOrder").val() || 1,
                IsActive: $("#mActive").is(":checked")
            };
        } else if (modalMode === "review") {
            postUrl = url("review-save");
            var $modal = $("#prModal.in, #prModal.show").last();
            if (!$modal.length) $modal = $("#prModal").last();
            var steps = [];
            var minOrder = null;
            $modal.find("#mStepsBody .pr-step-row").each(function () {
                var o = parseInt($(this).find(".m-step-order").val() || "1", 10);
                var role = $(this).find(".m-step-role").val();
                if (isSkippedRole(role)) return;
                if (minOrder === null || o < minOrder) minOrder = o;
            });
            $modal.find("#mStepsBody .pr-step-row").each(function () {
                var role = $(this).find(".m-step-role").val();
                if (isSkippedRole(role)) return;
                var order = parseInt($(this).find(".m-step-order").val() || "1", 10);
                var cids = [];
                var personIds = [];
                if (isHrRole(role)) {
                    $(this).find(".m-hr-selected [data-person-id]").each(function () {
                        var pid = parseInt($(this).attr("data-person-id"), 10);
                        if (pid > 0) personIds.push(pid);
                    });
                } else {
                    $(this).find(".pr-step-assign-cell .m-step-crit:checked").each(function () { cids.push(parseInt($(this).val(), 10)); });
                }
                steps.push({
                    StepOrder: order,
                    ReviewerRole: role,
                    IsViewPriorResponses: order !== minOrder && $(this).find(".m-step-prior").is(":checked"),
                    CriteriaIds: cids,
                    OtherPersonIds: personIds,
                    OtherPersonId: personIds.length ? personIds[0] : null
                });
            });
            var a1Steps = steps.filter(function (s) { return String(s.ReviewerRole || "").toUpperCase() === "APPROVER1"; });
            if (!a1Steps.length) {
                alert("Approver1 step is required.");
                return;
            }
            if (a1Steps.some(function (s) { return !s.CriteriaIds || !s.CriteriaIds.length; })) {
                alert("Approver1 must have at least one criteria assigned. Other steps are optional.");
                return;
            }
            var missingHr = steps.filter(function (s) { return !isSkippedRole(s.ReviewerRole) && isHrRole(s.ReviewerRole) && !s.OtherPersonIds.length; });
            if (missingHr.length) {
                if (!confirm("HR step has no people assigned. Any user with HR Approvals access can complete it. Continue?")) return;
            }
            var empCsv = collectSelectedEmployeeCsv();
            if (!$.trim(empCsv)) {
                alert("Select at least one employee from the dropdown and click Add before saving.");
                return;
            }
            payload = {
                ReviewId: editId,
                ReviewName: $modal.find("#mName").val(),
                RevieweeMode: $modal.find("#mMode").val(),
                Status: $modal.find("#mStatus").val(),
                IntervalType: $modal.find("#mInterval").val(),
                FromSchedule: $modal.find("#mFromSched").val(),
                FromDate: $modal.find("#mFromDate").val() || "",
                DaysToComplete: $modal.find("#mDays").val() || 14,
                IsAverageOfAllQuestions: $modal.find("input[name='mScoreRule']:checked").val() === "avg",
                IsSumOfAllQuestions: $modal.find("input[name='mScoreRule']:checked").val() === "sum",
                employeeIdsCsv: empCsv,
                employeeIds: empCsv,
                stepsJson: JSON.stringify(steps),
                criteriaIds: ""
            };
        } else return;

        $.post(postUrl, payload, function (res) {
            if (!res || !res.success) { alert((res && res.message) || "Save failed"); return; }
            $("#prModal").modal("hide");
            loadAll();
        }).fail(function (xhr) {
            alert("Save failed" + (xhr && xhr.status ? " (HTTP " + xhr.status + ")" : ""));
        });
    });

    loadAll();
})();
