(function () {
    var $root = $("#benPortalRoot");
    if (!$root.length) return;
    function url(n) { return $root.attr("data-" + n) || ""; }

    var plans = [];
    var enrollmentId = parseInt($root.attr("data-enrollment-id") || "0", 10) || 0;
    var elections = {}; // planId -> { coverageOptionId, isWaived }

    function money(n) { return "$" + (Number(n || 0).toFixed(2)); }

    function renderPlans() {
        var $list = $("#benPlansList").empty();
        var byCat = {};
        plans.forEach(function (p) {
            var cat = p.CategoryName || "Other";
            if (!byCat[cat]) byCat[cat] = [];
            byCat[cat].push(p);
        });
        Object.keys(byCat).forEach(function (cat) {
            $list.append("<h5 class='m-t-15'>" + cat + "</h5>");
            byCat[cat].forEach(function (p) {
                var opts = (p.CoverageOptions || []).map(function (o) {
                    return "<option value='" + o.CoverageOptionId + "' data-emp='" + o.EmployeeCost + "' data-er='" + o.EmployerCost + "' data-dep='" + (o.RequiresDependent ? "1" : "0") + "'>" +
                        o.OptionName + " (EE " + money(o.EmployeeCost) + ")</option>";
                }).join("");
                var waive = p.WaiveAllowed ? "<div class='checkbox'><label><input type='checkbox' class='ben-waive' data-plan='" + p.PlanId + "'/> Waive coverage</label></div>" : "";
                $list.append("<div class='card m-b-10 ben-plan-card' data-plan='" + p.PlanId + "' data-req-dep='" + (p.RequireDependents ? "1" : "0") + "' data-req-ben='" + (p.RequireBeneficiary ? "1" : "0") + "'>" +
                    "<div class='card-block'><strong>" + p.PlanName + "</strong>" + (p.Carrier ? " — " + p.Carrier : "") +
                    "<div class='form-group m-t-10'><label>Coverage</label><select class='form-control ben-coverage' data-plan='" + p.PlanId + "'>" +
                    "<option value=''>-- Select --</option>" + opts + "</select></div>" + waive + "</div></div>");
            });
        });
        updateCost();
        toggleExtraPanels();
    }

    function updateCost() {
        var emp = 0, er = 0;
        $(".ben-plan-card").each(function () {
            var $card = $(this);
            var planId = $card.data("plan");
            if ($card.find(".ben-waive").is(":checked")) return;
            var $opt = $card.find(".ben-coverage option:selected");
            if (!$opt.val()) return;
            emp += parseFloat($opt.attr("data-emp") || "0");
            er += parseFloat($opt.attr("data-er") || "0");
            elections[planId] = { coverageOptionId: parseInt($opt.val(), 10), isWaived: false };
        });
        $(".ben-waive:checked").each(function () {
            elections[$(this).data("plan")] = { coverageOptionId: null, isWaived: true };
        });
        $("#benCostSummary").html("<p><strong>Employee monthly:</strong> " + money(emp) +
            " &nbsp;|&nbsp; <strong>Employer monthly:</strong> " + money(er) +
            " &nbsp;|&nbsp; <strong>Payroll deduction:</strong> " + money(emp) + "</p>");
    }

    function toggleExtraPanels() {
        var needDep = false, needBen = false;
        $(".ben-plan-card").each(function () {
            var $card = $(this);
            if ($card.find(".ben-waive").is(":checked")) return;
            var $opt = $card.find(".ben-coverage option:selected");
            if (!$opt.val()) return;
            if ($card.attr("data-req-dep") === "1" || $opt.attr("data-dep") === "1") needDep = true;
            if ($card.attr("data-req-ben") === "1") needBen = true;
        });
        $("#benDepsPanel").toggle(needDep);
        $("#benBenePanel").toggle(needBen);
    }

    function collectElections() {
        var list = [];
        plans.forEach(function (p) {
            var $card = $(".ben-plan-card[data-plan='" + p.PlanId + "']");
            if (!$card.length) return;
            var waived = $card.find(".ben-waive").is(":checked");
            var cov = $card.find(".ben-coverage").val();
            if (waived) list.push({ PlanId: p.PlanId, CoverageOptionId: null, IsWaived: true });
            else if (cov) list.push({ PlanId: p.PlanId, CoverageOptionId: parseInt(cov, 10), IsWaived: false });
        });
        return list;
    }

    function collectDeps() {
        var list = [];
        $("#benDepsList .ben-dep-row").each(function () {
            var $r = $(this);
            list.push({
                FirstName: $r.find(".dep-first").val(),
                LastName: $r.find(".dep-last").val(),
                Relationship: $r.find(".dep-rel").val(),
                DateOfBirth: $r.find(".dep-dob").val() || null,
                Gender: $r.find(".dep-gender").val(),
                SSN: $r.find(".dep-ssn").val()
            });
        });
        return list;
    }

    function collectBenes() {
        var list = [];
        $("#benBeneList .ben-bene-row").each(function () {
            var $r = $(this);
            list.push({
                Name: $r.find(".bene-name").val(),
                Relationship: $r.find(".bene-rel").val(),
                Percentage: parseFloat($r.find(".bene-pct").val() || "0")
            });
        });
        return list;
    }

    $("#btnAddDep").on("click", function () {
        $("#benDepsList").append("<div class='row ben-dep-row m-b-5'>" +
            "<div class='col-md-2'><input class='form-control dep-first' placeholder='First'/></div>" +
            "<div class='col-md-2'><input class='form-control dep-last' placeholder='Last'/></div>" +
            "<div class='col-md-2'><input class='form-control dep-rel' placeholder='Relationship'/></div>" +
            "<div class='col-md-2'><input type='date' class='form-control dep-dob'/></div>" +
            "<div class='col-md-2'><input class='form-control dep-gender' placeholder='Gender'/></div>" +
            "<div class='col-md-2'><input class='form-control dep-ssn' placeholder='SSN'/></div></div>");
    });
    $("#btnAddBene").on("click", function () {
        $("#benBeneList").append("<div class='row ben-bene-row m-b-5'>" +
            "<div class='col-md-5'><input class='form-control bene-name' placeholder='Name'/></div>" +
            "<div class='col-md-4'><input class='form-control bene-rel' placeholder='Relationship'/></div>" +
            "<div class='col-md-3'><input type='number' class='form-control bene-pct' placeholder='%' step='0.01'/></div></div>");
    });

    $(document).on("change", ".ben-coverage, .ben-waive", function () {
        var $card = $(this).closest(".ben-plan-card");
        if ($(this).hasClass("ben-waive") && $(this).is(":checked")) $card.find(".ben-coverage").prop("disabled", true);
        else if ($(this).hasClass("ben-waive")) $card.find(".ben-coverage").prop("disabled", false);
        updateCost();
        toggleExtraPanels();
    });

    function ensureEnrollment(done) {
        if (enrollmentId > 0) { done(true); return; }
        $.post(url("start-url"), function (res) {
            if (!res || !res.success) { alert((res && res.message) || "Cannot start enrollment"); done(false); return; }
            enrollmentId = res.enrollmentId;
            if (res.plans && res.plans.length) { plans = res.plans; renderPlans(); }
            done(true);
        });
    }

    $("#btnStartBen").on("click", function () {
        ensureEnrollment(function (ok) {
            if (!ok) return;
            var electionsList = collectElections();
            if (!electionsList.length) { alert("Select at least one plan coverage or waive."); return; }
            $.post(url("save-elect-url"), { enrollmentId: enrollmentId, electionsJson: JSON.stringify(electionsList) }, function (res) {
                if (!res || !res.success) { alert((res && res.message) || "Save failed"); return; }
                $.post(url("save-dep-url"), { enrollmentId: enrollmentId, dependentsJson: JSON.stringify(collectDeps()) }, function () {
                    $.post(url("save-ben-url"), { enrollmentId: enrollmentId, beneficiariesJson: JSON.stringify(collectBenes()) }, function (bres) {
                        if (bres && !bres.success) { alert(bres.message || "Beneficiary save failed"); return; }
                        alert("Elections saved. Review cost, accept terms, and submit.");
                    });
                });
            });
        });
    });

    $("#btnSubmitBen").on("click", function () {
        ensureEnrollment(function (ok) {
            if (!ok) return;
            if (!$("#benTerms").is(":checked")) { alert("Accept the benefit terms."); return; }
            var name = ($("#benSignedName").val() || "").trim();
            if (!name) { alert("Enter your electronic signature name."); return; }
            var electionsList = collectElections();
            $.post(url("save-elect-url"), { enrollmentId: enrollmentId, electionsJson: JSON.stringify(electionsList) }, function () {
                $.post(url("save-dep-url"), { enrollmentId: enrollmentId, dependentsJson: JSON.stringify(collectDeps()) }, function () {
                    $.post(url("save-ben-url"), { enrollmentId: enrollmentId, beneficiariesJson: JSON.stringify(collectBenes()) }, function (bres) {
                        if (bres && bres.success === false) { alert(bres.message || "Beneficiary validation failed"); return; }
                        $.post(url("submit-url"), { enrollmentId: enrollmentId, signedName: name, termsAccepted: true }, function (res) {
                            if (!res || !res.success) { alert((res && res.message) || "Submit failed"); return; }
                            alert((res.message || "Success") + "\nConfirmation: " + (res.confirmationNumber || ""));
                            if (typeof LoadMenu === "function") LoadMenu("MyBenefitsPartial", "BenefitsEnrollment");
                        });
                    });
                });
            });
        });
    });

    if ($root.attr("data-eligible") === "1") {
        $.getJSON(url("portal-url"), function (r) {
            if (r && r.success && r.data && r.data.Plans) {
                plans = r.data.Plans;
                if (r.data.EnrollmentId) enrollmentId = r.data.EnrollmentId;
                renderPlans();
            }
        });
    }
})();
