(function () {

    var $root = $("#prCompletedRoot");
    if (!$root.length || !window.PerformanceReviewForm) return;

    function esc(s) { return PerformanceReviewForm.esc(s); }
    function url(n) { return $root.attr("data-" + n) || ""; }
    var $modal = $("#prCompletedModal");
    var $body = $("#prCompletedModalBody");

    function formatStepBadges(steps) {
        if (!steps || !steps.length) return "—";
        return steps.map(function (s) {
            var label = s.ReviewerLabel || s.ReviewerRole || "Step";
            var status = s.Status || "";
            var cls = status === "Submitted"
                ? "label-success"
                : (status === "Pending" || status === "InProgress" ? "label-warning" : "label-default");
            var parts = [label];
            if (s.ReviewerName) parts.push(s.ReviewerName);
            if (status === "Submitted" && s.SubmittedDate) parts.push(s.SubmittedDate);
            if (status === "Submitted" && s.Score != null) parts.push("Score: " + s.Score);
            return "<span class='label " + cls + " pr-step-badge' title='" + esc(parts.join(" · ")) + "'>" +
                esc(label) + (status === "Submitted" ? " <i class='fa fa-check'></i>" : "") +
                "</span>";
        }).join(" ");
    }

    function load() {
        var params = {};
        var empId = $root.attr("data-employee-id");
        if (empId) params.employeeId = empId;

        $.getJSON(url("list-url"), params, function (r) {
            var $tb = $("#prCompletedTable tbody").empty();
            if (!r || !r.success) {
                $tb.append("<tr><td colspan='9' class='text-danger'>" + esc((r && r.message) || "Unable to load") + "</td></tr>");
                return;
            }
            var rows = r.data || [];
            if (!rows.length) {
                $tb.append("<tr><td colspan='9' class='text-muted'>No performance reviews found.</td></tr>");
                return;
            }
            rows.forEach(function (row) {
                $tb.append("<tr data-review-employee-id='" + row.ReviewEmployeeId + "'>" +
                    "<td><a href='javascript:void(0)' class='pr-view-completed' data-id='" + row.ReviewEmployeeId + "'>View</a></td>" +
                    "<td>" + esc(row.ReviewName) + "</td>" +
                    "<td>" + esc(row.ReviewType || "") + "</td>" +
                    "<td>" + esc(row.InitiatedDate || "") + "</td>" +
                    "<td>" + esc(row.ReviewerSummary || "—") + "</td>" +
                    "<td>" + esc(row.CompletionDate || "—") + "</td>" +
                    "<td>" + esc(row.Status || "") + "</td>" +
                    "<td>" + (row.Score != null ? esc(row.Score) : "—") + "</td>" +
                    "<td class='pr-approvals-col'>" + formatStepBadges(row.Steps) + "</td>" +
                    "</tr>");
            });
        });
    }

    function openDetail(reviewEmployeeId) {
        $.getJSON(url("detail-url"), { reviewEmployeeId: reviewEmployeeId }, function (r) {
            if (!r || !r.success) { alert((r && r.message) || "Unable to load"); return; }
            $body.html(PerformanceReviewForm.renderCompletedReview(r.data));
            PerformanceReviewForm.bindPrint($body);
            PerformanceReviewForm.bindViewPrevious($body);
            $modal.modal("show");
        });
    }

    $(document).off(".prCompleted");
    $(document).on("click.prCompleted", ".pr-view-completed", function (e) {
        e.preventDefault();
        openDetail($(this).attr("data-id"));
    });
    $body.off("click.prCompleted").on("click.prCompleted", ".pr-completed-close", function () {
        $modal.modal("hide");
    });

    load();

})();
