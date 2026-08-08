(function () {
    var $root = $("#benSetupRoot");
    if (!$root.length) return;
    function url(n) { return $root.attr("data-" + n) || ""; }
    var cats = [], plans = [], waits = [], eligs = [], classes = [], oes = [];
    var modalMode = null;
    var editId = 0;

    function fmtDate(d) {
        if (d == null || d === "") return "";
        if (typeof d === "string") {
            var ms = /\/Date\((-?\d+)(?:[+-]\d+)?\)\//.exec(d);
            if (ms) d = parseInt(ms[1], 10);
            else if (/^\d{4}-\d{2}-\d{2}/.test(d)) return d.substring(0, 10);
        }
        var dt = (d instanceof Date) ? d : new Date(d);
        if (isNaN(dt.getTime())) return "";
        var m = ("0" + (dt.getMonth() + 1)).slice(-2);
        var day = ("0" + (dt.getDate())).slice(-2);
        return dt.getFullYear() + "-" + m + "-" + day;
    }

    function loadAll() {
        $.getJSON(url("cat-url"), function (r) { cats = (r && r.data) || []; renderCats(); });
        $.getJSON(url("wait-url"), function (r) { waits = (r && r.data) || []; renderWaits(); });
        $.getJSON(url("elig-url"), function (r) { eligs = (r && r.data) || []; renderEligs(); });
        $.getJSON(url("plan-url"), function (r) { plans = (r && r.data) || []; renderPlans(); });
        $.getJSON(url("class-url"), function (r) { classes = (r && r.data) || []; renderClasses(); });
        $.getJSON(url("oe-url"), function (r) { oes = (r && r.data) || []; renderOe(); });
    }

    function renderCats() {
        var $tb = $("#benCatTable tbody").empty();
        cats.forEach(function (c) {
            $tb.append("<tr><td>" + c.CategoryName + "</td><td>" + c.DisplayOrder + "</td><td>" + (c.IsActive ? "Yes" : "No") +
                "</td><td><button type='button' class='btn btn-xs btn-info btn-edit-cat' data-id='" + c.CategoryId + "'>Edit</button></td></tr>");
        });
    }
    function renderWaits() {
        var $tb = $("#benWaitTable tbody").empty();
        waits.forEach(function (w) {
            $tb.append("<tr><td>" + w.Name + "</td><td>" + w.Days + "</td><td>" + (w.CalculationType || "") + "</td><td>" + (w.IsActive ? "Yes" : "No") +
                "</td><td><button type='button' class='btn btn-xs btn-info btn-edit-wait' data-id='" + w.WaitingPeriodId + "'>Edit</button></td></tr>");
        });
    }
    function renderEligs() {
        var $tb = $("#benEligTable tbody").empty();
        eligs.forEach(function (e) {
            $tb.append("<tr><td>" + e.RuleName + "</td><td>" + (e.MinServiceDays != null ? e.MinServiceDays : "") + "</td><td>" + (e.MinHours != null ? e.MinHours : "") +
                "</td><td>" + (e.IsActive ? "Yes" : "No") + "</td><td><button type='button' class='btn btn-xs btn-info btn-edit-elig' data-id='" + e.EligibilityRuleId + "'>Edit</button></td></tr>");
        });
    }
    function renderPlans() {
        var $tb = $("#benPlanTable tbody").empty();
        plans.forEach(function (p) {
            $tb.append("<tr><td>" + p.PlanName + "</td><td>" + (p.CategoryName || "") + "</td><td>" + (p.Carrier || "") + "</td><td>" + p.EmployeeCost +
                "</td><td>" + (p.IsActive ? "Yes" : "No") + "</td><td><button type='button' class='btn btn-xs btn-info btn-edit-plan' data-id='" + p.PlanId + "'>Edit</button></td></tr>");
        });
    }
    function renderClasses() {
        var $tb = $("#benClassTable tbody").empty();
        classes.forEach(function (c) {
            $tb.append("<tr><td>" + c.ClassName + "</td><td>" + (c.WaitingPeriodName || "") + "</td><td>" + (c.EligibilityRuleName || "") +
                "</td><td>" + ((c.PlanNames || []).join(", ")) + "</td><td>" + (c.IsActive ? "Yes" : "No") +
                "</td><td><button type='button' class='btn btn-xs btn-info btn-edit-class' data-id='" + c.BenefitClassId + "'>Edit</button></td></tr>");
        });
    }
    function renderOe() {
        var $tb = $("#benOeTable tbody").empty();
        oes.forEach(function (p) {
            $tb.append("<tr><td>" + p.EnrollmentName + "</td><td>" + fmtDate(p.StartDate) + "</td><td>" + fmtDate(p.EndDate) +
                "</td><td>" + p.Status + "</td><td><button type='button' class='btn btn-xs btn-info btn-edit-oe' data-id='" + p.EnrollmentPeriodId + "'>Edit</button></td></tr>");
        });
    }

    function openModal(title, html, mode, id) {
        modalMode = mode; editId = id || 0;
        $("#benModalTitle").text(title);
        $("#benModalBody").html(html);
        $("#benModal").modal("show");
    }

    $("#btnNewCat").on("click", function () {
        openModal("Category", "<div class='form-group'><label>Name</label><input id='mName' class='form-control'/></div>" +
            "<div class='form-group'><label>Description</label><input id='mDesc' class='form-control'/></div>" +
            "<div class='form-group'><label>Display Order</label><input id='mOrder' type='number' class='form-control' value='1'/></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' checked/> Active</label></div>", "cat", 0);
    });
    $(document).on("click", ".btn-edit-cat", function () {
        var id = $(this).attr("data-id");
        var c = cats.filter(function (x) { return String(x.CategoryId) === String(id); })[0];
        if (!c) return;
        openModal("Category", "<div class='form-group'><label>Name</label><input id='mName' class='form-control' value='" + (c.CategoryName || "") + "'/></div>" +
            "<div class='form-group'><label>Description</label><input id='mDesc' class='form-control' value='" + (c.Description || "") + "'/></div>" +
            "<div class='form-group'><label>Display Order</label><input id='mOrder' type='number' class='form-control' value='" + c.DisplayOrder + "'/></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' " + (c.IsActive ? "checked" : "") + "/> Active</label></div>", "cat", c.CategoryId);
    });

    $("#btnNewWait").on("click", function () {
        openModal("Waiting Period", "<div class='form-group'><label>Name</label><input id='mName' class='form-control'/></div>" +
            "<div class='form-group'><label>Days</label><input id='mDays' type='number' class='form-control' value='0'/></div>" +
            "<div class='form-group'><label>Calculation Type</label><select id='mType' class='form-control'><option>Days</option><option>FirstDayNextMonth</option></select></div>" +
            "<div class='form-group'><label>Description</label><input id='mDesc' class='form-control'/></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' checked/> Active</label></div>", "wait", 0);
    });
    $(document).on("click", ".btn-edit-wait", function () {
        var id = $(this).attr("data-id");
        var w = waits.filter(function (x) { return String(x.WaitingPeriodId) === String(id); })[0];
        if (!w) return;
        openModal("Waiting Period", "<div class='form-group'><label>Name</label><input id='mName' class='form-control' value='" + (w.Name || "") + "'/></div>" +
            "<div class='form-group'><label>Days</label><input id='mDays' type='number' class='form-control' value='" + w.Days + "'/></div>" +
            "<div class='form-group'><label>Calculation Type</label><input id='mType' class='form-control' value='" + (w.CalculationType || "Days") + "'/></div>" +
            "<div class='form-group'><label>Description</label><input id='mDesc' class='form-control' value='" + (w.Description || "") + "'/></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' " + (w.IsActive ? "checked" : "") + "/> Active</label></div>", "wait", w.WaitingPeriodId);
    });

    $("#btnNewElig").on("click", function () {
        openModal("Eligibility Rule", "<div class='form-group'><label>Name</label><input id='mName' class='form-control'/></div>" +
            "<div class='form-group'><label>Description</label><input id='mDesc' class='form-control'/></div>" +
            "<div class='form-group'><label>Min Service Days</label><input id='mDays' type='number' class='form-control'/></div>" +
            "<div class='form-group'><label>Min Hours</label><input id='mHours' type='number' step='0.01' class='form-control'/></div>" +
            "<div class='form-group'><label>Rule Expression</label><input id='mExpr' class='form-control' placeholder='Employment Status = Full Time AND Service Days >= 30'/></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' checked/> Active</label></div>", "elig", 0);
    });
    $(document).on("click", ".btn-edit-elig", function () {
        var id = $(this).attr("data-id");
        var e = eligs.filter(function (x) { return String(x.EligibilityRuleId) === String(id); })[0];
        if (!e) return;
        openModal("Eligibility Rule", "<div class='form-group'><label>Name</label><input id='mName' class='form-control' value='" + (e.RuleName || "") + "'/></div>" +
            "<div class='form-group'><label>Description</label><input id='mDesc' class='form-control' value='" + (e.Description || "") + "'/></div>" +
            "<div class='form-group'><label>Min Service Days</label><input id='mDays' type='number' class='form-control' value='" + (e.MinServiceDays != null ? e.MinServiceDays : "") + "'/></div>" +
            "<div class='form-group'><label>Min Hours</label><input id='mHours' type='number' step='0.01' class='form-control' value='" + (e.MinHours != null ? e.MinHours : "") + "'/></div>" +
            "<div class='form-group'><label>Rule Expression</label><input id='mExpr' class='form-control' value='" + (e.RuleExpression || "") + "'/></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' " + (e.IsActive ? "checked" : "") + "/> Active</label></div>", "elig", e.EligibilityRuleId);
    });

    function findCoverage(opts, code, nameHint) {
        opts = opts || [];
        var byCode = opts.filter(function (o) { return String(o.OptionCode || "").toUpperCase() === code; })[0];
        if (byCode) return byCode;
        return opts.filter(function (o) {
            return String(o.OptionName || "").toLowerCase().indexOf(nameHint) >= 0;
        })[0] || null;
    }

    function coverageRow(code, name, requiresDep, existing, defaultEmp, defaultEr, defaultOn) {
        var emp = existing && existing.EmployeeCost != null ? existing.EmployeeCost : (defaultEmp || 0);
        var er = existing && existing.EmployerCost != null ? existing.EmployerCost : (defaultEr || 0);
        var enabled = existing ? true : !!defaultOn;
        return "<tr class='m-cov-row' data-code='" + code + "' data-name='" + name + "' data-dep='" + (requiresDep ? "1" : "0") + "'>" +
            "<td><label class='checkbox-inline' style='margin:0'><input type='checkbox' class='m-cov-on' " + (enabled ? "checked" : "") + "/> " + name + "</label></td>" +
            "<td><input type='number' step='0.01' class='form-control input-sm m-cov-emp' value='" + emp + "'/></td>" +
            "<td><input type='number' step='0.01' class='form-control input-sm m-cov-er' value='" + er + "'/></td>" +
            "</tr>";
    }

    function planForm(p) {
        p = p || {};
        var opts = p.CoverageOptions || [];
        var isNew = !opts.length;
        var catOpts = cats.map(function (c) {
            return "<option value='" + c.CategoryId + "'" + (String(c.CategoryId) === String(p.CategoryId) ? " selected" : "") + ">" + c.CategoryName + "</option>";
        }).join("");
        var defEmp = p.EmployeeCost || 0;
        var defEr = p.EmployerCost || 0;
        var covHtml =
            coverageRow("EE", "Employee Only (EE)", false, findCoverage(opts, "EE", "employee only") || findCoverage(opts, "EE", "ee only"), defEmp, defEr, isNew) +
            coverageRow("ES", "Employee + Spouse", true, findCoverage(opts, "ES", "spouse"), defEmp, defEr, isNew) +
            coverageRow("EC", "Employee + Child", true, findCoverage(opts, "EC", "child"), defEmp, defEr, isNew) +
            coverageRow("EF", "Employee + Family", true, findCoverage(opts, "EF", "family"), defEmp, defEr, isNew);

        return "<div class='row'><div class='col-md-6'><div class='form-group'><label>Plan Name</label><input id='mName' class='form-control' value='" + (p.PlanName || "") + "'/></div></div>" +
            "<div class='col-md-3'><div class='form-group'><label>Code</label><input id='mCode' class='form-control' value='" + (p.PlanCode || "") + "'/></div></div>" +
            "<div class='col-md-3'><div class='form-group'><label>Category</label><select id='mCat' class='form-control'>" + catOpts + "</select></div></div></div>" +
            "<div class='row'><div class='col-md-4'><div class='form-group'><label>Carrier</label><input id='mCarrier' class='form-control' value='" + (p.Carrier || "") + "'/></div></div>" +
            "<div class='col-md-4'><div class='form-group'><label>Default EE Cost</label><input id='mEmpCost' type='number' step='0.01' class='form-control' value='" + defEmp + "'/></div></div>" +
            "<div class='col-md-4'><div class='form-group'><label>Default ER Cost</label><input id='mErCost' type='number' step='0.01' class='form-control' value='" + defEr + "'/></div></div></div>" +
            "<div class='checkbox'><label><input id='mWaive' type='checkbox' " + (p.WaiveAllowed !== false ? "checked" : "") + "/> Waive allowed</label></div>" +
            "<div class='checkbox'><label><input id='mReqDep' type='checkbox' " + (p.RequireDependents ? "checked" : "") + "/> Require dependents</label></div>" +
            "<div class='checkbox'><label><input id='mReqBen' type='checkbox' " + (p.RequireBeneficiary ? "checked" : "") + "/> Require beneficiary</label></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' " + (p.IsActive !== false ? "checked" : "") + "/> Active</label></div>" +
            "<h5 class='m-t-15'>Coverage Options</h5>" +
            "<table class='table table-bordered table-condensed' id='mCovTable'>" +
            "<thead><tr><th>Coverage</th><th>EE Cost</th><th>ER Cost</th></tr></thead>" +
            "<tbody>" + covHtml + "</tbody></table>" +
            "<p class='text-muted small'>Enable tiers and set employee / employer cost for each coverage level.</p>";
    }
    $("#btnNewPlan").on("click", function () { openModal("Benefit Plan", planForm(), "plan", 0); });
    $(document).on("click", ".btn-edit-plan", function () {
        var id = $(this).attr("data-id");
        var p = plans.filter(function (x) { return String(x.PlanId) === String(id); })[0];
        if (!p) return;
        openModal("Benefit Plan", planForm(p), "plan", p.PlanId);
    });

    function collectCoverageOptions() {
        var list = [];
        var sort = 0;
        $("#mCovTable .m-cov-row").each(function () {
            var $r = $(this);
            if (!$r.find(".m-cov-on").is(":checked")) return;
            list.push({
                OptionCode: $r.data("code"),
                OptionName: $r.data("name"),
                EmployeeCost: parseFloat($r.find(".m-cov-emp").val() || "0"),
                EmployerCost: parseFloat($r.find(".m-cov-er").val() || "0"),
                RequiresDependent: $r.data("dep") === 1 || $r.data("dep") === "1",
                SortOrder: ++sort,
                IsActive: true
            });
        });
        return list;
    }

    function classForm(c) {
        c = c || { PlanIds: [] };
        var wOpts = "<option value=''>--None--</option>" + waits.map(function (w) {
            return "<option value='" + w.WaitingPeriodId + "'" + (String(w.WaitingPeriodId) === String(c.WaitingPeriodId) ? " selected" : "") + ">" + w.Name + "</option>";
        }).join("");
        var eOpts = "<option value=''>--None--</option>" + eligs.map(function (e) {
            return "<option value='" + e.EligibilityRuleId + "'" + (String(e.EligibilityRuleId) === String(c.EligibilityRuleId) ? " selected" : "") + ">" + e.RuleName + "</option>";
        }).join("");
        var pChecks = plans.map(function (p) {
            var checked = (c.PlanIds || []).indexOf(p.PlanId) >= 0 || (c.PlanIds || []).map(String).indexOf(String(p.PlanId)) >= 0;
            return "<div class='checkbox'><label><input type='checkbox' class='m-plan' value='" + p.PlanId + "' " + (checked ? "checked" : "") + "/> " + p.PlanName + " (" + (p.CategoryName || "") + ")</label></div>";
        }).join("");
        return "<div class='form-group'><label>Class Name</label><input id='mName' class='form-control' value='" + (c.ClassName || "") + "'/></div>" +
            "<div class='form-group'><label>Description</label><input id='mDesc' class='form-control' value='" + (c.Description || "") + "'/></div>" +
            "<div class='form-group'><label>Waiting Period</label><select id='mWait' class='form-control'>" + wOpts + "</select></div>" +
            "<div class='form-group'><label>Eligibility Rule</label><select id='mElig' class='form-control'>" + eOpts + "</select></div>" +
            "<div class='checkbox'><label><input id='mActive' type='checkbox' " + (c.IsActive !== false ? "checked" : "") + "/> Active</label></div>" +
            "<label>Assign Plans</label>" + pChecks;
    }
    $("#btnNewClass").on("click", function () { openModal("Benefit Class", classForm(), "class", 0); });
    $(document).on("click", ".btn-edit-class", function () {
        var id = $(this).attr("data-id");
        var c = classes.filter(function (x) { return String(x.BenefitClassId) === String(id); })[0];
        if (!c) return;
        openModal("Benefit Class", classForm(c), "class", c.BenefitClassId);
    });

    $("#btnNewOe").on("click", function () {
        openModal("Open Enrollment", "<div class='form-group'><label>Name</label><input id='mName' class='form-control' placeholder='Open Enrollment 2027'/></div>" +
            "<div class='row'><div class='col-md-4'><div class='form-group'><label>Start</label><input id='mStart' type='date' class='form-control'/></div></div>" +
            "<div class='col-md-4'><div class='form-group'><label>End</label><input id='mEnd' type='date' class='form-control'/></div></div>" +
            "<div class='col-md-4'><div class='form-group'><label>Coverage Effective</label><input id='mEff' type='date' class='form-control'/></div></div></div>" +
            "<div class='form-group'><label>Status</label><select id='mStatus' class='form-control'><option>Draft</option><option>Active</option><option>Closed</option></select></div>" +
            "<div class='form-group'><label>Message</label><textarea id='mMsg' class='form-control' rows='2'></textarea></div>" +
            "<div class='checkbox'><label><input id='mRemind' type='checkbox' checked/> Reminder emails</label></div>", "oe", 0);
    });
    $(document).on("click", ".btn-edit-oe", function () {
        var id = $(this).attr("data-id");
        var p = oes.filter(function (x) { return String(x.EnrollmentPeriodId) === String(id); })[0];
        if (!p) return;
        openModal("Open Enrollment", "<div class='form-group'><label>Name</label><input id='mName' class='form-control' value='" + (p.EnrollmentName || "") + "'/></div>" +
            "<div class='row'><div class='col-md-4'><div class='form-group'><label>Start</label><input id='mStart' type='date' class='form-control' value='" + fmtDate(p.StartDate) + "'/></div></div>" +
            "<div class='col-md-4'><div class='form-group'><label>End</label><input id='mEnd' type='date' class='form-control' value='" + fmtDate(p.EndDate) + "'/></div></div>" +
            "<div class='col-md-4'><div class='form-group'><label>Coverage Effective</label><input id='mEff' type='date' class='form-control' value='" + fmtDate(p.CoverageEffectiveDate) + "'/></div></div></div>" +
            "<div class='form-group'><label>Status</label><select id='mStatus' class='form-control'><option " + (p.Status === "Draft" ? "selected" : "") + ">Draft</option><option " + (p.Status === "Active" ? "selected" : "") + ">Active</option><option " + (p.Status === "Closed" ? "selected" : "") + ">Closed</option></select></div>" +
            "<div class='form-group'><label>Message</label><textarea id='mMsg' class='form-control' rows='2'>" + (p.EnrollmentMessage || "") + "</textarea></div>" +
            "<div class='checkbox'><label><input id='mRemind' type='checkbox' " + (p.ReminderEmails ? "checked" : "") + "/> Reminder emails</label></div>", "oe", p.EnrollmentPeriodId);
    });

    $("#benModalSave").on("click", function () {
        var payload, postUrl;
        if (modalMode === "cat") {
            postUrl = url("cat-save");
            payload = { categoryId: editId, categoryName: $("#mName").val(), description: $("#mDesc").val(), displayOrder: $("#mOrder").val() || 0, isActive: $("#mActive").is(":checked") };
        } else if (modalMode === "wait") {
            postUrl = url("wait-save");
            payload = { waitingPeriodId: editId, name: $("#mName").val(), days: $("#mDays").val() || 0, calculationType: $("#mType").val(), description: $("#mDesc").val(), isActive: $("#mActive").is(":checked") };
        } else if (modalMode === "elig") {
            postUrl = url("elig-save");
            payload = { EligibilityRuleId: editId, RuleName: $("#mName").val(), Description: $("#mDesc").val(), MinServiceDays: $("#mDays").val() || null, MinHours: $("#mHours").val() || null, RuleExpression: $("#mExpr").val(), IsActive: $("#mActive").is(":checked") };
        } else if (modalMode === "plan") {
            postUrl = url("plan-save");
            var cov = collectCoverageOptions();
            if (!cov.length) { alert("Select at least one coverage option (EE / EE+Spouse / EE+Child / Family)."); return; }
            payload = {
                PlanId: editId,
                PlanName: $("#mName").val(),
                PlanCode: $("#mCode").val(),
                CategoryId: $("#mCat").val(),
                Carrier: $("#mCarrier").val(),
                EmployeeCost: $("#mEmpCost").val() || 0,
                EmployerCost: $("#mErCost").val() || 0,
                WaiveAllowed: $("#mWaive").is(":checked"),
                RequireDependents: $("#mReqDep").is(":checked"),
                RequireBeneficiary: $("#mReqBen").is(":checked"),
                IsActive: $("#mActive").is(":checked"),
                coverageOptionsJson: JSON.stringify(cov)
            };
        } else if (modalMode === "class") {
            postUrl = url("class-save");
            var ids = [];
            $(".m-plan:checked").each(function () { ids.push($(this).val()); });
            payload = { benefitClassId: editId, className: $("#mName").val(), description: $("#mDesc").val(), waitingPeriodId: $("#mWait").val() || null, eligibilityRuleId: $("#mElig").val() || null, isActive: $("#mActive").is(":checked"), planIds: ids.join(",") };
        } else if (modalMode === "oe") {
            postUrl = url("oe-save");
            payload = {
                EnrollmentPeriodId: editId,
                EnrollmentName: $("#mName").val(),
                startDate: $("#mStart").val(),
                endDate: $("#mEnd").val(),
                coverageEffectiveDate: $("#mEff").val() || "",
                Status: $("#mStatus").val(),
                EnrollmentMessage: $("#mMsg").val(),
                ReminderEmails: $("#mRemind").is(":checked")
            };
        } else return;
        $.post(postUrl, payload, function (res) {
            if (!res || !res.success) { alert((res && res.message) || "Save failed"); return; }
            $("#benModal").modal("hide");
            loadAll();
        });
    });

    loadAll();
})();
