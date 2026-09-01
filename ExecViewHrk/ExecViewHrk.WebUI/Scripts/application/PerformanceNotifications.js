(function () {
    var $bell = $("#prNotifBell");
    if (!$bell.length) return;

    var countUrl = $bell.attr("data-count-url");
    var tasksView = $bell.attr("data-tasks-view") || "TasksPartial";
    var tasksController = $bell.attr("data-tasks-controller") || "PerformanceReview";
    var pollMs = parseInt($bell.attr("data-poll-ms") || "60000", 10);

    function setBadge(count) {
        var n = parseInt(count, 10) || 0;
        var $badge = $("#prNotifBadge");
        if (n > 0) {
            $badge.text(n > 99 ? "99+" : String(n)).show();
        } else {
            $badge.hide().text("");
        }
    }

    function refreshCount() {
        if (!countUrl) return;
        $.getJSON(countUrl, function (r) {
            if (r && r.success) setBadge(r.count);
        });
    }

    $bell.off("click.prNotif").on("click.prNotif", function (e) {
        e.preventDefault();
        if (typeof LoadMenu === "function") {
            LoadMenu(tasksView, tasksController);
        }
    });

    $(document).off("pr-notifications-changed.prNotif").on("pr-notifications-changed.prNotif", refreshCount);
    refreshCount();
    if (pollMs > 0) setInterval(refreshCount, pollMs);
})();
