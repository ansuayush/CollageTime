(function () {
    var $root = $("#requisitionDashboardRoot");
    if (!$root.length) return;

    function toDateInput(val) {
        if (!val) return "";
        var d = new Date(val);
        if (isNaN(d.getTime())) {
            // ASP.NET /Date(ms)/ or already yyyy-MM-dd
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

    function clearForm() {
        $("#reqId").val("0");
        $("#reqNumber").val("");
        $("#reqTitle").val("");
        $("#reqDivision").val("");
        $("#reqDepartment").val("");
        $("#reqDescription").val("");
        $("#reqStatus").val("Open");
        $("#reqPublished").val("true");
        $("#reqDate").val(toDateInput(new Date()));
        $("#reqOpenDate").val("");
        $("#reqClosedDate").val("");
        $("#reqEditTitle").text("Add Requisition");
    }

    function reloadList() {
        LoadMenu("RequisitionDashboardPartial", "Requisition");
    }

    $("#btnAddRequisition").off("click").on("click", function () {
        clearForm();
        $("#reqEditPanel").show();
        $("#reqApplicantsPanel").hide();
    });

    $("#btnCancelRequisition").off("click").on("click", function () {
        $("#reqEditPanel").hide();
    });

    $("#btnCloseApps").off("click").on("click", function () {
        $("#reqApplicantsPanel").hide();
    });

    $root.off("click", ".btn-edit-req").on("click", ".btn-edit-req", function () {
        var id = $(this).closest("tr").data("id");
        $.getJSON($root.data("get-url"), { id: id }, function (res) {
            if (!res.success) { alert(res.message || "Failed"); return; }
            var d = res.data;
            $("#reqId").val(d.RequisitionId);
            $("#reqNumber").val(d.RequisitionNumber);
            $("#reqTitle").val(d.PositionTitle);
            $("#reqDivision").val(d.Division || "");
            $("#reqDepartment").val(d.Department || "");
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
        var title = $(this).closest("tr").find("td").eq(3).text();
        $("#reqAppsTitle").text("— " + title);
        $.getJSON($root.data("applicants-url"), { requisitionId: id }, function (res) {
            var $tb = $("#reqAppsTable tbody").empty();
            if (!res.success) { alert(res.message || "Failed"); return; }
            (res.data || []).forEach(function (a) {
                $tb.append("<tr><td>" + (a.ApplicantName || "") + "</td><td>" + (a.Status || "") +
                    "</td><td>" + a.CurrentStep + "</td><td>" + (a.CreatedDate ? toDateInput(a.CreatedDate) : "") +
                    "</td><td>" + (a.SubmittedDate ? toDateInput(a.SubmittedDate) : "") + "</td></tr>");
            });
            if (!(res.data || []).length) $tb.append("<tr><td colspan='5'>No applicants yet.</td></tr>");
            $("#reqApplicantsPanel").show();
            $("#reqEditPanel").hide();
        });
    });

    $root.off("click", ".btn-quick-req").on("click", ".btn-quick-req", function () {
        var id = $(this).closest("tr").data("id");
        $.getJSON($root.data("quickapply-url"), { requisitionId: id }, function (res) {
            if (!res.success) { alert(res.message || "Failed"); return; }
            prompt("Quick Apply / external career link (copy):", res.url);
        });
    });

    $("#btnSaveRequisition").off("click").on("click", function () {
        var payload = {
            RequisitionId: parseInt($("#reqId").val(), 10) || 0,
            RequisitionNumber: $("#reqNumber").val(),
            PositionTitle: $("#reqTitle").val(),
            Division: $("#reqDivision").val(),
            Department: $("#reqDepartment").val(),
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
