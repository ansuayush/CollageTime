(function () {

    var $root = $("#prTasksRoot");
    if (!$root.length || !window.PerformanceReviewForm) return;

    function url(n) { return $root.attr("data-" + n) || ""; }
    var currentTaskId = 0;
    var $modal = $("#prTasksModal");
    var $body = $("#prTasksModalBody");

    function notifyChanged() {
        $(document).trigger("pr-notifications-changed");
    }

    function load() {
        $.getJSON(url("list-url"), function (r) {
            var $tb = $("#prTasksTable tbody").empty();
            var rows = (r && r.data) || [];
            if (!rows.length) {
                $tb.append("<tr><td colspan='4' class='text-muted'>No pending performance tasks.</td></tr>");
                return;
            }
            rows.forEach(function (n) {
                var unread = n.IsRead === false ? " style='font-weight:bold;'" : "";
                var empLink = n.TaskId
                    ? "<a href='javascript:void(0)' class='pr-open-task'" + unread + " data-task-id='" + n.TaskId + "'>" + PerformanceReviewForm.esc(n.EmployeeName || "Employee") + "</a>"
                    : PerformanceReviewForm.esc(n.EmployeeName || "");
                var clearBtn = n.Id > 0
                    ? "<button type='button' class='btn btn-xs btn-link text-danger pr-dismiss' data-id='" + n.Id + "' title='Clear'><i class='fa fa-times'></i></button>"
                    : "";
                $tb.append("<tr data-task-id='" + (n.TaskId || "") + "'>" +
                    "<td>" + empLink + "</td>" +
                    "<td" + unread + ">" + PerformanceReviewForm.esc(n.Description || "") + "</td>" +
                    "<td>" + PerformanceReviewForm.esc(n.ReceivedOn || "") + "</td>" +
                    "<td class='text-center'>" + clearBtn + "</td></tr>");
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
            if (submit) {
                $modal.modal("hide");
                load();
                notifyChanged();
            }
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
            notifyChanged();
        });
    }

    function openTask(taskId) {
        if (!taskId) return;
        $.getJSON(url("task-url"), { id: taskId }, function (r) {
            if (!r || !r.success) { alert((r && r.message) || "Unable to load"); return; }
            renderTask(r.data);
        });
    }

    $(document).off(".prTasks");
    $(document).on("click.prTasks", ".pr-open-task", function (e) {
        e.preventDefault();
        openTask($(this).attr("data-task-id"));
    });
    $(document).on("click.prTasks", "#prTasksTable tbody tr[data-task-id]", function (e) {
        if ($(e.target).closest(".pr-dismiss, a").length) return;
        var taskId = $(this).attr("data-task-id");
        if (taskId) openTask(taskId);
    });
    $(document).on("click.prTasks", ".pr-dismiss", function (e) {
        e.preventDefault();
        e.stopPropagation();
        var id = $(this).attr("data-id");
        if (!id || !confirm("Clear this notification?")) return;
        $.post(url("dismiss-url"), { id: id }, function (res) {
            if (!res || !res.success) { alert((res && res.message) || "Unable to clear"); return; }
            load();
            notifyChanged();
        });
    });

    $body.off("click.prTasks").on("click.prTasks", ".pr-rf-save", function () { postSave(false); });
    $body.on("click.prTasks", ".pr-rf-submit", function () {
        if (confirm("Submit this review step? The next approver will be notified.")) postSave(true);
    });

    load();
})();
