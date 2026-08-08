(function () {
    var $root = $("#benAssignRoot");
    if (!$root.length) return;
    function url(n) { return $root.attr("data-" + n) || ""; }

    var searchTimer = null;
    var selectedEmps = []; // { id, text }

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
        var day = ("0" + dt.getDate()).slice(-2);
        return dt.getFullYear() + "-" + m + "-" + day;
    }

    function renderSelected() {
        var $box = $("#benEmpSelected").empty();
        if (!selectedEmps.length) {
            $box.hide();
            $("#benEmpCount").text("");
            $("#btnClearEmps").hide();
            return;
        }
        selectedEmps.forEach(function (e) {
            var $chip = $("<span class='ben-emp-chip'/>").attr("data-id", e.id);
            $chip.append(document.createTextNode(e.text));
            $chip.append($("<button type='button' class='ben-remove-emp' title='Remove'>&times;</button>"));
            $box.append($chip);
        });
        $box.show();
        $("#benEmpCount").text(selectedEmps.length + " employee" + (selectedEmps.length === 1 ? "" : "s") + " selected");
        $("#btnClearEmps").show();
    }

    function isSelected(id) {
        return selectedEmps.some(function (e) { return String(e.id) === String(id); });
    }

    function addEmployee(id, text) {
        if (!id || isSelected(id)) return;
        selectedEmps.push({ id: String(id), text: text });
        renderSelected();
        $("#benEmpSearch").val("").focus();
        $("#benEmpResults").hide().empty();
    }

    function removeEmployee(id) {
        selectedEmps = selectedEmps.filter(function (e) { return String(e.id) !== String(id); });
        renderSelected();
    }

    function clearEmployees() {
        selectedEmps = [];
        renderSelected();
        $("#benEmpSearch").val("").focus();
        $("#benEmpResults").hide().empty();
    }

    function loadClasses() {
        $.getJSON(url("class-url"), function (r) {
            var $s = $("#benClassId").empty().append("<option value=''>-- Select class --</option>");
            ((r && r.data) || []).forEach(function (c) {
                if (c.IsActive) $s.append("<option value='" + c.BenefitClassId + "'>" + c.ClassName + "</option>");
            });
        });
    }

    function loadAssignments() {
        $.getJSON(url("list-url"), function (r) {
            var $tb = $("#benAssignTable tbody").empty();
            if (!r || r.success === false) {
                $tb.append("<tr><td colspan='4' class='text-danger'>" + ((r && r.message) || "Unable to load assignments.") + "</td></tr>");
                return;
            }
            var rows = (r && r.data) || [];
            if (!rows.length) {
                $tb.append("<tr><td colspan='4' class='text-muted'>No assignments yet.</td></tr>");
                return;
            }
            rows.forEach(function (a) {
                $tb.append("<tr><td>" + (a.EmployeeName || "") + "</td><td>" + (a.FileNumber || "") + "</td><td>" + (a.BenefitClassName || "") +
                    "</td><td>" + fmtDate(a.EffectiveDate) + "</td></tr>");
            });
        }).fail(function () {
            $("#benAssignTable tbody").html("<tr><td colspan='4' class='text-danger'>Unable to load assignments.</td></tr>");
        });
    }

    function searchEmp() {
        var q = $.trim($("#benEmpSearch").val() || "");
        if (q.length < 2) {
            $("#benEmpResults").hide().empty();
            return;
        }
        $.getJSON(url("search-url"), { q: q }, function (r) {
            var $list = $("#benEmpResults").empty();
            var rows = (r && r.data) || [];
            if (!rows.length) {
                $list.append("<li class='text-muted'>No matches</li>").show();
                return;
            }
            rows.forEach(function (e) {
                var already = isSelected(e.id);
                var $li = $("<li/>").attr("data-id", e.id).text(e.text + (already ? " (selected)" : ""));
                if (already) $li.addClass("disabled");
                $list.append($li);
            });
            $list.show();
        });
    }

    $("#benEmpSearch").on("keyup input", function () {
        clearTimeout(searchTimer);
        searchTimer = setTimeout(searchEmp, 250);
    });

    $("#benEmpResults").on("click", "li[data-id]:not(.disabled)", function () {
        addEmployee($(this).attr("data-id"), $(this).text().replace(/\s*\(selected\)\s*$/, ""));
    });

    $root.on("click", ".ben-remove-emp", function (e) {
        e.preventDefault();
        removeEmployee($(this).closest(".ben-emp-chip").attr("data-id"));
    });

    $("#btnClearEmps").on("click", function () { clearEmployees(); });

    $(document).on("click.benAssign", function (e) {
        if (!$(e.target).closest(".ben-emp-picker").length) {
            $("#benEmpResults").hide();
        }
    });

    $("#btnAssignClass").on("click", function () {
        var classId = $("#benClassId").val();
        if (!selectedEmps.length || !classId) {
            alert("Select at least one employee and a benefit class.");
            return;
        }
        var ids = selectedEmps.map(function (e) { return e.id; }).join(",");
        var postUrl = url("assign-url");
        if (!postUrl) {
            alert("Assign URL is missing. Reload the page.");
            return;
        }
        var $btn = $("#btnAssignClass").prop("disabled", true);
        $.ajax({
            url: postUrl,
            type: "POST",
            dataType: "json",
            data: {
                employeeIds: ids,
                benefitClassId: classId,
                effectiveDate: $("#benEffDate").val() || ""
            }
        }).done(function (res) {
            if (!res || !(res.success === true || res.Success === true)) {
                alert((res && (res.message || res.Message)) || "Assign failed.");
                return;
            }
            alert(res.message || res.Message || "Assigned");
            clearEmployees();
            loadAssignments();
        }).fail(function (xhr) {
            var msg = "Assign failed.";
            if (xhr && xhr.responseText) {
                try {
                    var parsed = JSON.parse(xhr.responseText);
                    if (parsed && (parsed.message || parsed.Message))
                        msg = parsed.message || parsed.Message;
                } catch (e) {
                    if (xhr.responseText.indexOf("Assign failed:") >= 0)
                        msg = xhr.responseText;
                    else if (xhr.status)
                        msg = "Assign failed (HTTP " + xhr.status + ").";
                }
            }
            alert(msg);
        }).always(function () {
            $btn.prop("disabled", false);
        });
    });

    if (!$("#benEffDate").val()) {
        $("#benEffDate").val(fmtDate(new Date()));
    }

    loadClasses();
    loadAssignments();
})();
