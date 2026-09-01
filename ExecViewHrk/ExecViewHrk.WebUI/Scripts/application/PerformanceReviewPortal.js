(function () {

    var $root = $("#prMyRoot");
    if (!$root.length || !window.PerformanceReviewForm) return;

    function url(n) { return $root.attr("data-" + n) || ""; }
    var currentTaskId = 0;
    var $modal = $("#prMyModal");
    var $body = $("#prMyModalBody");

    function load() {
        $.getJSON(url("list-url"), function (r) {
            var $tb = $("#prMyTable tbody").empty();
            var rows = (r && r.data) || [];
            if (!rows.length) {
                $tb.append("<tr><td colspan='6' class='text-muted'>No open performance tasks.</td></tr>");
                return;
            }
            rows.forEach(function (t) {
                $tb.append("<tr><td>" + PerformanceReviewForm.esc(t.ReviewName) + "</td><td>" + PerformanceReviewForm.esc(t.EmployeeName) +
                    "</td><td>" + PerformanceReviewForm.esc(t.ReviewerRole) + "</td><td>" + PerformanceReviewForm.esc(t.DueDate) +
                    "</td><td>" + PerformanceReviewForm.esc(t.Status) +
                    "</td><td><button type='button' class='btn btn-xs btn-primary btn-open-task' data-id='" + t.Id + "'>Open</button></td></tr>");
            });
        });
    }

    function renderTask(t) {
        currentTaskId = t.Id;
        $("#prRejectOverlay").remove();
        $body.html(PerformanceReviewForm.render(t, { commentsId: "prRfComments", ansClass: "pr-rf-ans", cmtClass: "pr-rf-cmt" }));
        PerformanceReviewForm.bindLiveScore($body);
        PerformanceReviewForm.bindPrint($body);
        PerformanceReviewForm.bindAttachFile($body);
        PerformanceReviewForm.bindViewPrevious($body);
        PerformanceReviewForm.bindReject($body, postReject);
        $modal.modal("show");
    }

    function postSave(submit) {
        $.ajax({
            url: url("save-url"),
            type: "POST",
            dataType: "json",
            data: {
                id: currentTaskId,
                comments: $("#prRfComments").val() || "",
                answersJson: JSON.stringify(PerformanceReviewForm.collectAnswers("pr-rf-ans", "pr-rf-cmt")),
                submit: submit
            }
        }).done(function (res) {
            if (!res || !res.success) { alert((res && res.message) || "Failed"); return; }
            if (submit) { $modal.modal("hide"); load(); $(document).trigger("pr-notifications-changed"); }
        }).fail(function (xhr) {
            alert("Save failed" + (xhr && xhr.status ? " (HTTP " + xhr.status + ")" : ""));
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

    $(document).off(".prMy");
    $(document).on("click.prMy", ".btn-open-task", function () {
        var id = $(this).attr("data-id");
        $.getJSON(url("task-url"), { id: id }, function (r) {
            if (!r || !r.success) { alert((r && r.message) || "Unable to load"); return; }
            renderTask(r.data);
        });
    });

    $body.off("click.prMy").on("click.prMy", ".pr-rf-save", function () { postSave(false); });
    $body.on("click.prMy", ".pr-rf-submit", function () {
        if (confirm("Submit this review step?")) postSave(true);
    });

    load();
})();
