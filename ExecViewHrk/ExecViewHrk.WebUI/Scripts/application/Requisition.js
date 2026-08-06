(function () {
    var $root = $("#requisitionDashboardRoot");
    if (!$root.length) return;

    var lookupsLoaded = false;
    var positionLookup = [];
    var currentRequisitionId = 0;
    var currentApplicationId = 0;

    function esc(value) {
        return $("<div/>").text(value == null ? "" : String(value)).html();
    }

    function toDateInput(val) {
        if (!val) return "";
        var d = new Date(val);
        if (isNaN(d.getTime())) {
            if (typeof val === "string" && val.indexOf("/Date(") === 0) {
                var ms = parseInt(val.replace(/\/Date\((-?\d+)\)\//, "$1"), 10);
                d = new Date(ms);
            } else if (typeof val === "string" && val.length >= 10) {
                return val.substring(0, 10);
            } else return "";
        }
        var m = (d.getMonth() + 1);
        var day = d.getDate();
        return d.getFullYear() + "-" + (m < 10 ? "0" : "") + m + "-" + (day < 10 ? "0" : "") + day;
    }

    function fillSelect($sel, items, placeholder) {
        $sel.empty().append($("<option/>").val("").text(placeholder || "Select"));
        (items || []).forEach(function (item) {
            $sel.append($("<option/>").val(item.id).text(item.text));
        });
    }

    function loadLookups(done) {
        if (lookupsLoaded) {
            if (done) done();
            return;
        }
        $.getJSON($root.data("lookups-url"), function (res) {
            if (!res.success) {
                alert(res.message || "Unable to load dropdowns.");
                return;
            }
            positionLookup = res.positions || [];
            fillSelect($("#reqPositionId"), res.positions, "Select position");
            fillSelect($("#reqDivisionId"), res.divisions, "Select division");
            fillSelect($("#reqDepartmentId"), res.departments, "Select department");
            fillSelect($("#reqReportToPositionId"), res.reportToPositions, "Select report-to position");
            lookupsLoaded = true;
            if (done) done();
        }).fail(function () {
            alert("Unable to load dropdowns.");
        });
    }

    function clearForm() {
        $("#reqId").val("0");
        $("#reqNumber").val("");
        $("#reqPositionId").val("");
        $("#reqDivisionId").val("");
        $("#reqDepartmentId").val("");
        $("#reqReportToPositionId").val("");
        $("#reqDescription").val("");
        $("#reqStatus").val("Open");
        $("#reqPublished").val("true");
        $("#reqDate").val(toDateInput(new Date()));
        $("#reqOpenDate").val("");
        $("#reqClosedDate").val("");
        $("#reqEditTitle").text("Add Requisition");
    }

    function applyPositionDefaults(positionId, force) {
        var id = parseInt(positionId, 10) || 0;
        if (!id) return;
        var pos = null;
        for (var i = 0; i < positionLookup.length; i++) {
            if (positionLookup[i].id === id) { pos = positionLookup[i]; break; }
        }
        if (!pos) return;
        if (force || !$("#reqDepartmentId").val()) {
            if (pos.departmentId) $("#reqDepartmentId").val(String(pos.departmentId));
        }
        if (force || !$("#reqDivisionId").val()) {
            if (pos.businessUnitId) $("#reqDivisionId").val(String(pos.businessUnitId));
        }
        if (force || !$("#reqReportToPositionId").val()) {
            if (pos.reportToPositionId) $("#reqReportToPositionId").val(String(pos.reportToPositionId));
        }
    }

    function reloadList() {
        LoadMenu("RequisitionDashboardPartial", "Requisition");
    }

    $("#btnAddRequisition").off("click").on("click", function () {
        loadLookups(function () {
            clearForm();
            $("#reqEditPanel").show();
            $("#reqApplicantsPanel").hide();
        });
    });

    $("#btnCancelRequisition").off("click").on("click", function () {
        $("#reqEditPanel").hide();
    });

    $("#btnCloseApps").off("click").on("click", function () {
        $("#reqApplicantsPanel").hide();
        $("#applicantDetailPanel").hide();
    });

    $("#btnCloseApplicantDetail").off("click").on("click", function () {
        $("#applicantDetailPanel").hide();
    });

    $("#reqPositionId").off("change").on("change", function () {
        applyPositionDefaults($(this).val(), true);
    });

    $root.off("click", ".btn-edit-req").on("click", ".btn-edit-req", function () {
        var id = $(this).closest("tr").data("id");
        loadLookups(function () {
            $.getJSON($root.data("get-url"), { id: id }, function (res) {
                if (!res.success) { alert(res.message || "Failed"); return; }
                var d = res.data;
                $("#reqId").val(d.RequisitionId);
                $("#reqNumber").val(d.RequisitionNumber);
                $("#reqPositionId").val(d.PositionId ? String(d.PositionId) : "");
                $("#reqDivisionId").val(d.BusinessUnitId ? String(d.BusinessUnitId) : "");
                $("#reqDepartmentId").val(d.DepartmentId ? String(d.DepartmentId) : "");
                $("#reqReportToPositionId").val(d.ReportToPositionId ? String(d.ReportToPositionId) : "");
                $("#reqDescription").val(d.Description || "");
                $("#reqStatus").val(d.Status || "Open");
                $("#reqPublished").val(d.IsPublished ? "true" : "false");
                $("#reqDate").val(toDateInput(d.RequisitionDate));
                $("#reqOpenDate").val(toDateInput(d.OpenDate));
                $("#reqClosedDate").val(toDateInput(d.ClosedDate));
                $("#reqEditTitle").text("Edit Requisition");
                $("#reqEditPanel").show();
                $("#reqApplicantsPanel").hide();
            });
        });
    });

    $root.off("click", ".btn-del-req").on("click", ".btn-del-req", function () {
        if (!confirm("Delete this requisition?")) return;
        var id = $(this).closest("tr").data("id");
        $.post($root.data("delete-url"), { id: id }, function (res) {
            if (!res.success) { alert(res.message || "Failed"); return; }
            reloadList();
        });
    });

    $root.off("click", ".btn-apps-req").on("click", ".btn-apps-req", function () {
        var id = $(this).closest("tr").data("id");
        currentRequisitionId = id;
        var title = $(this).closest("tr").find("td").eq(3).text();
        $("#reqAppsTitle").text("- " + title);
        loadApplicants(id);
    });

    function loadApplicants(requisitionId) {
        $.getJSON($root.data("applicants-url"), { requisitionId: requisitionId }, function (res) {
            var $tb = $("#reqAppsTable tbody").empty();
            if (!res.success) { alert(res.message || "Failed"); return; }
            var apps = res.data || [];
            var applicantCount = 0;
            var candidateCount = 0;
            apps.forEach(function (a) {
                if (a.Status === "Submitted") applicantCount++;
                if (a.Status === "Candidate" || a.Status === "Hire") candidateCount++;
                var hireButton = a.Status === "Hire"
                    ? "<span class='label label-success'>Hired</span>"
                    : "<button type='button' class='btn btn-success btn-xs btn-hire-app' data-app='" + a.ApplicationId +
                      "'" + (a.CanHire ? "" : " disabled title='Convert to Candidate before hiring'") + ">Click to Hire</button>";
                var actions = "<select class='form-control input-sm applicant-action' data-app='" + a.ApplicationId + "'>" +
                    "<option value=''>Select action</option>";
                if (a.Status === "Submitted") actions += "<option value='Candidate'>Convert to Candidate</option>";
                if (a.Status !== "Hire" && a.Status !== "Rejected") actions += "<option value='Reject'>Reject</option>";
                actions += "</select>";

                $tb.append("<tr data-app='" + a.ApplicationId + "'>" +
                    "<td>" + esc(a.ApplicantName) + "</td>" +
                    "<td>" + hireButton + "</td>" +
                    "<td>" + esc(a.ApplicantType) + "</td>" +
                    "<td><button type='button' class='btn btn-info btn-xs btn-view-app' data-app='" + a.ApplicationId + "'>Quick view</button></td>" +
                    "<td>" + (a.CreatedDate ? toDateInput(a.CreatedDate) : "") + "</td>" +
                    "<td>-</td>" +
                    "<td><strong>" + esc(a.Status) + "</strong></td>" +
                    "<td>" + actions + "</td></tr>");
            });
            if (!apps.length) $tb.append("<tr><td colspan='8'>No applicants yet.</td></tr>");

            // Keep dashboard Applicants / Candidates columns in sync
            var $reqRow = $("#reqTable tr[data-id='" + requisitionId + "']");
            if ($reqRow.length) {
                $reqRow.find("td").eq(9).text(applicantCount);
                $reqRow.find("td").eq(10).text(candidateCount);
            }

            $("#reqApplicantsPanel").show();
            $("#reqEditPanel").hide();
        });
    }

    function detailSection(title, rows) {
        if (!rows || !rows.length) return "";
        var html = "<h6 style='margin-top:15px;'>" + esc(title) + "</h6><div class='table-responsive'><table class='table table-bordered table-condensed'><tbody>";
        rows.forEach(function (row) {
            html += "<tr>";
            row.forEach(function (cell) { html += "<td>" + (cell && cell.raw ? cell.value : esc(cell)) + "</td>"; });
            html += "</tr>";
        });
        return html + "</tbody></table></div>";
    }

    $root.off("click", ".btn-view-app").on("click", ".btn-view-app", function () {
        var applicationId = $(this).data("app");
        $.getJSON($root.data("applicant-details-url"), { applicationId: applicationId }, function (res) {
            if (!res.success) { alert(res.message || "Unable to load application."); return; }
            var d = res.data;
            currentApplicationId = d.ApplicationId;
            $("#applicantDetailName").text(d.ApplicantName + " - " + d.PositionTitle);

            var html = detailSection("Applicant", [
                ["Name", d.ApplicantName, "Status", d.Status],
                ["Email", d.Email || "", "Phone", d.Phone || ""],
                ["Address", d.Address || "", "Submitted", d.SubmittedDate ? toDateInput(d.SubmittedDate) : ""]
            ]);
            html += detailSection("Question Answers", (d.Answers || []).map(function (x) {
                return [x.Question, x.Answer || ""];
            }));
            html += detailSection("Documents", (d.Files || []).map(function (x) {
                var link = $root.data("applicant-download-url") + "?fileId=" + x.FileId;
                return [x.FileCategory, { raw: true, value: "<a href='" + esc(link) + "'>" + esc(x.FileName) + "</a>" }];
            }));
            html += detailSection("References", (d.References || []).map(function (x) {
                return [x.FullName, x.Relationship || "", x.Company || "", x.Phone || "", x.Email || ""];
            }));
            html += detailSection("Employment History", (d.Employment || []).map(function (x) {
                return [x.EmployerName, x.JobTitle || "", x.StartDate ? toDateInput(x.StartDate) : "", x.EndDate ? toDateInput(x.EndDate) : "", x.Duties || ""];
            }));
            html += detailSection("Education", (d.Education || []).map(function (x) {
                return [x.SchoolName, x.Degree || "", x.FieldOfStudy || "", x.GraduationYear || ""];
            }));
            html += detailSection("Signatures", (d.Signatures || []).map(function (x) {
                return [x.SignatureType, x.SignerName, x.SignedDate ? toDateInput(x.SignedDate) : ""];
            }));
            $("#applicantDetailContent").html(html);
            $("#applicantDetailPanel").show();
        });
    });

    function updateApplicant(applicationId, actionType, comment) {
        $.post($root.data("applicant-update-url"), {
            applicationId: applicationId,
            actionType: actionType,
            comment: comment
        }, function (res) {
            if (!res.success) { alert(res.message || "Update failed."); return; }
            alert("Applicant status changed to " + res.status + ".");
            loadApplicants(currentRequisitionId);
            if (currentApplicationId === applicationId) {
                $("#applicantDetailPanel").hide();
                currentApplicationId = 0;
            }
        });
    }

    $root.off("change", ".applicant-action").on("change", ".applicant-action", function () {
        var actionType = $(this).val();
        var applicationId = $(this).data("app");
        $(this).val("");
        if (!actionType) return;
        var comment = "";
        if (actionType === "Reject") {
            comment = prompt("Enter rejection reason:");
            if (!$.trim(comment || "")) return;
        }
        if (!confirm(actionType === "Candidate" ? "Convert this applicant to Candidate?" : "Reject this applicant?")) return;
        updateApplicant(applicationId, actionType, comment);
    });

    $root.off("click", ".btn-hire-app").on("click", ".btn-hire-app", function () {
        var applicationId = $(this).data("app");
        if (!confirm("Hire this candidate? Status will change to Hire.")) return;
        updateApplicant(applicationId, "Hire", "");
    });

    $("#btnEmployerCareerLink").off("click").on("click", function () {
        $.getJSON($root.data("quickapply-url"), function (res) {
            if (!res.success) { alert(res.message || "Failed"); return; }
            prompt("Employer Career Portal link (copy and share):", res.url);
        });
    });

    $("#btnSaveRequisition").off("click").on("click", function () {
        var positionId = parseInt($("#reqPositionId").val(), 10) || 0;
        if (!positionId) {
            alert("Please select a Position.");
            return;
        }
        var payload = {
            RequisitionId: parseInt($("#reqId").val(), 10) || 0,
            RequisitionNumber: $("#reqNumber").val(),
            PositionId: positionId,
            BusinessUnitId: parseInt($("#reqDivisionId").val(), 10) || null,
            DepartmentId: parseInt($("#reqDepartmentId").val(), 10) || null,
            ReportToPositionId: parseInt($("#reqReportToPositionId").val(), 10) || null,
            Description: $("#reqDescription").val(),
            Status: $("#reqStatus").val(),
            IsPublished: $("#reqPublished").val() === "true",
            RequisitionDate: $("#reqDate").val() || null,
            OpenDate: $("#reqOpenDate").val() || null,
            ClosedDate: $("#reqClosedDate").val() || null
        };
        $.ajax({
            url: $root.data("save-url"),
            type: "POST",
            data: payload,
            success: function (res) {
                if (!res.success) { alert(res.message || "Save failed"); return; }
                reloadList();
            },
            error: function () { alert("Save failed"); }
        });
    });
})();
