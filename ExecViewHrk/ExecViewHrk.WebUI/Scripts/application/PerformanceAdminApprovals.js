(function () {

    var $root = $("#prHrRoot");
    if (!$root.length || !window.PerformanceReviewForm) return;

    function url(n) { return $root.attr("data-" + n) || ""; }
    var currentTaskId = 0;
    var $modal = $("#prHrModal");
    var $body = $("#prHrModalBody");

    function load() {
        $.getJSON(url("queue-url"), function (r) {
            var $tb = $("#prHrTable tbody").empty();
            var rows = (r && r.data) || [];
            if (!rows.length) {
                $tb.append("<tr><td colspan='5' class='text-muted'>No HR tasks waiting.</td></tr>");
                return;
            }
            rows.forEach(function (t) {
                $tb.append("<tr><td>" + PerformanceReviewForm.esc(t.ReviewName) + "</td><td>" + PerformanceReviewForm.esc(t.EmployeeName) +
                    "</td><td>" + PerformanceReviewForm.esc(t.DueDate) + "</td><td>" + PerformanceReviewForm.esc(t.Status) +
                    "</td><td><button type='button' class='btn btn-xs btn-info btn-open-hr' data-id='" + t.Id + "'>Open</button></td></tr>");
            });
        });
    }

    function renderTask(t) {
        currentTaskId = t.Id;
        $("#prRejectOverlay").remove();
        $body.html(PerformanceReviewForm.render(t, { commentsId: "prRfComments", ansClass: "pr-rf-ans", cmtClass: "pr-rf-cmt" }));
        PerformanceReviewForm.bindPrint($body);
        PerformanceReviewForm.bindViewPrevious($body);
        PerformanceReviewForm.bindReject($body, postReject);
        $modal.modal("show");
    }

    function postSave(submit) {
        $.post(url("save-url"), {
            id: currentTaskId,
            comments: $("#prRfComments").val() || "",
            answersJson: "[]",
            submit: submit
        }, function (res) {
            if (!res || !res.success) { alert((res && res.message) || "Failed"); return; }
            if (submit) {
                $modal.modal("hide");
                load();
                $(document).trigger("pr-notifications-changed");
            }
        });
    }

    function postReject(targetRole, comments, reason) {
        $.post(url("reject-url"), {
            id: currentTaskId,
            targetRole: targetRole,
            comments: comments,
            reason: reason
        }, function (res) {
            if (!res || !res.success) { alert((res && res.message) || "Reject failed"); return; }
            PerformanceReviewForm.hideRejectOverlay();
            $modal.modal("hide");
            load();
            $(document).trigger("pr-notifications-changed");
        });
    }

    $(document).off(".prHr");
    $(document).on("click.prHr", ".btn-open-hr", function () {
        var id = $(this).attr("data-id");
        $.getJSON(url("task-url"), { id: id }, function (r) {
            if (!r || !r.success) { alert((r && r.message) || "Unable to load"); return; }
            renderTask(r.data);
        });
    });

    $body.off("click.prHr").on("click.prHr", ".pr-rf-save", function () { postSave(false); });
    $body.on("click.prHr", ".pr-rf-submit", function () {
        if (confirm("Approve final and complete this review?")) postSave(true);
    });

    load();
})();
