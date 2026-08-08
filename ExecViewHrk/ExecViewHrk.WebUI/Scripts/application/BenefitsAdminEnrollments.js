(function () {
    var $root = $("#benEnrollRoot");
    if (!$root.length) return;
    function url(n) { return $root.attr("data-" + n) || ""; }

    function load() {
        $.getJSON(url("list-url"), function (r) {
            var $tb = $("#benEnrollTable tbody").empty();
            ((r && r.data) || []).forEach(function (e) {
                var actions = "";
                if (e.Status === "Submitted") {
                    actions = "<button class='btn btn-xs btn-success btn-approve' data-id='" + e.EnrollmentId + "'>Approve</button>";
                }
                $tb.append("<tr><td>" + (e.EmployeeName || "") + "</td><td>" + (e.FileNumber || "") + "</td><td>" + (e.BenefitClassName || "") +
                    "</td><td>" + (e.EnrollmentPeriodName || "") + "</td><td>" + (e.Status || "") + "</td><td>" + (e.ConfirmationNumber || "") +
                    "</td><td>" + (e.SubmittedDate ? String(e.SubmittedDate).substring(0, 10) : "") + "</td><td>" + actions + "</td></tr>");
            });
        });
    }

    $(document).off("click.benApprove", ".btn-approve").on("click.benApprove", ".btn-approve", function () {
        if (!confirm("Approve this enrollment?")) return;
        $.post(url("approve-url"), { enrollmentId: $(this).data("id") }, function (res) {
            if (!res || !res.success) { alert((res && res.message) || "Failed"); return; }
            load();
        });
    });

    load();
})();
